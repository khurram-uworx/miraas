namespace MiraasWeb.Domain.Dtos;

/// <summary>
/// DTO representing a single heir's calculated share.
/// </summary>
public class HeirShareDto
{
    /// <summary>
    /// The relation type of this heir (e.g., "Son", "Wife").
    /// </summary>
    public string Relation { get; set; } = string.Empty;

    /// <summary>
    /// Number of heirs of this type.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// The share as a fraction string (e.g., "1/4", "2/3").
    /// </summary>
    public string Fraction { get; set; } = string.Empty;

    /// <summary>
    /// The share as a percentage (e.g., 25 for 25%).
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>
    /// The monetary amount if estate value was provided.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Human-readable explanation of this heir's share.
    /// </summary>
    public string Explanation { get; set; } = string.Empty;
}
