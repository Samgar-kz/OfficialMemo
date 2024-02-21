namespace OfficialMemo.Models.Dbo;

public record MessageDbo
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
    public string? ExecutorName { get; set; } = null;
    public string UserCode { get; set; } = null!;
    public string? UserName { get; set; }
    
    public string MessageType { get; set; } = null!;
    public string SchemaName { get; set; } = null!;

    public bool? ResponseRequired { get; set; }
    public DateTime MessageDate { get; set; }
    public DateTime StartDate { get; set; }
    public string? RegNum { get; set; }
    public string DocumentType { get; set; } = null!;
    public string? Summary { get; set; }
    public string MessageName { get; set; } = null!;
    public string? ConfidenceType { get; set; }

    public string? InitiatorCode { get; set; } = null!;
    public string? InitiatorName { get; set; } = null!;

    public DateTimeOffset? DueToDate { get; set; }
    public bool StrictDeadline { get; set; }
    public string? OutMessageStep { get; set; }
    public string? OutMessageScheme { get; set; }

}
