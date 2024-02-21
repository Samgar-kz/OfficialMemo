using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models.Dto;

public record SignMessageDto
{
    public Guid RequestGuid { get; set; }
    public string? Signature { get; set; }
    public Document? SignDocument { get; set; }
    public string? RegisterSignature { get; set; }
    public string Data { get; set; } = string.Empty;
    public DateTime? SignedTime { get; set; }
    public EmployeeDto? Signer { get; set; }
    public string? SignType { get; set; }
}