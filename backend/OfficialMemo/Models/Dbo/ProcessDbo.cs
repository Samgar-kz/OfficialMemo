namespace OfficialMemo.Models.Dbo;

public class ProcessDbo
{
    public Guid ProcessGuid { get; set; }
    public DateTime StartDate { get; set; }
    public string? RegNum { get; set; }
    public string? DocumentType { get; set; }
    public string? Summary { get; set; }
    public string? ConfidenceType { get; set; }
    public string? ProcessStatus { get; set; }
    public string? CurrentStep { get; set; }
    public string? InitiatorCode { get; set; }
    public string? InitiatorName { get; set; }
    public DateTime? FinishDate { get; set; }
    public string? Status { get; set; }
    public string? UserCode { get; set; }
}