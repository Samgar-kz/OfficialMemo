using OfficialMemo.Models.Dbo;
using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models;

public record SignMessage
{
    public Guid RequestGuid { get; set; }
    public Guid MessageGuid { get; set; }
    public string SignedBy { get; set; }
    public string? Signature { get; set; }
    public string? RegisterSignature { get; set; }
    public string? RegisterSignedBy { get; set; }
    public DateTime? RegisterSignedDate { get; set; }
    public Document? SignDocument { get; set; }

    public string Data { get; set; }
    public DateTime SignedDate { get; set; }
    public SignType? SignType { get; set; }
}