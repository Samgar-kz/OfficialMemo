using Microsoft.EntityFrameworkCore;
using OfficialMemo.Models.Dto;

namespace OfficialMemo.Models.Dbo;
[Keyless]
public class ProcessReportDbo : ProcessReportDto
{
    //public string? InitiatorCode { get; set; }
    public bool IsSigned { get; set; }
    public bool IsApproved { get; set; }
}

public class UserProcessReportDbo : ProcessReportDbo
{
    //public string? InitiatorCode { get; set; }
    public string? UserCode { get; set; }
}
