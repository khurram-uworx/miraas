namespace MiraasWeb.Domain;

/// <summary>
/// Represents the relationship type of an heir to the deceased.
/// Based on Islamic inheritance law (Ilm al-Fara'id).
/// </summary>
public enum RelationType
{
    // Direct Descendants
    Son,
    Daughter,
    SonOfSon,
    DaughterOfSon,

    // Ascendants
    Father,
    Mother,
    Grandfather,
    GrandmotherMaternal,
    GrandmotherPaternal,

    // Spouse
    Husband,
    Wife,

    // Full Siblings (same father and mother)
    FullBrother,
    FullSister,

    // Consanguine Siblings (same father, different mother)
    ConsanguineBrother,
    ConsanguineSister,

    // Uterine Siblings (same mother, different father)
    UterineBrother,
    UterineSister
}
