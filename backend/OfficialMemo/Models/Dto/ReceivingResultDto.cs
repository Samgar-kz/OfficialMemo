using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models.Dto;

public record ReceivingResultDto
{
    public DateTime? Created { get; set; }
    public EmployeeDto? Receiver { get; set; }
    public EmployeeDto? Executor { get; set; }
    public string? Result { get; set; }
    public List<ReceivingResultDto>? Children { get; set; }
    public int ChildCount { get; set; }
    public string? Comment { get; set; }
    public Document[]? Documents { get; set; }
}