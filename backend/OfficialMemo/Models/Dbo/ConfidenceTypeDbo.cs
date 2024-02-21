using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OfficialMemo.Models.Dbo;

public record ConfidenceTypeDbo
{
    public int Id { get; set; }
    public string DisplayTextKz { get; set; } = null!;
    public string DisplayTextRu { get; set; } = null!;
}

internal class ConfidenceTypeDboConfiguration : IEntityTypeConfiguration<ConfidenceTypeDbo>
{
    public void Configure(EntityTypeBuilder<ConfidenceTypeDbo> builder)
    {
        builder.ToTable("ConfidenceTypes", "DocEx");
        builder.HasKey(p => p.Id);
    }
}
