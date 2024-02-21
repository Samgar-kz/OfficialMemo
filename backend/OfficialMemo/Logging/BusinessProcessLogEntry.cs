using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OfficialMemo.Logging;

public record BusinessProcessLogEntry
{
    public int Id { get; set; }
    public Guid DocumentId { get; set; }
    public string LogLevel { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string EventName{ get; set; } = null!;
    public string Category { get; set; } = null!;
    public string ProcessName { get; set; } = null!;
    public string UserCode { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    
    public string? AdditionalData { get; set; } 
}

public class BusinessProcessLogEntryConfiguration : IEntityTypeConfiguration<BusinessProcessLogEntry>
{
    public void Configure(EntityTypeBuilder<BusinessProcessLogEntry> builder)
    {
        builder.ToTable("LogEntries", "OffMemo");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.ProcessName);
    }
}