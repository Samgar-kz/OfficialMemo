namespace OfficialMemo.Models.Dto;

public class ProcessReportDto
{
    public Guid ProcessGuid { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public DateTimeOffset? DueToDate { get; set; }
    public string? RegNum { get; set; }
    public string? DocumentType { get; set; }
    public string? ClientName { get; set; }
    public string? Language { get; set; }
    public string? ClientType { get; set; }
    public string? Branch { get; set; }
    public string? BranchCode { get; set; }
    public string? SignerName { get; set; }
    public string? RecipientName { get; set; }
    public string? FeedBackTool { get; set; }
    public string? ExecutorName { get; set; }
    public string? RegisterCode { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string? Summary { get; set; }
    public string? ConfidenceType { get; set; }
    public string? ProcessStatus { get; set; }
    public string? CurrentStep { get; set; }
    public string? InitiatorName { get; set; }
    public string? Status { get; set; }
    public bool? IsBookmark { get; set; }
}