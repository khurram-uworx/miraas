namespace MiraasWeb.Domain;

/// <summary>
/// Main engine for calculating Islamic inheritance shares.
/// Implements the complete calculation pipeline:
/// 1. Validate input
/// 2. Apply blocking rules
/// 3. Allocate fixed shares
/// 4. Calculate remainder
/// 5. Distribute remainder to residuaries
/// 6. Normalize fractions
/// 7. Generate explanations
/// </summary>
public class InheritanceEngine
{
    readonly InheritanceValidator validator;
    readonly BlockingRuleEngine blockingEngine;
    readonly ShareRuleEngine shareEngine;

    public InheritanceEngine()
    {
        validator = new InheritanceValidator();
        blockingEngine = new BlockingRuleEngine();
        shareEngine = new ShareRuleEngine();
    }

    /// <summary>
    /// Distributes the residue to residuary heirs according to Islamic law (2:1 ratio for males vs females).
    /// </summary>
    void distributeResidue(List<Heir> residuaryHeirs, Fraction residue, InheritanceCase inheritanceCase)
    {
        if (residuaryHeirs.Count == 0 || residue.Numerator == 0)
            return;

        // Handle male/female residuaries with 2:1 ratio
        var maleHeirs = residuaryHeirs
            .Where(h => h.Relation == RelationType.Son || 
                        h.Relation == RelationType.SonOfSon ||
                        h.Relation == RelationType.Father ||
                        h.Relation == RelationType.FullBrother ||
                        h.Relation == RelationType.ConsanguineBrother)
            .ToList();

        var femaleHeirs = residuaryHeirs
            .Where(h => h.Relation == RelationType.Daughter ||
                        h.Relation == RelationType.DaughterOfSon ||
                        h.Relation == RelationType.FullSister ||
                        h.Relation == RelationType.ConsanguineSister)
            .ToList();

        if (maleHeirs.Count > 0 && femaleHeirs.Count > 0)
        {
            // Calculate total units: males = 2 units each, females = 1 unit each
            int totalUnits = (maleHeirs.Sum(h => h.Count) * 2) + (femaleHeirs.Sum(h => h.Count) * 1);
            var unitValue = residue / totalUnits;

            foreach (var male in maleHeirs)
            {
                var maleShare = unitValue * (2 * male.Count);
                male.Result = new ShareResult(maleShare)
                {
                    Explanation = $"Residuary male heir: {maleShare} (2 units × {male.Count} heirs)"
                };
            }

            foreach (var female in femaleHeirs)
            {
                var femaleShare = unitValue * female.Count;
                female.Result = new ShareResult(femaleShare)
                {
                    Explanation = $"Residuary female heir: {femaleShare} (1 unit × {female.Count} heirs)"
                };
            }
        }
        else if (maleHeirs.Count > 0)
        {
            // Only males: distribute equally
            int totalMales = maleHeirs.Sum(h => h.Count);
            var sharePerMale = residue / totalMales;

            foreach (var male in maleHeirs)
            {
                var share = sharePerMale * male.Count;
                male.Result = new ShareResult(share)
                {
                    Explanation = $"Residuary male heir: {share}"
                };
            }
        }
        else if (femaleHeirs.Count > 0)
        {
            // Only females: distribute equally
            int totalFemales = femaleHeirs.Sum(h => h.Count);
            var sharePerFemale = residue / totalFemales;

            foreach (var female in femaleHeirs)
            {
                var share = sharePerFemale * female.Count;
                female.Result = new ShareResult(share)
                {
                    Explanation = $"Residuary female heir: {share}"
                };
            }
        }
        else
        {
            // Only one residuary heir (e.g., father alone)
            var singleHeir = residuaryHeirs.First();
            singleHeir.Result = new ShareResult(residue)
            {
                Explanation = $"Sole residuary heir: {residue}"
            };
        }
    }

    /// <summary>
    /// Generates a human-readable explanation for an heir's share.
    /// </summary>
    string generateExplanation(Heir heir, InheritanceCase inheritanceCase)
    {
        return heir.Relation switch
        {
            RelationType.Wife => generateWifeExplanation(inheritanceCase),
            RelationType.Husband => generateHusbandExplanation(inheritanceCase),
            RelationType.Son => "Residuary heir (inherits remainder after fixed shares)",
            RelationType.Daughter => generateDaughterExplanation(inheritanceCase),
            RelationType.Father => generateFatherExplanation(inheritanceCase),
            RelationType.Mother => generateMotherExplanation(inheritanceCase),
            RelationType.UterineBrother or RelationType.UterineSister => 
                "Uterine sibling (same mother, different father)",
            RelationType.FullBrother or RelationType.FullSister => 
                "Full sibling (same parents)",
            RelationType.ConsanguineBrother or RelationType.ConsanguineSister => 
                "Consanguine sibling (same father, different mother)",
            _ => $"Heir: {heir.Relation}"
        };
    }

    string generateWifeExplanation(InheritanceCase inheritanceCase)
    {
        bool hasDescendants = inheritanceCase.HasHeir(RelationType.Son) ||
                             inheritanceCase.HasHeir(RelationType.Daughter) ||
                             inheritanceCase.HasHeir(RelationType.SonOfSon) ||
                             inheritanceCase.HasHeir(RelationType.DaughterOfSon);

        if (hasDescendants)
            return "Wife: 1/8 of estate (with children)";
        
        return "Wife: 1/4 of estate (no children)";
    }

    string generateHusbandExplanation(InheritanceCase inheritanceCase)
    {
        bool hasDescendants = inheritanceCase.HasHeir(RelationType.Son) ||
                             inheritanceCase.HasHeir(RelationType.Daughter) ||
                             inheritanceCase.HasHeir(RelationType.SonOfSon) ||
                             inheritanceCase.HasHeir(RelationType.DaughterOfSon);

        if (hasDescendants)
            return "Husband: 1/4 of estate (with children)";
        
        return "Husband: 1/2 of estate (no children)";
    }

    string generateDaughterExplanation(InheritanceCase inheritanceCase)
    {
        int daughters = inheritanceCase.GetHeirCount(RelationType.Daughter);
        int sons = inheritanceCase.GetHeirCount(RelationType.Son);

        if (sons > 0)
            return "Daughter: 2:1 ratio with son (residuary)";
        
        if (daughters == 1)
            return "Daughter: 1/2 of estate (sole daughter)";
        
        if (daughters >= 2)
            return "Daughter: 2/3 of estate (multiple daughters, shared equally)";

        return "Daughter";
    }

    string generateFatherExplanation(InheritanceCase inheritanceCase)
    {
        bool hasDescendants = inheritanceCase.HasHeir(RelationType.Son) ||
                             inheritanceCase.HasHeir(RelationType.Daughter) ||
                             inheritanceCase.HasHeir(RelationType.SonOfSon) ||
                             inheritanceCase.HasHeir(RelationType.DaughterOfSon);

        if (hasDescendants)
            return "Father: 1/6 of estate (with children)";
        
        return "Father: Residuary (remainder after fixed shares)";
    }

    string generateMotherExplanation(InheritanceCase inheritanceCase)
    {
        bool hasDescendants = inheritanceCase.HasHeir(RelationType.Son) ||
                             inheritanceCase.HasHeir(RelationType.Daughter) ||
                             inheritanceCase.HasHeir(RelationType.SonOfSon) ||
                             inheritanceCase.HasHeir(RelationType.DaughterOfSon);

        bool hasSiblings = inheritanceCase.GetHeirCount(RelationType.FullBrother) > 0 ||
                         inheritanceCase.GetHeirCount(RelationType.FullSister) > 0 ||
                         inheritanceCase.GetHeirCount(RelationType.ConsanguineBrother) > 0 ||
                         inheritanceCase.GetHeirCount(RelationType.ConsanguineSister) > 0 ||
                         inheritanceCase.GetHeirCount(RelationType.UterineBrother) > 0 ||
                         inheritanceCase.GetHeirCount(RelationType.UterineSister) > 0;

        if (hasDescendants || hasSiblings)
            return "Mother: 1/6 of estate (with children or siblings)";
        
        return "Mother: 1/3 of estate (no children or siblings)";
    }

    /// <summary>
    /// Calculates inheritance shares for the given case.
    /// </summary>
    public CalculationResult Calculate(InheritanceCase inheritanceCase)
    {
        try
        {
            // Step 1: Validate input
            var validationResult = validator.Validate(inheritanceCase);
            if (!validationResult.IsValid)
            {
                return CalculationResult.Failure(
                    $"Invalid inheritance case: {string.Join("; ", validationResult.Errors)}");
            }

            if (inheritanceCase.Heirs.Count == 0)
            {
                return CalculationResult.Failure("No heirs specified.");
            }

            // Step 2: Identify blocked heirs and filter them out
            var blockedRelations = blockingEngine.GetBlockedHeirs(inheritanceCase);
            var activeHeirs = inheritanceCase.Heirs
                .Where(h => !blockedRelations.Contains(h.Relation))
                .ToList();

            if (activeHeirs.Count == 0)
            {
                return CalculationResult.Failure("All heirs are blocked from inheritance.");
            }

            // Step 3: Calculate fixed shares
            var fixedShareHeirs = new List<Heir>();
            var residuaryHeirs = new List<Heir>();
            var totalFixed = new Fraction(0, 1);

            foreach (var heir in activeHeirs)
            {
                var share = shareEngine.GetShare(heir.Relation, inheritanceCase);

                if (share.Numerator > 0)
                {
                    heir.Result = new ShareResult(share)
                    {
                        Explanation = generateExplanation(heir, inheritanceCase)
                    };

                    if (heir.Relation == RelationType.Wife)
                    {
                        // Each wife gets an equal share of the total wife allocation
                        totalFixed += share / heir.Count;
                    }
                    else if (heir.Relation == RelationType.UterineBrother ||
                             heir.Relation == RelationType.UterineSister)
                    {
                        // Uterine siblings share equally
                        totalFixed += share / heir.Count;
                    }
                    else if (heir.Count > 1 &&
                            (heir.Relation == RelationType.Daughter ||
                             heir.Relation == RelationType.FullSister ||
                             heir.Relation == RelationType.ConsanguineSister ||
                             heir.Relation == RelationType.DaughterOfSon))
                    {
                        // Female heirs with the same count share the fraction equally
                        totalFixed += share / heir.Count;
                    }
                    else
                    {
                        totalFixed += share;
                    }

                    fixedShareHeirs.Add(heir);
                }
                else
                {
                    // Residuary heir
                    residuaryHeirs.Add(heir);
                }
            }

            // Step 4: Calculate residue and distribute to residuary heirs
            var residue = new Fraction(1, 1) - totalFixed;

            if (residue.Numerator < 0)
            {
                return CalculationResult.Failure(
                    "Fixed shares exceed total estate (Awl scenario - requires scholarly intervention).");
            }

            distributeResidue(residuaryHeirs, residue, inheritanceCase);
            totalFixed += residue;

            // Step 5: Combine all heirs
            var allCalculatedHeirs = new List<Heir>();
            allCalculatedHeirs.AddRange(fixedShareHeirs);
            allCalculatedHeirs.AddRange(residuaryHeirs);

            // Step 6: Generate result
            var result = CalculationResult.Success(allCalculatedHeirs, totalFixed);

            if (totalFixed.Numerator != 1 || totalFixed.Denominator != 1)
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
