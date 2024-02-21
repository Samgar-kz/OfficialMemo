using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models.Dto;

public record ReceivingResultMessageDto
{
    public DateTime Created { get; set; }
    public EmployeeDto? Executor { get; set; }
    public string? Result { get; set; }
    public string? Comment { get; set; }
    public Document[]? Documents { get; set; }
}