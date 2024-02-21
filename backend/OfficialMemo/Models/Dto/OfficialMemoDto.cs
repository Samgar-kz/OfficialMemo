namespace OfficialMemo.Models.Dto;

public record OfficialMemoDto
{
    public OfficialMemoCoreDataDto Data { get; set; } = null!;
    public string? DocumentUrl { get; set; }
    public string? OriginalDocumentUrl { get; set; }

    public Guid? ProcessGuid { get; set; }
}