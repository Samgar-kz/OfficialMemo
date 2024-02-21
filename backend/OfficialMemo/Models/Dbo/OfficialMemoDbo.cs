using OfficialMemo.Converters;
using OfficialMemo.Models.Poco;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mapster;

namespace OfficialMemo.Models.Dbo;

public record OfficialMemoDbo
{
    public string? RegNum { get; set; }

    public Guid MessageGuid { get; set; }

    public EmployeeDbo? Executor { get; set; }
    public EmployeeDbo? Signer { get; set; }
    public string? ExecutorCode { get; set; } = null!;
    public string? SignerCode { get; set; } = null!;
    public string RegistrarCode { get; set; } = string.Empty;
    public List<ReceivingResultDbo>? ReceivingResults { get; set; }
    public List<EmployeeDbo> Recipients
    {
        get
        {
            return OfficialMemoRecipients.Select(oa => oa.Recipient).ToList();
        }
        set
        {
            OfficialMemoRecipients = value.Select((dbo, i) => new OfficialMemoRecipient
            { Recipient = dbo, RecipientsLogin = dbo.Login, OfficialMemoDboMessageGuid = MessageGuid, OfficialMemo = this, Order = i })
                .ToList();
        }
    }

    public List<OfficialMemoRecipient> OfficialMemoRecipients { get; set; } = null!;
    public string Subject { get; set; } = string.Empty;
    public string Language { get; set; } = null!;
    public int? ConfidenceTypeId { get; set; }
    public ConfidenceTypeDbo? ConfidenceType { get; set; } = null!;
    public string? IndexNomenclature { get; set; }
    public int? AmountPage { get; set; }
    public string? VerticalText { get; set; }
    public string? Summary { get; set; }
    // public string Data { get; set; } = null!;
    public List<Document>? Attachments { get; set; }
    public bool ApprovalRequired { get; set; }
    public bool RegistrarRequired { get; set; }
    public string ApproveType { get; set; } = null!;

    public List<ApprovalResultDbo>? ApprovalResults { get; set; }

    public List<EmployeeDbo>? Approvers
    {
        get
        {
            return OfficialMemoApprovers?.Select(oa => oa.Approver).ToList();
        }
        set
        {
            OfficialMemoApprovers = value?.Select((dbo, i) => new OfficialMemoApprover
            { Approver = dbo, ApproversLogin = dbo.Login, OfficialMemoDboMessageGuid = MessageGuid, OfficialMemo = this, Order = i })
                .ToList();
        }
    }

    public List<OfficialMemoApprover>? OfficialMemoApprovers { get; set; }
    public string? RegisterCode { get; set; }
    public DateTime? RegisterDate { get; set; }
    public DateTime? DueToDate { get; set; }
    public OfficialMemoProcessDataDbo ProcessData { get; set; } = null!;
    public DateTime? MessageDate { get; set; }

    public string? DocumentUrl { get; set; }
    public string? OriginalDocumentUrl { get; set; }
    public SignMessageDbo? SignData { get; set; }
    public bool IsBookmark { get; set; }
}

internal class OutMessageDboConfiguration : IEntityTypeConfiguration<OfficialMemoDbo>
{
    public void Configure(EntityTypeBuilder<OfficialMemoDbo> builder)
    {
        builder.ToTable("OfficialMemo", "OffMemo");
        builder.HasKey(dbo => dbo.MessageGuid);
        builder.HasOne(dbo => dbo.Executor).WithMany().HasForeignKey(dbo => dbo.ExecutorCode);
        builder.HasOne(dbo => dbo.Signer).WithMany().HasForeignKey(dbo => dbo.SignerCode);

        builder.Property(dbo => dbo.Attachments).HasConversion<EfJsonConverter<List<Document>>>();
        builder.HasMany(p => p.ApprovalResults).WithOne().HasForeignKey(dbo => dbo.MessageId).IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);        
        builder.HasMany(p => p.ReceivingResults).WithOne().HasForeignKey(dbo => dbo.MessageId).IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(dbo => dbo.ConfidenceType).WithMany().HasForeignKey(dbo => dbo.ConfidenceTypeId);

        builder.Ignore(dbo => dbo.Approvers);
        builder.Ignore(dbo => dbo.Recipients);
    }
}
