using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models.ProcessModels.Poco;

public record ProcessMessage
{
    public Guid Id { get; set; }
    public Guid ProcessGuid { get; set; }
    public Guid? MessageGuid { get; set; }
    public Guid? RequestGuid { get; set; }

    public Guid? ParentGuid { get; set; }
    public Guid? TaskGuid { get; set; }
    public Guid? PreviousTaskGuid { get; set; }

    public string? MessageType { get; set; }
    public string? StepName { get; set; }
    public int ChildCount { get; set; }

    public DateTime MessageDate { get; set; }
    public string? MessageComment { get; set; }
    public string? MessageStatus { get; set; }
    public string? MessageStatusName { get; set; }
    public Document[]? MessageDocuments { get; set; }
    public string? InitiatorCode { get; set; }
    public string? InitiatorName { get; set; }


    public string? ExecutorCode { get; set; }
    public string? ExecutorName { get; set; }


    public DateTime? ReplyDate { get; set; }
    public bool ResponseRecieved { get; set; }
    public string? UserCode { get; set; }
    public string? UserName { get; set; }

    public string? ReplyDecision { get; set; }
    public string? ReplyDecisionName { get; set; }
    public string? ReplyComment { get; set; }
    public Document[]? ReplyDocuments { get; set; }

    public bool? ApprovalRequired { get; set; }
    public bool? RegistrarRequired { get; set; }
    public bool? Approved { get; set; }
    public string? ApproverCode { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovalDate { get; set; }

    public string? ApprovalDecision { get; set; }
    public string? ApprovalDecisionName { get; set; }
    public string? ApprovalComment { get; set; }
    public Document[]? ApprovalDocuments { get; set; }
    public List<ProcessMessage>? Children { get; set; }
    public string? Data { get; set; }
    public DateTimeOffset? DueToDate { get; set; }
    public string? RegNum { get; set; }
}