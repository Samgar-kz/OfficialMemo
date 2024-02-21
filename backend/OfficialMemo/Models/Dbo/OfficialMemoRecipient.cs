using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OfficialMemo.Models.Dbo;

public class OfficialMemoRecipient
{
    public int Order { get; set; }

    public EmployeeDbo Recipient { get; set; } = new EmployeeDbo();
    public OfficialMemoDbo OfficialMemo { get; set; } = new OfficialMemoDbo();

    public string RecipientsLogin { get; set; } = string.Empty;
    public Guid OfficialMemoDboMessageGuid { get; set; }
}

internal class OfficialMemoRecipientConfiguration : IEntityTypeConfiguration<OfficialMemoRecipient>
{
    public void Configure(EntityTypeBuilder<OfficialMemoRecipient> builder)
    {
        builder.ToTable("RecipientDboOfficialMemoDbo", "OffMemo");
        builder.HasKey(dbo => new { dbo.Order, dbo.RecipientsLogin, dbo.OfficialMemoDboMessageGuid });
        builder.HasOne(dbo => dbo.Recipient).WithMany().HasForeignKey(dbo => dbo.RecipientsLogin);
        builder.HasOne(dbo => dbo.OfficialMemo)
            .WithMany(dbo => dbo.OfficialMemoRecipients)
            .HasForeignKey(dbo => dbo.OfficialMemoDboMessageGuid);
        //builder.Ignore(dbo => dbo.Recipient);
        //builder.Ignore(dbo => dbo.OfficialMemo);
    }
}
