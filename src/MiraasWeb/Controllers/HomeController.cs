using Microsoft.AspNetCore.Mvc;
using MiraasWeb.Models;
using MiraasWeb.Services;
using System.Diagnostics;

namespace MiraasWeb.Controllers;

public class HomeController : Controller
{
    readonly CalculatorService calculatorService;
    readonly IssueReportingService issueReportingService;

    public HomeController(CalculatorService calculatorService, IssueReportingService issueReportingService)
    {
        this.calculatorService = calculatorService;
        this.issueReportingService = issueReportingService;
    }

    public IActionResult Index()
    {
        this.ViewData["Title"] = "Islamic Inheritance Calculator";
        return View(new IndexViewModel());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Rules()
    {
        this.ViewData["Title"] = "Inheritance Rules";
        return View();
    }

    [HttpPost]
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

    [HttpPost]
    public async Task<IActionResult> ReportIssue([FromBody] IssueReportDto report)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Invalid report format." });
        }

        try
        {
            var issueNumber = await issueReportingService.ReportIssueAsync(report.CalculationRequest, report.UserComment);
            
            return Ok(new 
            { 
                success = true, 
                issueNumber,
                message = $"Issue reported successfully. Reference number: {issueNumber:D4}"
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "Failed to report issue. Please try again." });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
