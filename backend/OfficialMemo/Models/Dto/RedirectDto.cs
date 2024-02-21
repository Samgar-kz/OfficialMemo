namespace OfficialMemo.Models.Dto;

public record RedirectDto : ReplyDto
{
    public EmployeeShortDto RedirectTo { get; set; } = null!;
}