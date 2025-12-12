namespace MiraasWeb.Models;

/// <summary>
/// Request DTO for inheritance calculation API.
/// Contains the deceased's gender and all heirs information.
/// </summary>
public class CalculationRequestDto
{
    /// <summary>
    /// Gender of the deceased (0 = Male, 1 = Female).
    /// </summary>
    public int DeceasedGender { get; set; }

    /// <summary>
    /// Optional estate value for monetary calculations.
    /// </summary>
    public decimal? EstateValue { get; set; }

    /// <summary>
    /// Dictionary of heir relation type to count.
    /// Key: RelationType enum value as string
    /// Value: number of heirs of that type
    /// </summary>
    public Dictionary<string, int> Heirs { get; set; } = new();
}
