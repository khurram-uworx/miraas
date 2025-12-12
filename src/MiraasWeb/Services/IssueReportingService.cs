namespace MiraasWeb.Services;

using System.Text.Json;
using MiraasWeb.Models;

public class IssueReportingService
{
    readonly string issuesDirectory;

    public IssueReportingService(IWebHostEnvironment environment)
    {
        this.issuesDirectory = Path.Combine(environment.ContentRootPath, "App_Data", "Issues");
        
        // Ensure the directory exists
        Directory.CreateDirectory(issuesDirectory);
    }

    int getNextIssueNumber() =>
        Directory.GetFiles(issuesDirectory, "*.txt")
        .Select(Path.GetFileNameWithoutExtension)
        .Where(name =>
            !string.IsNullOrEmpty(name) &&
            name.Length >= 4 &&
            int.TryParse(name.Substring(0, 4), out _)
        )
        .Select(name => int.Parse(name.Substring(0, 4)))
        .DefaultIfEmpty(0)
        .Max() + 1;

    string buildReportContent(int issueNumber, CalculationRequestDto request, string userComment)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        var content = $"""
            ISSUE REPORT #{issueNumber:D4}
            ================================
            Reported: {timestamp}
            
            USER COMMENT:
            {userComment}
            
            CALCULATION REQUEST DATA:
            ================================
            Deceased Gender: {(request.DeceasedGender == 0 ? "Male" : "Female")}
            Estate Value: {(request.EstateValue?.ToString("C") ?? "Not specified")}
            
            HEIRS:
            """;

        if (request.Heirs?.Any() == true)
        {
            foreach (var heir in request.Heirs)
            {
                content += $"\n- {heir.Key}: {heir.Value}";
            }
        }
        else
        {
            content += "\n(No heirs specified)";
        }

        content += $"""
            
            
            RAW JSON DATA:
            ================================
            {JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true })}
            """;

        return content;
    }

    public async Task<int> ReportIssueAsync(CalculationRequestDto calculationRequest, string userComment)
    {
        var issueNumber = getNextIssueNumber();
        var fileName = $"{issueNumber:D4}.txt";
        var filePath = Path.Combine(issuesDirectory, fileName);

        var reportContent = buildReportContent(issueNumber, calculationRequest, userComment);

        await File.WriteAllTextAsync(filePath, reportContent);

        return issueNumber;
    }
}