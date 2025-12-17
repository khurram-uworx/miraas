using Microsoft.AspNetCore.Mvc;

namespace MiraasWeb.Controllers
{
    public class IssuesController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public IssuesController(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var issuesPath = Path.Combine(webHostEnvironment.ContentRootPath, "App_Data", "Issues");
            
            if (!Directory.Exists(issuesPath))
            {
                ViewBag.Message = "Issues directory not found.";
                return View(new List<IssueFileInfo>());
            }

            var issueFiles = Directory.GetFiles(issuesPath, "*.txt")
                .Select(filePath => new IssueFileInfo
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    CreatedDate = System.IO.File.GetCreationTime(filePath),
                    Size = new System.IO.FileInfo(filePath).Length
                })
                .OrderBy(f => f.FileName) // This will order by issue number since files are named 0000.txt, 0001.txt, etc.
                .ToList();

            return View(issueFiles);
        }

        public IActionResult Details(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !fileName.EndsWith(".txt"))
            {
                return BadRequest("Invalid file name.");
            }

            var issuesPath = Path.Combine(webHostEnvironment.ContentRootPath, "App_Data", "Issues");
            var filePath = Path.Combine(issuesPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Security check: ensure the file is within the Issues directory
            var fullIssuesPath = Path.GetFullPath(issuesPath);
            var fullFilePath = Path.GetFullPath(filePath);
            
            if (!fullFilePath.StartsWith(fullIssuesPath))
            {
                return BadRequest("Invalid file path.");
            }

            var content = System.IO.File.ReadAllText(filePath);
            var issueDetail = ParseIssueFile(content, fileName);
            
            return View(issueDetail);
        }

        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !fileName.EndsWith(".txt"))
            {
                return BadRequest("Invalid file name.");
            }

            var issuesPath = Path.Combine(webHostEnvironment.ContentRootPath, "App_Data", "Issues");
            var filePath = Path.Combine(issuesPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Security check: ensure the file is within the Issues directory
            var fullIssuesPath = Path.GetFullPath(issuesPath);
            var fullFilePath = Path.GetFullPath(filePath);
            
            if (!fullFilePath.StartsWith(fullIssuesPath))
            {
                return BadRequest("Invalid file path.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return base.File(fileBytes, "text/plain", fileName);
        }

        private IssueDetail ParseIssueFile(string content, string fileName)
        {
            var lines = content.Split('\n', StringSplitOptions.None);
            var detail = new IssueDetail
            {
                FileName = fileName,
                IssueNumber = Path.GetFileNameWithoutExtension(fileName)
            };

            var section = "";
            var sectionContent = new System.Text.StringBuilder();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith("ISSUE REPORT #"))
                {
                    section = "header";
                    continue;
                }
                else if (trimmedLine.StartsWith("Reported:"))
                {
                    detail.ReportedDate = trimmedLine.Replace("Reported:", "").Trim();
                    continue;
                }
                else if (trimmedLine == "USER COMMENT:")
                {
                    section = "comment";
                    sectionContent.Clear();
                    continue;
                }
                else if (trimmedLine == "CALCULATION REQUEST DATA:")
                {
                    if (section == "comment")
                    {
                        detail.UserComment = sectionContent.ToString().Trim();
                    }
                    section = "calculation";
                    sectionContent.Clear();
                    continue;
                }
                else if (trimmedLine == "RAW JSON DATA:")
                {
                    if (section == "calculation")
                    {
                        detail.CalculationData = sectionContent.ToString().Trim();
                    }
                    section = "json";
                    sectionContent.Clear();
                    continue;
                }
                else if (trimmedLine == "================================")
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(section) && section != "header")
                {
                    sectionContent.AppendLine(line);
                }
            }

            // Capture the last section
            if (section == "json")
            {
                detail.RawJsonData = sectionContent.ToString().Trim();
            }

            return detail;
        }
    }

    public class IssueFileInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public long Size { get; set; }
    }

    public class IssueDetail
    {
        public string FileName { get; set; } = string.Empty;
        public string IssueNumber { get; set; } = string.Empty;
        public string ReportedDate { get; set; } = string.Empty;
        public string UserComment { get; set; } = string.Empty;
        public string CalculationData { get; set; } = string.Empty;
        public string RawJsonData { get; set; } = string.Empty;
    }
}