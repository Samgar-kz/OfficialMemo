using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OfficialMemo.Models.Dbo;

public class OfficialMemoProcessDataDbo
{
    public int Id { get; set; }
    public Guid ProcessGuid { get; set; }
    public string ProcessCode { get; set; } = string.Empty;
    public decimal ProcessVersion { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public string ProcessStatus { get; set; } = string.Empty;
    public string RegNum { get; set; } = string.Empty;
    public string BranchId { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string DepId { get; set; } = string.Empty;
    public string DepName { get; set; } = string.Empty;
    public string InitiatorCode { get; set; } = string.Empty;
    public string? InitiatorName { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Actual { get; set; }
    public bool Deleted { get; set; }
    public Guid MessageGuid { get; set; }
    public string? ExecutorCode { get; set; }
    public OfficialMemoDbo OfficialMemo { get; set; } = new OfficialMemoDbo();
}

internal class OfficialMemoProcessDataDboConfiguration : IEntityTypeConfiguration<OfficialMemoProcessDataDbo>
{
    public void Configure(EntityTypeBuilder<OfficialMemoProcessDataDbo> builder)
    {
        builder.ToTable("OfficialMemoProcessData", "OffMemo");
        builder.HasKey(dbo => dbo.Id);
        builder.Property(dbo => dbo.ProcessGuid).HasColumnName("PROCESS_GUID");
        builder.Property(dbo => dbo.ProcessCode).HasColumnName("PROCESS_CODE");
        builder.Property(dbo => dbo.ProcessVersion).HasColumnName("PROCESS_VERSION");
        builder.Property(dbo => dbo.ProcessStatus).HasColumnName("PROCESS_STATUS");
        builder.Property(dbo => dbo.StartDate).HasColumnName("START_DATE");
        builder.Property(dbo => dbo.FinishDate).HasColumnName("FINISH_DATE");
        builder.Property(dbo => dbo.Actual).HasColumnType("bit");
        builder.Property(dbo => dbo.Deleted).HasColumnType("bit");
        builder.HasOne(dbo => dbo.OfficialMemo)
            .WithOne(dbo => dbo.ProcessData)
            .HasForeignKey<OfficialMemoProcessDataDbo>(a => a.MessageGuid)
            .HasPrincipalKey<OfficialMemoDbo>(dbo => dbo.MessageGuid);
    }
}