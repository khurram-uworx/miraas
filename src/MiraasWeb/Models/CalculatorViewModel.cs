namespace MiraasWeb.Models;

/// <summary>
/// View model for the Islamic Inheritance Calculator page.
/// </summary>
public class CalculatorViewModel
{
    /// <summary>
    /// Gender of the deceased (0 = Male, 1 = Female).
    /// </summary>
    public int DeceasedGender { get; set; } = 0;

    /// <summary>
    /// Optional estate value for monetary calculations.
    /// </summary>
    public decimal? EstateValue { get; set; }

    /// <summary>
    /// Number of wives.
    /// </summary>
    public int WifeCount { get; set; } = 0;

    /// <summary>
    /// Number of sons.
    /// </summary>
    public int SonCount { get; set; } = 0;

    /// <summary>
    /// Number of daughters.
    /// </summary>
    public int DaughterCount { get; set; } = 0;

    /// <summary>
    /// Number of fathers (0 or 1).
    /// </summary>
    public bool HasFather { get; set; } = false;

    /// <summary>
    /// Number of mothers (0 or 1).
    /// </summary>
    public bool HasMother { get; set; } = false;

    /// <summary>
    /// Number of grandfathers (0 or 1).
    /// </summary>
    public bool HasGrandfather { get; set; } = false;

    /// <summary>
    /// Number of maternal grandmothers (0 or 1).
    /// </summary>
    public bool HasGrandmotherMaternal { get; set; } = false;

    /// <summary>
    /// Number of paternal grandmothers (0 or 1).
    /// </summary>
    public bool HasGrandmotherPaternal { get; set; } = false;

    /// <summary>
    /// Number of full brothers.
    /// </summary>
    public int FullBrotherCount { get; set; } = 0;

    /// <summary>
    /// Number of full sisters.
    /// </summary>
    public int FullSisterCount { get; set; } = 0;

    /// <summary>
    /// Number of consanguine brothers.
    /// </summary>
    public int ConsanguineBrotherCount { get; set; } = 0;

    /// <summary>
    /// Number of consanguine sisters.
    /// </summary>
    public int ConsanguineSisterCount { get; set; } = 0;

    /// <summary>
    /// Number of uterine brothers.
    /// </summary>
    public int UterineBrotherCount { get; set; } = 0;

    /// <summary>
    /// Number of uterine sisters.
    /// </summary>
    public int UterineSisterCount { get; set; } = 0;

    /// <summary>
    /// Number of sons of sons (grandsons).
    /// </summary>
    public int SonOfSonCount { get; set; } = 0;

    /// <summary>
    /// Number of daughters of sons (granddaughters).
    /// </summary>
    public int DaughterOfSonCount { get; set; } = 0;

    /// <summary>
    /// Number of husbands (0 or 1).
    /// </summary>
    public bool HasHusband { get; set; } = false;
}
