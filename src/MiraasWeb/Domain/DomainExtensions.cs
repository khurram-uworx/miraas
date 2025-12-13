namespace MiraasWeb.Domain;

static class DomainExtensions
{
    public static bool DeceasedHasDescendants(this InheritanceCase i) =>
        i.HasHeir(RelationType.Son)
        || i.HasHeir(RelationType.Daughter)
        || i.HasHeir(RelationType.SonOfSon)
        || i.HasHeir(RelationType.DaughterOfSon);

    public static bool DeceasedHasSiblings(this InheritanceCase i) =>
        i.HasHeir(RelationType.FullBrother)
        || i.HasHeir(RelationType.FullSister)
        || i.HasHeir(RelationType.ConsanguineBrother)
        || i.HasHeir(RelationType.ConsanguineSister)
        || i.HasHeir(RelationType.UterineBrother)
        || i.HasHeir(RelationType.UterineSister);

    public static bool HasSpouse(this InheritanceCase i) =>
        (i.Deceased.Gender == Gender.Male && i.HasHeir(RelationType.Wife))
        || (i.Deceased.Gender == Gender.Female && i.HasHeir(RelationType.Husband));
}
