namespace OfficialMemo.Models.Poco;

public record ProcessInfo
{
    public Guid ProcessGuid { get; set; }
    public Guid RequestGuid { get; set; }
    public Guid MessageGuid { get; set; }
    public string SchemeId { get; set; } = string.Empty;
    public string ProcessStatus { get; set; } = string.Empty;
}