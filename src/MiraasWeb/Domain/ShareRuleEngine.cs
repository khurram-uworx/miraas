using MiraasWeb.Abstractions;

namespace MiraasWeb.Domain;

/// <summary>
/// Rule for calculating a specific heir's share based on case conditions.
/// </summary>
public class ShareRule
{
    public RelationType Heir { get; }
    public Func<InheritanceCase, Fraction> Rule { get; }

    public ShareRule(RelationType heir, Func<InheritanceCase, Fraction> rule)
    {
        Heir = heir;
        Rule = rule ?? throw new ArgumentNullException(nameof(rule));
    }
}

/// <summary>
/// Engine that calculates heir shares based on Islamic inheritance rules.
/// Implements all Quranic fractions and conditions from RULES.md.
/// </summary>
public class ShareRuleEngine
{
    readonly List<ShareRule> rules;

    public ShareRuleEngine()
    {
        rules = new List<ShareRule>();
        initializeShareRules();
    }

    /// <summary>
    /// Initializes all share rules based on Islamic inheritance law (Ilm al-Fara'id).
    /// </summary>
    void initializeShareRules()
    {
        // Husband: 1/2 or 1/4
        rules.Add(new ShareRule(RelationType.Husband, inheritanceCase =>
        {
            bool hasDescendants = inheritanceCase.HasHeir(RelationType.Son) ||
                                 inheritanceCase.HasHeir(RelationType.Daughter) ||
                                 inheritanceCase.HasHeir(RelationType.SonOfSon) ||
                                 inheritanceCase.HasHeir(RelationType.DaughterOfSon);
            
            return hasDescendants ? new Fraction(1, 4) : new Fraction(1, 2);
        }));

        // Wife: 1/4 or 1/8 (divided equally among all wives)
        rules.Add(new ShareRule(RelationType.Wife, inheritanceCase =>
        {
            bool hasDescendants = inheritanceCase.HasHeir(RelationType.Son) ||
                                 inheritanceCase.HasHeir(RelationType.Daughter) ||
                                 inheritanceCase.HasHeir(RelationType.SonOfSon) ||
                                 inheritanceCase.HasHeir(RelationType.DaughterOfSon);
            
            var fractionPerWife = hasDescendants ? new Fraction(1, 8) : new Fraction(1, 4);
            
            // Each wife gets an equal share; will be divided by wife count in engine
            return fractionPerWife;
        }));

        // Son: Residuary (2x the share of daughter)
        // Handled in residuary distribution

        // Daughter: 1/2 (alone), 2/3 (2+), or 2:1 ratio with son
        rules.Add(new ShareRule(RelationType.Daughter, inheritanceCase =>
        {
            int daughterCount = inheritanceCase.GetHeirCount(RelationType.Daughter);
            int sonCount = inheritanceCase.GetHeirCount(RelationType.Son);

            if (sonCount > 0)
            {
                // Will be calculated as 2:1 ratio in residuary distribution
                return new Fraction(0, 1);
            }

            if (daughterCount == 1)
                return new Fraction(1, 2);
            
            if (daughterCount >= 2)
                return new Fraction(2, 3);

            return new Fraction(0, 1);
        }));

        // Father: 1/6 or 1/6 + residue or residue
        rules.Add(new ShareRule(RelationType.Father, inheritanceCase =>
        {
            bool hasDescendants = inheritanceCase.HasHeir(RelationType.Son) ||
                                 inheritanceCase.HasHeir(RelationType.Daughter) ||
                                 inheritanceCase.HasHeir(RelationType.SonOfSon) ||
                                 inheritanceCase.HasHeir(RelationType.DaughterOfSon);

            if (hasDescendants)
                return new Fraction(1, 6);

            // No descendants: Father is residuary
            return new Fraction(0, 1);
        }));

        // Mother: 1/6 or 1/3
        rules.Add(new ShareRule(RelationType.Mother, inheritanceCase =>
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
                return new Fraction(1, 6);

            // No descendants or siblings
            bool hasHusband = inheritanceCase.HasHeir(RelationType.Husband);
            if (!hasHusband)
                return new Fraction(1, 3);

            // If husband exists, mother gets 1/3 of residue
            return new Fraction(0, 1); // Handled separately
        }));

        // Grandfather: 1/6 or 1/6 + residue or residue
        rules.Add(new ShareRule(RelationType.Grandfather, inheritanceCase =>
        {
            bool hasDescendants = inheritanceCase.HasHeir(RelationType.Son) ||
                                 inheritanceCase.HasHeir(RelationType.Daughter) ||
                                 inheritanceCase.HasHeir(RelationType.SonOfSon) ||
                                 inheritanceCase.HasHeir(RelationType.DaughterOfSon);

            if (hasDescendants)
                return new Fraction(1, 6);

            return new Fraction(0, 1);
        }));

        // Grandmother (maternal & paternal): 1/6
        rules.Add(new ShareRule(RelationType.GrandmotherMaternal, inheritanceCase =>
            new Fraction(1, 6)));

        rules.Add(new ShareRule(RelationType.GrandmotherPaternal, inheritanceCase =>
            new Fraction(1, 6)));

        // Uterine Brother: 1/6 (alone) or 1/3 (2+)
        rules.Add(new ShareRule(RelationType.UterineBrother, inheritanceCase =>
        {
            int utBrotherCount = inheritanceCase.GetHeirCount(RelationType.UterineBrother);
            int utSisterCount = inheritanceCase.GetHeirCount(RelationType.UterineSister);
            int totalUterine = utBrotherCount + utSisterCount;

            if (totalUterine == 0)
                return new Fraction(0, 1);

            if (totalUterine == 1)
                return new Fraction(1, 6);

            return new Fraction(1, 3);
        }));

        // Uterine Sister: 1/6 (alone) or 1/3 (2+, divided equally)
        rules.Add(new ShareRule(RelationType.UterineSister, inheritanceCase =>
        {
            int utBrotherCount = inheritanceCase.GetHeirCount(RelationType.UterineBrother);
            int utSisterCount = inheritanceCase.GetHeirCount(RelationType.UterineSister);
            int totalUterine = utBrotherCount + utSisterCount;

            if (totalUterine == 0)
                return new Fraction(0, 1);

            if (totalUterine == 1)
                return new Fraction(1, 6);

            return new Fraction(1, 3);
        }));

        // Daughter of Son: 1/2 (alone), 2/3 (2+), or 2:1 ratio with son of son
        rules.Add(new ShareRule(RelationType.DaughterOfSon, inheritanceCase =>
        {
            int dosSisterCount = inheritanceCase.GetHeirCount(RelationType.DaughterOfSon);
            int dosBrotherCount = inheritanceCase.GetHeirCount(RelationType.SonOfSon);

            if (dosSisterCount == 0)
                return new Fraction(0, 1);

            if (dosBrotherCount > 0)
            {
                // Will be calculated as 2:1 ratio
                return new Fraction(0, 1);
            }

            if (dosSisterCount == 1)
                return new Fraction(1, 2);

            if (dosSisterCount >= 2)
                return new Fraction(2, 3);

            return new Fraction(0, 1);
        }));

        // Full Sister: 1/2 (alone), 2/3 (2+), 1/6 (with daughter), or 2:1 ratio with brother
        rules.Add(new ShareRule(RelationType.FullSister, inheritanceCase =>
        {
            int sisterCount = inheritanceCase.GetHeirCount(RelationType.FullSister);
            int brotherCount = inheritanceCase.GetHeirCount(RelationType.FullBrother);
            bool hasDaughter = inheritanceCase.HasHeir(RelationType.Daughter);

            if (sisterCount == 0)
                return new Fraction(0, 1);

            if (brotherCount > 0)
            {
                // Will be calculated as 2:1 ratio
                return new Fraction(0, 1);
            }

            if (hasDaughter)
                return new Fraction(1, 6);

            if (sisterCount == 1)
                return new Fraction(1, 2);

            if (sisterCount >= 2)
                return new Fraction(2, 3);

            return new Fraction(0, 1);
        }));

        // Consanguine Sister: 1/2 (alone), 2/3 (2+), or 2:1 ratio with brother
        rules.Add(new ShareRule(RelationType.ConsanguineSister, inheritanceCase =>
        {
            int sisterCount = inheritanceCase.GetHeirCount(RelationType.ConsanguineSister);
            int brotherCount = inheritanceCase.GetHeirCount(RelationType.ConsanguineBrother);
            bool hasFullSister = inheritanceCase.HasHeir(RelationType.FullSister);

            if (sisterCount == 0 || hasFullSister)
                return new Fraction(0, 1);

            if (brotherCount > 0)
            {
                // Will be calculated as 2:1 ratio
                return new Fraction(0, 1);
            }

            if (sisterCount == 1)
                return new Fraction(1, 2);

            if (sisterCount >= 2)
                return new Fraction(2, 3);

            return new Fraction(0, 1);
        }));

        // Son of Son: Residuary (2x the share of daughter of son)
        // Handled in residuary distribution
    }

    /// <summary>
    /// Gets the share for a specific heir based on the case.
    /// </summary>
    public Fraction GetShare(RelationType heirType, InheritanceCase inheritanceCase)
    {
        var rule = rules.FirstOrDefault(r => r.Heir == heirType);
        
        if (rule == null)
            return new Fraction(0, 1);

        return rule.Rule(inheritanceCase);
    }

    /// <summary>
    /// Gets all applicable share rules for the given case.
    /// </summary>
    public List<ShareRule> GetApplicableRules(InheritanceCase inheritanceCase)
    {
        return rules
            .Where(r => inheritanceCase.HasHeir(r.Heir))
            .ToList();
    }
}
