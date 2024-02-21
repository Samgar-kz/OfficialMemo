using System.Text.Json.Serialization;
using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models.Dto;

public class ApproveRequest
{
    [JsonPropertyName("guid")]
    public Guid MessageId { get; set; }

    [JsonPropertyName("replyDecision")]
    public string? Decision { get; set; }
    
    [JsonPropertyName("replyDecisionName")]
    public string? DecisionName { get; set; }
    
    [JsonPropertyName("replyComment")]
    public string? Comment { get; set; }
    
    [JsonPropertyName("replyDocuments")]
    public Document[]? Documents { get; set; }
    
    [JsonPropertyName("meta")]
    public string? Meta { get; set; }

    [JsonPropertyName("employeeCode")]
    public string EmployeeCode { get; set; } = null!;

    [JsonPropertyName("executorCode")]

    public string? ExecutorCode { get; set; }

    [JsonPropertyName("isRoot")]
    public bool IsRoot { get; set; }
}