using OfficialMemo.Models.Dbo;
using OfficialMemo.Models.Poco;

namespace OfficialMemo.Models;

public class ApprovalResult
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    
    public DateTime Created { get; set; }
    public EmployeeDbo Approver { get; set; } = null!;
    public string Comment { get; set; } = string.Empty;
    public Document[] Documents { get; set; } = Array.Empty<Document>();    

    public ApprovalResults Result { get; set; }
}