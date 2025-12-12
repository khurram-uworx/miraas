using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MiraasWeb.Models;

/// <summary>
/// Razor Page code-behind for the Islamic Inheritance Calculator.
/// Handles page initialization and model binding.
/// </summary>
public class IndexViewModel : PageModel
{
    public CalculatorViewModel Calculator { get; set; } = new();

    public void OnGet()
    {
        // Initialize with default values
        Calculator = new CalculatorViewModel();
    }
}
