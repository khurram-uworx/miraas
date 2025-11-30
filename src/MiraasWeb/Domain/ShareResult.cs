using MiraasWeb.Abstractions;

namespace MiraasWeb.Domain;

/// <summary>
/// Represents the calculated share result for an heir.
/// </summary>
public class ShareResult
{
    /// <summary>
    /// The share as a fraction (e.g., 1/4, 1/2, etc.).
    /// </summary>
    public Fraction Fraction { get; set; }

    /// <summary>
    /// The share as a percentage of the estate.
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>
    /// The actual monetary amount (if estate value is provided).
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Human-readable explanation of why this share was assigned.
    /// </summary>
    public string Explanation { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this heir is blocked from inheritance.
    /// </summary>
    public bool IsBlocked { get; set; }

    public ShareResult(Fraction fraction)
    {
        Fraction = fraction;
        Percentage = fraction.ToPercentage();
    }

    public ShareResult() { }
}
