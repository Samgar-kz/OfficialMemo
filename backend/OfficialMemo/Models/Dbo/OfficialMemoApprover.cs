using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OfficialMemo.Models.Dbo;

public class OfficialMemoApprover
{
    public int Order { get; set; }

    public EmployeeDbo Approver { get; set; } = new EmployeeDbo();
    public OfficialMemoDbo OfficialMemo { get; set; } = new OfficialMemoDbo();

    public string ApproversLogin { get; set; } = string.Empty;
    public Guid OfficialMemoDboMessageGuid { get; set; }
}

internal class OfficialMemoApproverConfiguration : IEntityTypeConfiguration<OfficialMemoApprover>
{
    public void Configure(EntityTypeBuilder<OfficialMemoApprover> builder)
    {
        builder.ToTable("EmployeeDboOfficialMemoDbo", "OffMemo");
        builder.HasKey(dbo => new { dbo.Order, dbo.ApproversLogin, dbo.OfficialMemoDboMessageGuid });
        builder.HasOne(dbo => dbo.Approver).WithMany().HasForeignKey(dbo => dbo.ApproversLogin);
        builder.HasOne(dbo => dbo.OfficialMemo)
            .WithMany(dbo => dbo.OfficialMemoApprovers)
            .HasForeignKey(dbo => dbo.OfficialMemoDboMessageGuid);
        //builder.Ignore(dbo => dbo.Approver);
        //builder.Ignore(dbo => dbo.OfficialMemo);
    }
}
