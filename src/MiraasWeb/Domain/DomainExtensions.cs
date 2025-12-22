namespace MiraasWeb.Domain;

static class DomainExtensions
{
    public static bool HasAny(this InheritanceCase c, params RelationType[] relations)
    {
        foreach (var r in relations)
            if (c.HasHeir(r)) return true;

        return false;
    }

    public static bool HasOnly(this InheritanceCase c, params RelationType[] allowed)
    {
        foreach (RelationType r in Enum.GetValues(typeof(RelationType)))
        {
            if (allowed.Contains(r))
                continue;

            if (c.HasHeir(r))
                return false;
        }

        return true;
    }

    public static bool DeceasedHasChildren(this InheritanceCase i) =>
        i.HasHeir(RelationType.Son)
        || i.HasHeir(RelationType.Daughter);

    public static bool DeceasedHasGrandChildren(this InheritanceCase i) =>
        i.HasHeir(RelationType.SonOfSon)
        || i.HasHeir(RelationType.DaughterOfSon);

    public static bool DeceasedHasDescendants(this InheritanceCase i) =>
        DeceasedHasChildren(i)
        || DeceasedHasGrandChildren(i);

    public static bool DeceasedHasMaleDescendants(this InheritanceCase i) =>
        i.HasHeir(RelationType.Son)
        || i.HasHeir(RelationType.SonOfSon);

    public static bool DeceasedHasMaleAscendants(this InheritanceCase i) =>
        i.HasHeir(RelationType.Father)
        || i.HasHeir(RelationType.Grandfather);

    public static bool DeceasedHasMaleDescendantsOrAscendants(this InheritanceCase i) =>
        DeceasedHasMaleDescendants(i)
        || DeceasedHasMaleAscendants(i);

    public static bool DeceasedHasMaleHeirs(this InheritanceCase i) =>
        DeceasedHasMaleDescendantsOrAscendants(i)
        || i.HasHeir(RelationType.FullBrother)
        || i.HasHeir(RelationType.ConsanguineBrother);

    public static bool DeceasedHasSiblings(this InheritanceCase i) =>
        i.HasHeir(RelationType.FullBrother)
        || i.HasHeir(RelationType.FullSister)
        || i.HasHeir(RelationType.ConsanguineBrother)
        || i.HasHeir(RelationType.ConsanguineSister)
        || i.HasHeir(RelationType.UterineBrother)
        || i.HasHeir(RelationType.UterineSister);

    public static bool DeceasedHasSpouse(this InheritanceCase i) =>
        (i.Deceased.Gender == GenderType.Male && i.HasHeir(RelationType.Wife))
        || (i.Deceased.Gender == GenderType.Female && i.HasHeir(RelationType.Husband));
}
