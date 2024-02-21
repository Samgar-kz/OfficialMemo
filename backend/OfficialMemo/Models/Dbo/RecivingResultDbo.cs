using OfficialMemo.Models.Poco;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OfficialMemo.Models.Dbo;

public class ReceivingResultDbo
{
    public ReceivingResultDbo() {    
    }
    public ReceivingResultDbo(Guid messageId, string result)
    {
        MessageId = messageId;
        Result = result;
    }
    
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    
    public DateTime Created { get; set; }
    public EmployeeDbo? Receiver { get; set; }
    public EmployeeDbo? Executor { get; set; }

    public string ReceiverCode { get; set; } = null!;
    public string ExecutorCode { get; set; } = null!;
    public string Result { get; set; }
    public string? Comment { get; set; }
    public bool IsRoot { get; set; }
    public Document[]? Documents { get; set; }
}

internal class ReceivingResultDboConfiguration : IEntityTypeConfiguration<ReceivingResultDbo>
{
    public void Configure(EntityTypeBuilder<ReceivingResultDbo> builder)
    {
        builder.ToTable("ReceivingResults", "OffMemo");
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Receiver).WithMany().HasForeignKey(e => e.ReceiverCode);
        builder.HasOne(e => e.Executor).WithMany().HasForeignKey(e => e.ExecutorCode);
        
    }
}