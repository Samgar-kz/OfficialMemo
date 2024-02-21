namespace OfficialMemo.Models.Dbo;

public record EmployeePositionsDbo
{
    public string UserCode { get; set; } = null!;
    public string? Kz { get; set; }
    public string? Ru { get; set; }
    public string? En { get; set; }
    public string? LocalPhone { get; set; }

    public EmployeeDbo Employee { get; set; } = null!;
}