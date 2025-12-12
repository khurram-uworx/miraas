namespace MiraasWeb.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO for reporting calculation issues.
/// </summary>
public class IssueReportDto
{
    /// <summary>
    /// The original calculation request that had issues.
    /// </summary>
    [Required]
    public CalculationRequestDto CalculationRequest { get; set; } = new();

    /// <summary>
    /// User's description of the problem or expected behavior.
    /// </summary>
    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string UserComment { get; set; } = string.Empty;
}