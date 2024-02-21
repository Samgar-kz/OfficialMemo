using OfficialMemo.Models.Dto;

namespace OfficialMemo.Models.ProcessModels.Dto;

public class ProcessMessageDto
{
    public string Id { get; set; } = string.Empty;
    public Guid ProcessGuid { get; set; }
    public Guid? MessageGuid { get; set; }
    public Guid? RequestGuid { get; set; }

    public Guid? ParentGuid { get; set; }
    public Guid? TaskGuid { get; set; }
    public Guid? PreviousTaskGuid { get; set; }
        
    public int ChildCount { get; set; }

    public string InitiatorCode { get; set; } = string.Empty;
    public string InitiatorName { get; set; } = string.Empty;
    public DateTime MessageDate { get; set; }
    public string? MessageType { get; set; }
    public string? StepName { get; set; }

    public string ExecutorCode { get; set; } = string.Empty;
    public string ExecutorName { get; set; } = string.Empty;

    public string? UserCode { get; set; }
    public string? UserName { get; set; }
    public DateTime? ReplyDate { get; set; }
    public ReplyDto Reply { get; set; } = new ReplyDto();

    public bool? ApprovalRequired { get; set; }
    public bool? RegistrarRequired { get; set; }
    public bool? Approved { get; set; }
    public string? ApproverCode { get; set; }
    public string? ApproverName { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public ReplyDto? Approval { get; set; }

    public List<ProcessMessageDto> Children { get; set; } = new List<ProcessMessageDto>();

    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessVersion { get; set; } = string.Empty;
    public string ProcessStatus { get; set; } = string.Empty;
    public string ResponseRequired { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public DateTime MessageCreated { get; set; }
    public DateTime? MessageResponded { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string DepartamentName { get; set; } = string.Empty;
    public string RegNum { get; set; } = string.Empty;
    public string Step { get; set; } = string.Empty;
    public string MessageComment { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string? Decn { get; set; }//
    public string Summary { get; set; } = string.Empty;
    public string ConfidenceType { get; set; } = string.Empty;
    public OfficialMemoModel OfficialMemo { get; set; } = new OfficialMemoModel();  
    public DateTime? DueToDate { get; set; }
    public string? Data { get; set; }
}