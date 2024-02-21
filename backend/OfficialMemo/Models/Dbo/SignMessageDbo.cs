using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OfficialMemo.Models.Dbo;

public record SignMessageDbo
{
    public Guid MessageGuid { get; set; }
    public string SignedBy { get; set; }
    public string? Signature { get; set; }
    public string? RegisterSignature { get; set; }
    public string? RegisterSignedBy { get; set; }
    public string? SignatureLink { get; set; }
    public string? SignDocumentName { get; set; }
    public DateTime? RegisterSignedTime { get; set; }
    public string Data { get; set; }
    public DateTime SignedTime { get; set; }
    public EmployeeDbo? Signer { get; set; }
    public SignType? SignType { get; set; }
}

public enum SignType
{
    Digital = 0,
    HandWritten = 1
}

class SignMessageDboConfiguration : IEntityTypeConfiguration<SignMessageDbo>
{
    public void Configure(EntityTypeBuilder<SignMessageDbo> builder)
    {
        builder.ToTable("SignedData", "OffMemo");
        builder.HasKey(e => e.MessageGuid);
        builder.HasOne(e => e.Signer).WithMany().HasForeignKey(e => e.SignedBy);
        builder.Property(e => e.SignType)
            .HasConversion(new EnumToStringConverter<SignType>(new ConverterMappingHints(30, unicode: false)));
    }
}