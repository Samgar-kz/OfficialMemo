using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OfficialMemo.Models.Dbo;

public record IndexNomenclatureDbo
{
    public string Department { get; set; } = null!;
    public string Index { get; set; } = null!;
    public string Name { get; set; } = null!;
}

internal class IndexNomenclatureDboConfiguration : IEntityTypeConfiguration<IndexNomenclatureDbo>
{
    public void Configure(EntityTypeBuilder<IndexNomenclatureDbo> builder)
    {
        builder.ToTable("IndexNomenclatures", "OffMemo");
        builder.HasKey(p => p.Index);
    }
}
