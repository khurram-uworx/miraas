using MiraasWeb.Abstractions;
using System.Text;

namespace MiraasWeb.Domain;

public class CalculationResult
{
    public List<Heir> Heirs { get; set; } = new();
    public Fraction TotalFraction { get; set; }
    public bool RequiresScholarlyReview { get; set; }
    public List<string> Warnings { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        Action<string> append = s =>
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            if (sb.Length > 0) sb.Append(Environment.NewLine);
            sb.Append(s);
        };

        if (!this.IsSuccessful)
        {
            if (!string.IsNullOrWhiteSpace(this.ErrorMessage))
                append($"Error: {this.ErrorMessage}");
            else
                append("Error");

            return sb.ToString();
        }

        if (this.RequiresScholarlyReview) append("Requires Scholarly Review");
        if (!string.IsNullOrWhiteSpace(this.Summary)) append($"Summary: {this.Summary}");

        foreach (var heir in Heirs)
            append(heir.ToString());

        if (!string.IsNullOrWhiteSpace(this.ErrorMessage))
            append($"Error: {this.ErrorMessage}");

        if (this.Warnings.Count > 0)
        {
            append("Warnings");
            this.Warnings.ForEach(s => append($"- {s}"));
        }

        return sb.ToString();
    }

    // factory methods
    public static CalculationResult Success(List<Heir> heirs, Fraction totalFraction) =>
        new CalculationResult
        {
            Heirs = heirs.Where(h => h.Result.Fraction > Fraction.Zero).ToList(),
            TotalFraction = totalFraction,
            IsSuccessful = true
        };
    public static CalculationResult Failure(string errorMessage) =>
        new CalculationResult
        {
            IsSuccessful = false,
            ErrorMessage = errorMessage
        };
}

public class CalculationEngine
{
    readonly BlockingEngine blockingEngine = new();
    readonly ShareEngine shareEngine = new();

    public CalculationResult Calculate(InheritanceCase inheritanceCase)
    {
        try
        {
            // Validate input
            Validator validator = new(inheritanceCase);
            var validationResult = validator.Validate();
            if (!validationResult.IsValid)
                return CalculationResult.Failure(
                    $"Invalid inheritance case: {string.Join("; ", validationResult.Errors)}");

            if (inheritanceCase.Heirs.Count() == 0)
                return CalculationResult.Failure("No heirs specified.");

            // Identify blocked heirs and filter them out
            var blockedRelations = blockingEngine.GetBlockedHeirs(inheritanceCase);
            var activeHeirs = inheritanceCase.Heirs
                .Where(h => !blockedRelations.Contains(h.Relation))
                .ToList();

            if (activeHeirs.Count == 0)
                return CalculationResult.Failure("All heirs are blocked from inheritance.");

            var totalFixed = Fraction.Zero;
            var fixedShareHeirs = new List<Heir>();
            var residuaryHeirs = new List<Heir>();

            // Calculate fixed shares
            foreach (var heir in activeHeirs)
            {
                var share = shareEngine.GetShare(heir.Relation, inheritanceCase);

                if (share.ShareFaction > Fraction.Zero)
                {
                    heir.AddShare(new ShareResult(share.ShareFaction)
                    {
                        Explanation = share.ShareExplaination
                    });
                    totalFixed += share.ShareFaction;
                    fixedShareHeirs.Add(heir);
                }
            }

            // Check for Awl (share overflow) - when fixed shares exceed 100%
            if (totalFixed > Fraction.One)
            {
                // Apply Awl: proportionally reduce all fixed shares
                // This is a fundamental principle in Islamic inheritance
                // Example: If shares total 13/12, each share is multiplied by 12/13
                var awlDenominator = totalFixed;
                foreach (var heir in fixedShareHeirs)
                {
                    var originalShare = heir.Result.Fraction;
                    var reducedShare = originalShare / awlDenominator;
                    heir.Result.Fraction = reducedShare;
                    heir.Result.Explanation += $" (Awl applied: {originalShare} reduced to {reducedShare})";
                }
                totalFixed = Fraction.One;
            }
            else
            {
                // Residue if any (when fixed shares < 100%)
                var residue = Fraction.One - totalFixed;

                if (residue > Fraction.Zero
                    && inheritanceCase.Heirs.FirstOrDefault(h => h.Relation == RelationType.Mother) is { } mother
                    && !fixedShareHeirs.Exists(h => h.Relation == RelationType.Mother))
                {
                    // Mother exists but she hasnt got fixed share
                    var share = residue * Fraction.Third;
                    mother.AddShare(new ShareResult(share)
                    {
                        Explanation = "1/3 of residue"
                    });

                    fixedShareHeirs.Add(mother); // so she can participate in Radd

                    residue -= share;
                    totalFixed += share;
                }

                var residuaryRelations = ShareEngine.DetermineResiduaryGroup(inheritanceCase).ToList();
                residuaryHeirs = inheritanceCase.Heirs.Where(h => residuaryRelations.Contains(h.Relation)).ToList();

                if (residue > Fraction.Zero && residuaryHeirs.Count > 0)
                {
                    ShareEngine.DistributeResidue(inheritanceCase, residuaryHeirs, residue);
                    totalFixed += residue;
                }
                else if (residue > Fraction.Zero)
                {
                    if (ShareEngine.DistributeRadd(inheritanceCase, fixedShareHeirs, residue))
                        totalFixed += residue;
                }
            }

            // Compiling result
            List<Heir> allCalculatedHeirs = [.. fixedShareHeirs, .. residuaryHeirs];
            var result = CalculationResult.Success(allCalculatedHeirs, totalFixed);

            if (totalFixed != Fraction.One)
            {
                result.RequiresScholarlyReview = true;
                result.Warnings.Add($"Total fraction is {totalFixed}, not 1 (may indicate Awl or Radd scenario).");
            }

            return result;
        }
        catch (Exception ex)
        {
            return CalculationResult.Failure($"Calculation error: {ex.Message}");
        }
    }
}
