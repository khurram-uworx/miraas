using MiraasWeb.Abstractions;

namespace MiraasWeb.Domain;

class ShareRule
{
    public RelationType Heir { get; }
    public Func<InheritanceCase, (Fraction, string)> Rule { get; }
    public string Explanation { get; set; } = string.Empty;

    public ShareRule(RelationType heir, Func<InheritanceCase, (Fraction, string)> rule)
    {
        Heir = heir;
        Rule = rule ?? throw new ArgumentNullException(nameof(rule));
    }

    public ShareRule(RelationType heir, Func<InheritanceCase, Fraction> rule)
    {
        if (rule is null) throw new ArgumentNullException(nameof(rule));

        Heir = heir;
        Rule = i => (rule(i), this.Explanation);
    }
}

class ShareEngine
{
    readonly List<ShareRule> rules;

    public ShareEngine()
    {
        rules =
        [
            new ShareRule(RelationType.Husband, inheritanceCase =>
            inheritanceCase.DeceasedHasDescendants()
            ? (Fraction.Quarter, "Husband: 1/4 of estate (with children)")
            : (Fraction.Half, "Husband: 1/2 of estate (no children)")),

            new ShareRule(RelationType.Wife, inheritanceCase =>
            inheritanceCase.DeceasedHasDescendants()
            ? (Fraction.Eighth, "Wife: 1/8 of estate (with children)")
            : (Fraction.Quarter, "Wife: 1/4 of estate (no children)")),

            new ShareRule(RelationType.Son, inheritanceCase =>
            (Fraction.Zero, "Residuary heir (inherits remainder after fixed shares)")),

            new ShareRule(RelationType.SonOfSon, inheritanceCase =>
            (Fraction.Zero, "Residuary heir (inherits remainder after fixed shares)")),

            new ShareRule(RelationType.Daughter, inheritanceCase =>
            {
                int daughterCount = inheritanceCase.GetHeirCount(RelationType.Daughter);
                int sonCount = inheritanceCase.GetHeirCount(RelationType.Son);

                if (sonCount > 0)
                    return (Fraction.Zero, "Daughter: 2:1 ratio with son (residuary)");
                else if (daughterCount == 1)
                    return (Fraction.Half, "Daughter: 1/2 of estate (sole daughter)");
                if (daughterCount >= 2)
                    return (Fraction.TwoThirds, "Daughter: 2/3 of estate (multiple daughters, shared equally)");

                return (Fraction.Zero, "Residuary heir (inherits remainder after fixed shares)");
            }),

            new ShareRule(RelationType.Father, inheritanceCase =>
            inheritanceCase.DeceasedHasDescendants()
            ? (Fraction.Sixth, "Father: 1/6 of estate (with children)")
            : (Fraction.Zero, "Father: Residuary (remainder after fixed shares)")),

            new ShareRule(RelationType.Mother, inheritanceCase =>
            {
                if (inheritanceCase.DeceasedHasDescendants())
                    return (Fraction.Sixth, "Mother: 1/6 of estate (with children or siblings)");
                else if (inheritanceCase.DeceasedHasSiblings())
                    return (Fraction.Sixth, "Mother: 1/6 of estate (with children or siblings)");
                else if (!inheritanceCase.DeceasedHasSpouse() && !inheritanceCase.HasHeir(RelationType.Father))
                    return (Fraction.Third, "Mother: 1/3 of estate (no children or siblings)");
                else
                    return (Fraction.Zero, "Mother: 1/3 of residue (no children/sibling/spouse/father");
            }),

            new ShareRule(RelationType.Grandfather, inheritanceCase =>
            inheritanceCase.DeceasedHasDescendants() ? Fraction.Sixth : Fraction.Zero),

            new ShareRule(RelationType.GrandmotherMaternal, inheritanceCase =>
            inheritanceCase.HasHeir(RelationType.GrandmotherPaternal) // grand mothers share 1/6
            ? Fraction.Sixth / 2 : Fraction.Sixth),

            new ShareRule(RelationType.GrandmotherPaternal, inheritanceCase =>
            inheritanceCase.HasHeir(RelationType.GrandmotherMaternal) // grand mothers share 1/6
            ? Fraction.Sixth / 2 : Fraction.Sixth),

            new ShareRule(RelationType.UterineBrother, inheritanceCase =>
            {
                int utBrotherCount = inheritanceCase.GetHeirCount(RelationType.UterineBrother);
                int utSisterCount = inheritanceCase.GetHeirCount(RelationType.UterineSister);
                int totalUterine = utBrotherCount + utSisterCount;

                if (totalUterine == 0)
                    return Fraction.Zero;
                else if (totalUterine == 1)
                    return Fraction.Sixth;
                else
                    return Fraction.Third;
            }),

            new ShareRule(RelationType.UterineSister, inheritanceCase =>
            {
                int utBrotherCount = inheritanceCase.GetHeirCount(RelationType.UterineBrother);
                int utSisterCount = inheritanceCase.GetHeirCount(RelationType.UterineSister);
                int totalUterine = utBrotherCount + utSisterCount;

                if (totalUterine == 0)
                    return Fraction.Zero;
                else if (totalUterine == 1)
                    return Fraction.Sixth;
                else
                    return Fraction.Third;
            }),

            new ShareRule(RelationType.DaughterOfSon, inheritanceCase =>
            {
                int dosSisterCount = inheritanceCase.GetHeirCount(RelationType.DaughterOfSon);
                int dosBrotherCount = inheritanceCase.GetHeirCount(RelationType.SonOfSon);
                int daughterCount = inheritanceCase.GetHeirCount(RelationType.Daughter);

                if (dosSisterCount == 0)
                    return (Fraction.Zero, "No daughter of son");
                else if (dosBrotherCount > 0)
                    return (Fraction.Zero, "Daughter of son: residuary with son of son (2:1)");

                if (daughterCount > 0)
                {
                    if (daughterCount == 1)
                        return (Fraction.Sixth, "Daughter of son: 1/6 (completes 2/3 with daughter)");

                    return (Fraction.Zero, "Daughter of son: blocked (daughters already have 2/3)");
                }

                if (dosSisterCount == 1)
                    return (Fraction.Half, "Daughter of son: 1/2 (sole, no daughter)");
                else if (dosSisterCount >= 2)
                    return (Fraction.TwoThirds, "Daughter of son: 2/3 (multiple, no daughter)");

                return (Fraction.Zero, "Daughter of son");
            }),

            new ShareRule(RelationType.FullSister, inheritanceCase =>
            {
                int sisterCount = inheritanceCase.GetHeirCount(RelationType.FullSister);
                int brotherCount = inheritanceCase.GetHeirCount(RelationType.FullBrother);
                bool hasDaughter = inheritanceCase.HasHeir(RelationType.Daughter);

                if (sisterCount == 0)
                    return Fraction.Zero;

                if (brotherCount > 0)
                    return Fraction.Zero;

                if (hasDaughter)
                    return Fraction.Sixth;

                if (sisterCount == 1)
                    return Fraction.Half;
                else if (sisterCount >= 2)
                    return Fraction.TwoThirds;
                else
                    return Fraction.Zero;
            }),

            new ShareRule(RelationType.FullBrother, inheritanceCase =>
            (Fraction.Zero, "Residuary heir (inherits remainder after fixed shares)")),

            new ShareRule(RelationType.ConsanguineBrother, inheritanceCase =>
            (Fraction.Zero, "Residuary heir (inherits remainder after fixed shares)")),

            new ShareRule(RelationType.ConsanguineSister, inheritanceCase =>
            {
                int sisterCount = inheritanceCase.GetHeirCount(RelationType.ConsanguineSister);
                int brotherCount = inheritanceCase.GetHeirCount(RelationType.ConsanguineBrother);
                bool hasFullSister = inheritanceCase.HasHeir(RelationType.FullSister);

                if (sisterCount == 0 || hasFullSister)
                    return Fraction.Zero;

                if (brotherCount > 0)
                    return Fraction.Zero;

                if (sisterCount == 1)
                    return Fraction.Half;
                else if (sisterCount >= 2)
                    return Fraction.TwoThirds;
                else
                    return Fraction.Zero;
            }),
        ];
    }

    public static IEnumerable<RelationType> DetermineResiduaryGroup(InheritanceCase inheritanceCase)
    {
        RelationType[] children = [RelationType.Son, RelationType.Daughter];
        RelationType[] grandChildren = [RelationType.SonOfSon, RelationType.DaughterOfSon];
        RelationType[] fatherLine = [RelationType.Father];
        RelationType[] grandfatherLine = [RelationType.Grandfather /* add higher paternal ancestors if present */];
        RelationType[] fullBrotherLine = [RelationType.FullBrother, /* add sons of full brother if named in enum, e.g. SonOfFullBrother */];
        RelationType[] consanguineBrotherLine = [RelationType.ConsanguineBrother /* add their descendants */];
        
        // we now have a place here
        // RelationType[] paternalUncleLine = [RelationType.PaternalUncle /* add paternal uncle descendants if present */];

        if (inheritanceCase.HasAny(RelationType.Son))
            return children.Where(r => inheritanceCase.HasHeir(r)); // include daughter-of-son as asaba-bi-ghayrihi ?
        if (inheritanceCase.HasAny(RelationType.SonOfSon))
            return grandChildren.Where(r => inheritanceCase.HasHeir(r)).ToList();
        // what if out of two sons, one is missing

        if (inheritanceCase.HasHeir(RelationType.Father))
            return fatherLine.Where(r => inheritanceCase.HasHeir(r)).ToList();

        if (inheritanceCase.HasAny(RelationType.Grandfather))
            return grandfatherLine.Where(r => inheritanceCase.HasHeir(r)).ToList();

        if (inheritanceCase.HasAny(RelationType.FullBrother /* or their male descendants */))
        {
            var group = new List<RelationType>();
            if (inheritanceCase.HasHeir(RelationType.FullBrother)) group.Add(RelationType.FullBrother);
            if (inheritanceCase.HasHeir(RelationType.FullSister)) group.Add(RelationType.FullSister); // joins as asaba bi-ghayrihi

            // we now have a place here
            // full-brother descendants if present...
            
            return group;
        }

        if (inheritanceCase.HasAny(RelationType.ConsanguineBrother /* or their descendants */))
        {
            var group = new List<RelationType>();
            if (inheritanceCase.HasHeir(RelationType.ConsanguineBrother)) group.Add(RelationType.ConsanguineBrother);
            if (inheritanceCase.HasHeir(RelationType.ConsanguineSister)) group.Add(RelationType.ConsanguineSister); // joins as asaba bi-ghayrihi
            return group;
        }

        // we now have a place here
        //if (hasAny(inheritanceCase, RelationType.PaternalUncle /*, descendants */))
        //{
        //    var group = new List<RelationType>();
        //    if (inheritanceCase.HasHeir(RelationType.PaternalUncle)) group.Add(RelationType.PaternalUncle);
        //    // include their male-line descendants if available in your enum
        //    return group;
        //}

        // Special asaba ma'a ghayrihi for Full Sister triggered by presence of daughters
        bool hasDaughterOrDaughterOfSon = inheritanceCase.HasHeir(RelationType.Daughter) || inheritanceCase.HasHeir(RelationType.DaughterOfSon);
        if (hasDaughterOrDaughterOfSon && inheritanceCase.HasHeir(RelationType.FullSister))
            return [RelationType.FullSister];

        // if only spouse
        if (inheritanceCase.HasOnly(RelationType.Husband, RelationType.Wife))
            return [RelationType.Husband, RelationType.Wife];

        return [];
    }

    public static bool DistributeRadd(InheritanceCase inheritanceCase, List<Heir> fixedShareHeirs, Fraction residue)
    {
        if (residue.Numerator <= 0)
            return false;

        var raddEligibleHeirs = fixedShareHeirs
            .Where(h => h.Relation != RelationType.Husband && h.Relation != RelationType.Wife)
            .ToList();

        if (raddEligibleHeirs.Count == 0) return false;

        var totalFixedForRadd = Fraction.Zero;
        foreach (var heir in raddEligibleHeirs)
        {
            if (heir.Result?.Fraction != null)
                totalFixedForRadd += heir.Result.Fraction;
        }

        foreach (var heir in raddEligibleHeirs)
        {
            if (heir.Result?.Fraction == null || heir.Result.Fraction == Fraction.Zero)
                continue;

            var proportion = heir.Result.Fraction / totalFixedForRadd;
            var raddAmount = residue * proportion;

            heir.AddShare(new ShareResult(raddAmount)
            {
                Explanation = $"Radd ({raddAmount})"
            });
        }

        return true;
    }

    public static void DistributeResidue(InheritanceCase inheritanceCase, List<Heir> residuaryHeirs, Fraction residue)
    {
        if (residuaryHeirs.Count == 0 || residue.Numerator == 0)
            return;

        var maleHeirs = residuaryHeirs
            .Where(h => h.Gender == GenderType.Male)
            .ToList();

        var femaleHeirs = residuaryHeirs
            .Where(h => h.Gender == GenderType.Female)
            .ToList();

        if (maleHeirs.Count > 0 && femaleHeirs.Count > 0)
        {
            int totalUnits = (maleHeirs.Sum(h => h.Count) * 2) + (femaleHeirs.Sum(h => h.Count) * 1);
            var unitValue = residue / totalUnits;

            foreach (var male in maleHeirs)
            {
                var maleShare = unitValue * (2 * male.Count);
                male.AddShare(new ShareResult(maleShare)
                {
                    Explanation = $"Residuary male heir: {maleShare} (2 units x {male.Count} heirs)"
                });
            }

            foreach (var female in femaleHeirs)
            {
                var femaleShare = unitValue * female.Count;
                female.AddShare(new ShareResult(femaleShare)
                {
                    Explanation = $"Residuary female heir: {femaleShare} (1 unit x {female.Count} heirs)"
                });
            }
        }
        else if (maleHeirs.Count > 0)
        {
            int totalMales = maleHeirs.Sum(h => h.Count);
            var sharePerMale = residue / totalMales;

            foreach (var male in maleHeirs)
            {
                var share = sharePerMale * male.Count;
                male.AddShare(new ShareResult(share)
                {
                    Explanation = $"Residuary male heir: {share}"
                });
            }
        }
        else if (femaleHeirs.Count > 0)
        {
            int totalFemales = femaleHeirs.Sum(h => h.Count);
            var sharePerFemale = residue / totalFemales;

            foreach (var female in femaleHeirs)
            {
                var share = sharePerFemale * female.Count;
                female.AddShare(new ShareResult(share)
                {
                    Explanation = $"Residuary female heir: {share}"
                });
            }
        }
        else
        {
            var singleHeir = residuaryHeirs.First();
            singleHeir.AddShare(new ShareResult(residue)
            {
                Explanation = $"Sole residuary heir: {residue}"
            });
        }
    }

    string generateExplanation(RelationType relation) => relation switch
    {
        RelationType.UterineBrother or RelationType.UterineSister =>
            "Uterine sibling (same mother, different father)",
        RelationType.FullBrother or RelationType.FullSister =>
            "Full sibling (same parents)",
        RelationType.ConsanguineBrother or RelationType.ConsanguineSister =>
            "Consanguine sibling (same father, different mother)",
        _ => $"Heir: {relation}"
    };

    public (Fraction ShareFaction, string ShareExplaination) GetShare(RelationType heirType, InheritanceCase inheritanceCase)
    {
        var rule = rules.FirstOrDefault(r => r.Heir == heirType);

        if (rule == null)
            return (Fraction.Zero, "No rule found");
        else
        {
            (var s, var e) = rule.Rule(inheritanceCase);
            return (s, string.IsNullOrWhiteSpace(e) ? generateExplanation(heirType) : e);
        }
    }

    public IEnumerable<ShareRule> GetApplicableRules(InheritanceCase inheritanceCase) =>
        rules.Where(r => inheritanceCase.HasHeir(r.Heir));
}
