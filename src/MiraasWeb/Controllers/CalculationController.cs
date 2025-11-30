namespace MiraasWeb.Controllers;

using Microsoft.AspNetCore.Mvc;
using MiraasWeb.Services;

/// <summary>
/// API controller for Islamic inheritance calculations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CalculationController : ControllerBase
{
    private readonly CalculatorService calculatorService;

    public CalculationController(CalculatorService calculatorService)
    {
        this.calculatorService = calculatorService;
    }

    /// <summary>
    /// Calculates inheritance shares based on deceased gender and heirs.
    /// </summary>
    /// <param name="request">The calculation request with deceased gender, heirs, and optional estate value.</param>
    /// <returns>The calculation result with shares, percentages, and explanations.</returns>
    /// <remarks>
    /// Request example:
    /// {
    ///   "deceasedGender": 0,
    ///   "estateValue": 100000,
    ///   "heirs": {
    ///     "Wife": 1,
    ///     "Son": 2,
    ///     "Daughter": 1
    ///   }
    /// }
    /// 
    /// Response includes:
    /// - Success status
    /// - Calculated shares (fraction, percentage, amount)
    /// - Human-readable explanations
    /// - Warnings for edge cases
    /// </remarks>
    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] CalculationRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new CalculationResponseDto
            {
                Success = false,
                ErrorMessage = "Invalid request format."
            });
        }

        var result = calculatorService.Calculate(request);
        return Ok(result);
    }
}
