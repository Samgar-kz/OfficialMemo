using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models.Dto;

public class ApprovalResultDto
{
    public DateTime Created { get; set; }
    public EmployeeDto? Approver { get; set; }
    public EmployeeDto? Executor { get; set; }
    public ApprovalResults Result { get; set; }
    public string? Comment { get; set; }
    public Document[]? Documents { get; set; }
}