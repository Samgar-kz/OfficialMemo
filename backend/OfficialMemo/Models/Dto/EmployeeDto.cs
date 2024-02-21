namespace OfficialMemo.Models.Dto;

public record EmployeeDto
{
    public string Login { get; set; } = null!;
    public string? Code { get; init; }
    public string? Iin { get; init; }
    public string Name { get; init; } = null!;
    public string? Email { get; init; }
    public string? Phones { get; init; }
    public string? LocalPhone { get; init; }
    public string? WorkStatus { get; init; }

    public string? PositionKz { get; set; }
    public string? PositionRu { get; set; }
    public string? PositionEn { get; set; }
    
    public string? DepartmentCode { get; init; }
}

public record EmployeeShortDto
{
    public string Login { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Position { get; set; }
    public string? Phones { get; set; }
}
