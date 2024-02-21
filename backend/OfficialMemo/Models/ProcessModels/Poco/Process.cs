namespace OfficialMemo.Models.ProcessModels.Poco;

public class Process
{
    public string Id { get; set; } = string.Empty;
    public Guid ProcessGuid { get; set; }
    public Guid MessageGuid { get; set; }

    public string ProcessCode { get; set; } = string.Empty;
    public string? ProcessVersion { get; set; }
    public string? ProcessStatus { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public string InitiatorCode { get; set; } = string.Empty;
    public string InitiatorName { get; set; } = string.Empty;

    public string? ExecutorName { get; set; }
    public string? ExecutorCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string? DepartamentName { get; set; }
    public string RegNum { get; set; } = string.Empty;
    public string? Step { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public OfficialMemoModel OutMessage { get; set; } = new OfficialMemoModel();
    public DateTime? DueToDate { get; set; }
    public string ResponseRequired { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string MessageComment { get; set; } = string.Empty;
    public string ConfidenceType { get; set; } = string.Empty;
    public string ApproveType { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public DateTime? MessageCreated { get; set; }

}
