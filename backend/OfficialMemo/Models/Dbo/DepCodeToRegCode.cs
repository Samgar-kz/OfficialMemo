namespace OfficialMemo.Models.Dbo;

public record DepCodeToRegCode
{
    public string Code { get; set; }
    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }
}