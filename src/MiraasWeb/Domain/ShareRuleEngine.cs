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
    private readonly List<ShareRule> _rules;

    public ShareRuleEngine()
    {
        _rules = new List<ShareRule>();
        initializeShareRules();
    }

    /// <summary>
    /// Initializes all share rules based on Islamic inheritance law (Ilm al-Fara'id).
    /// </summary>
    void initializeShareRules()
    {
        // Husband: 1/2 or 1/4
        _rules.Add(new ShareRule(RelationType.Husband, case_ =>
        {
            bool hasDescendants = case_.HasHeir(RelationType.Son) ||
                                 case_.HasHeir(RelationType.Daughter) ||
                                 case_.HasHeir(RelationType.SonOfSon) ||
                                 case_.HasHeir(RelationType.DaughterOfSon);
            
            return hasDescendants ? new Fraction(1, 4) : new Fraction(1, 2);
        }));

        // Wife: 1/4 or 1/8 (divided equally among all wives)
        _rules.Add(new ShareRule(RelationType.Wife, case_ =>
        {
            bool hasDescendants = case_.HasHeir(RelationType.Son) ||
                                 case_.HasHeir(RelationType.Daughter) ||
                                 case_.HasHeir(RelationType.SonOfSon) ||
                                 case_.HasHeir(RelationType.DaughterOfSon);
            
            var fractionPerWife = hasDescendants ? new Fraction(1, 8) : new Fraction(1, 4);
            
            // Each wife gets an equal share; will be divided by wife count in engine
            return fractionPerWife;
        }));

        // Son: Residuary (2x the share of daughter)
        // Handled in residuary distribution

        // Daughter: 1/2 (alone), 2/3 (2+), or 2:1 ratio with son
        _rules.Add(new ShareRule(RelationType.Daughter, case_ =>
        {
            int daughterCount = case_.GetHeirCount(RelationType.Daughter);
            int sonCount = case_.GetHeirCount(RelationType.Son);

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
        _rules.Add(new ShareRule(RelationType.Father, case_ =>
        {
            bool hasDescendants = case_.HasHeir(RelationType.Son) ||
                                 case_.HasHeir(RelationType.Daughter) ||
                                 case_.HasHeir(RelationType.SonOfSon) ||
                                 case_.HasHeir(RelationType.DaughterOfSon);

            if (hasDescendants)
                return new Fraction(1, 6);

            // No descendants: Father is residuary
            return new Fraction(0, 1);
        }));

        // Mother: 1/6 or 1/3
        _rules.Add(new ShareRule(RelationType.Mother, case_ =>
        {
            bool hasDescendants = case_.HasHeir(RelationType.Son) ||
                                 case_.HasHeir(RelationType.Daughter) ||
                                 case_.HasHeir(RelationType.SonOfSon) ||
                                 case_.HasHeir(RelationType.DaughterOfSon);

            bool hasSiblings = case_.GetHeirCount(RelationType.FullBrother) > 0 ||
                             case_.GetHeirCount(RelationType.FullSister) > 0 ||
                             case_.GetHeirCount(RelationType.ConsanguineBrother) > 0 ||
                             case_.GetHeirCount(RelationType.ConsanguineSister) > 0 ||
                             case_.GetHeirCount(RelationType.UterineBrother) > 0 ||
                             case_.GetHeirCount(RelationType.UterineSister) > 0;

            if (hasDescendants || hasSiblings)
                return new Fraction(1, 6);

            // No descendants or siblings
            bool hasHusband = case_.HasHeir(RelationType.Husband);
            if (!hasHusband)
                return new Fraction(1, 3);

            // If husband exists, mother gets 1/3 of residue
            return new Fraction(0, 1); // Handled separately
        }));

        // Grandfather: 1/6 or 1/6 + residue or residue
        _rules.Add(new ShareRule(RelationType.Grandfather, case_ =>
        {
            bool hasDescendants = case_.HasHeir(RelationType.Son) ||
                                 case_.HasHeir(RelationType.Daughter) ||
                                 case_.HasHeir(RelationType.SonOfSon) ||
                                 case_.HasHeir(RelationType.DaughterOfSon);

            if (hasDescendants)
                return new Fraction(1, 6);

            return new Fraction(0, 1);
        }));

        // Grandmother (maternal & paternal): 1/6
        _rules.Add(new ShareRule(RelationType.GrandmotherMaternal, case_ =>
            new Fraction(1, 6)));

        _rules.Add(new ShareRule(RelationType.GrandmotherPaternal, case_ =>
            new Fraction(1, 6)));

        // Uterine Brother: 1/6 (alone) or 1/3 (2+)
        _rules.Add(new ShareRule(RelationType.UterineBrother, case_ =>
        {
            int utBrotherCount = case_.GetHeirCount(RelationType.UterineBrother);
            int utSisterCount = case_.GetHeirCount(RelationType.UterineSister);
            int totalUterine = utBrotherCount + utSisterCount;

            if (totalUterine == 0)
                return new Fraction(0, 1);

            if (totalUterine == 1)
                return new Fraction(1, 6);

            return new Fraction(1, 3);
        }));

        // Uterine Sister: 1/6 (alone) or 1/3 (2+, divided equally)
        _rules.Add(new ShareRule(RelationType.UterineSister, case_ =>
        {
            int utBrotherCount = case_.GetHeirCount(RelationType.UterineBrother);
            int utSisterCount = case_.GetHeirCount(RelationType.UterineSister);
            int totalUterine = utBrotherCount + utSisterCount;

            if (totalUterine == 0)
                return new Fraction(0, 1);

            if (totalUterine == 1)
                return new Fraction(1, 6);

            return new Fraction(1, 3);
        }));

        // Daughter of Son: 1/2 (alone), 2/3 (2+), or 2:1 ratio with son of son
        _rules.Add(new ShareRule(RelationType.DaughterOfSon, case_ =>
        {
            int dosSisterCount = case_.GetHeirCount(RelationType.DaughterOfSon);
            int dosBrotherCount = case_.GetHeirCount(RelationType.SonOfSon);

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
        _rules.Add(new ShareRule(RelationType.FullSister, case_ =>
        {
            int sisterCount = case_.GetHeirCount(RelationType.FullSister);
            int brotherCount = case_.GetHeirCount(RelationType.FullBrother);
            bool hasDaughter = case_.HasHeir(RelationType.Daughter);

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
        _rules.Add(new ShareRule(RelationType.ConsanguineSister, case_ =>
        {
            int sisterCount = case_.GetHeirCount(RelationType.ConsanguineSister);
            int brotherCount = case_.GetHeirCount(RelationType.ConsanguineBrother);
            bool hasFullSister = case_.HasHeir(RelationType.FullSister);

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
    public Fraction GetShare(RelationType heirType, InheritanceCase case_)
    {
        var rule = _rules.FirstOrDefault(r => r.Heir == heirType);
        
        if (rule == null)
            return new Fraction(0, 1);

        return rule.Rule(case_);
    }

    /// <summary>
    /// Gets all applicable share rules for the given case.
    /// </summary>
    public List<ShareRule> GetApplicableRules(InheritanceCase case_)
    {
        return _rules
            .Where(r => case_.HasHeir(r.Heir))
            .ToList();
    }
}
