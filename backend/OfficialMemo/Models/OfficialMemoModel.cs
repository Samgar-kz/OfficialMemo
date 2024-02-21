using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models;

public class OfficialMemoCoreData
{
    public string? RegNum { get; set; }
    public string? RegistrarCode { get; set; }
    public string Id { get; set; } = string.Empty;
    public Guid? MessageGuid { get; set; }
    public Employee? Executor { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public ConfidenceType? ConfidenceType { get; set; }
    public string? IndexNomenclature { get; set; }
    public int? AmountPage { get; set; }
    public string? VerticalText { get; set; }
    public List<Employee>? Approvers { get; set; }
    public List<Employee> Recipients { get; set; } = new List<Employee>();
    public string? ApprovalMode { get; set; }
    public Employee? Responsible { get; set; }
    public Employee? Signer { get; set; }
    public string Summary { get; set; } = string.Empty;
    public List<Document>? Attachments { get; set; }

    public string? Data { get; set; }
    public bool ApprovalRequired{get;set;}
    public bool RegistrarRequired{get;set;}
    public string RegisterCode { get; set; } = string.Empty;
    public DateTime? RegisterDate { get; set; }
    public DateTime DueToDate { get; set; }
    public string? Details { get; set; }
    public DateTime MessageDate { get; set; }
    public SignMessage? SignData { get; set; }
}

public record ConfidenceType
{
    public int Id { get; set; }
    public string DisplayTextKz { get; set; } = null!;
    public string DisplayTextRu { get; set; } = null!;
}

public record OfficialMemoModel
{
    public OfficialMemoCoreData Data { get; set; } = null!;
    public string? DocumentUrl { get; set; }
    public string? OriginalDocumentUrl { get; set; }
}