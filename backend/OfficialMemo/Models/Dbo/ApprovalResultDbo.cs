using OfficialMemo.Models.Poco;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OfficialMemo.Models.Dbo;

public class ApprovalResultDbo
{
    public ApprovalResultDbo() {    
    }
    public ApprovalResultDbo(Guid messageId, ApprovalResults result)
    {
        MessageId = messageId;
        Result = result;
    }
    
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    
    public DateTime Created { get; set; }
    public EmployeeDbo? Approver { get; set; }
    public EmployeeDbo? Executor { get; set; }

    public string ApproverCode { get; set; } = null!;
    public string ExecutorCode { get; set; } = null!;
    public ApprovalResults Result { get; set; }
    public string? Comment { get; set; }
    public Document[]? Documents { get; set; }
}

internal class ApprovalResultDboConfiguration : IEntityTypeConfiguration<ApprovalResultDbo>
{
    public void Configure(EntityTypeBuilder<ApprovalResultDbo> builder)
    {
        builder.ToTable("ApprovalResults", "OffMemo");
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Approver).WithMany().HasForeignKey(e => e.ApproverCode);
        builder.HasOne(e => e.Executor).WithMany().HasForeignKey(e => e.ExecutorCode);
        
    }
}