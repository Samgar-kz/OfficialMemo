using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models.Dto;

public record ReplyDto
{
    public Guid RequestGuid { get; set; }

    public Employee? RepliedBy { get; set; }
    public DateTime? ReplyDate { get; set; }

    public string ReplyDecision { get; set; } = null!;
    public string ReplyDecisionName { get; set; } = null!;
    public string? ReplyComment { get; set; }
    public Document[]? ReplyDocuments { get; set; }
}