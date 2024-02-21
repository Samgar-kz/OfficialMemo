namespace OfficialMemo.Models.Dto;

public class MessageDto
{
    public Guid? TaskGuid { get; set; }
    public Guid? TaskParentGuid { get; set; }
    public int TaskChildCount { get; set; }
    public string TaskType { get; set; } = null!;
    public string TaskStatus { get; set; } = null!;

    public Guid RequestGuid { get; set; }
    public Guid MessageGuid { get; set; }
    public Guid ProcessGuid { get; set; }

    public string ExecutorCode { get; set; } = null!;
    public string? ExecutorName { get; set; }
    public string UserCode { get; set; } = null!;
    public string? UserName { get; set; }
    
    public string MessageType { get; set; } = null!;
    public string Step { get; set; } = null!;

    public bool? ResponseRequired { get; set; }
    public DateTime MessageDate { get; set; }
    public DateTime StartDate { get; set; }
    public string? RegNum { get; set; }
    public string DocumentType { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string MessageName { get; set; } = null!;
    public string? ConfidenceType { get; set; }

    public string? InitiatorCode { get; set; } = null!;
    public string? InitiatorName { get; set; } = null!;

    public DateTime? DueToDate { get; set; }
    public bool StrictDeadline { get; set; }
    public string? OutMessageStep { get; set; }
    public string? OutMessageScheme { get; set; }
}



public enum OutMessageStep
{
    Completed = -90,
    OutMessageReview = -70,
    OutgoingMessageSigning =- 50,
    OutgoingMessageApprove = -30,
    OutgoingMessageRework = -101,
    WithoutOutMessages = -100
}