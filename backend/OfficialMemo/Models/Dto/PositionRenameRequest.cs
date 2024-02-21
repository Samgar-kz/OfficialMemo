namespace OfficialMemo.Models.Dto;

public record PositionRenameRequest
{
    public string Login { get; set; } = null!;
    public string? PositionKz { get; set; }
    public string? PositionRu { get; set; } 
    public string? PositionEn { get; set; }
    public string? LocalPhone { get; set; }
}