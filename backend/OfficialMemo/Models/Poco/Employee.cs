namespace OfficialMemo.Models.Poco;

public class Employee
{
    public int Id { get; init; } = -1;
    public string Code { get; init; } = null!;
    public string Login { get; init; } = null!;
    public string Iin { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Phones { get; init; } = null!;
    public string LocalPhone { get; init; } = null!;
    public string WorkStatus { get; init; } = null!;

    public string PositionKz { get; set; } = null!;
    public string PositionRu { get; set; } = null!;
    public string PositionEn { get; set; } = null!;

    public string DepartmentCode { get; init; } = null!;
}