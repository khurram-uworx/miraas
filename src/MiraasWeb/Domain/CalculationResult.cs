using MiraasWeb.Abstractions;

namespace MiraasWeb.Domain;

/// <summary>
/// Result of an inheritance calculation.
/// </summary>
public class CalculationResult
{
    /// <summary>
    /// The heirs with their calculated shares.
    /// </summary>
    public List<Heir> Heirs { get; set; } = new();

    /// <summary>
    /// Total of all allocated fractions (should equal 1 in normal cases).
    /// </summary>
    public Fraction TotalFraction { get; set; }

    /// <summary>
    /// Indicates if the calculation requires scholarly review.
    /// This is true for edge cases or when awl/radd is needed.
    /// </summary>
    public bool RequiresScholarlyReview { get; set; }

    /// <summary>
    /// Any warnings or notes about the calculation.
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Summary explanation of the calculation.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if calculation was successful.
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// Error message if calculation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    public static CalculationResult Success(List<Heir> heirs, Fraction totalFraction)
    {
        return new CalculationResult
        {
            Heirs = heirs,
            TotalFraction = totalFraction,
            IsSuccessful = true
        };
    }

    public static CalculationResult Failure(string errorMessage)
    {
        return new CalculationResult
        {
            IsSuccessful = false,
            ErrorMessage = errorMessage
        };
    }
}
