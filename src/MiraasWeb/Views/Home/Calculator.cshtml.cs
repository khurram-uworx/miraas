namespace MiraasWeb.Views.Home;

using Microsoft.AspNetCore.Mvc.RazorPages;
using MiraasWeb.Models;

/// <summary>
/// Razor Page code-behind for the Islamic Inheritance Calculator.
/// Handles page initialization and model binding.
/// </summary>
public class IndexModel : PageModel
{
    public CalculatorViewModel Calculator { get; set; } = new();

    public void OnGet()
    {
        // Initialize with default values
        Calculator = new CalculatorViewModel();
    }
}
