namespace MiraasWeb.Controllers;

/// <summary>
/// Response DTO for inheritance calculation API.
/// Contains the calculation results and any errors/warnings.
/// </summary>
public class CalculationResponseDto
{
    /// <summary>
    /// Indicates if the calculation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if calculation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// List of calculated heir shares.
    /// </summary>
    public List<HeirShareDto> Shares { get; set; } = new();

    /// <summary>
    /// Total of all allocated fractions as a fraction string.
    /// Should be "1/1" in normal cases.
    /// </summary>
    public string TotalFraction { get; set; } = "1/1";

    /// <summary>
    /// Total as a percentage (should be 100 in normal cases).
    /// </summary>
    public decimal TotalPercentage { get; set; }

    /// <summary>
    /// Indicates if the calculation requires scholarly review (e.g., Awl scenario).
    /// </summary>
    public bool RequiresScholarlyReview { get; set; }

    /// <summary>
    /// Any warnings or notes about the calculation.
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}
