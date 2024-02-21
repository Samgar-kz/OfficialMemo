using OfficialMemo.Models;
using Microsoft.EntityFrameworkCore;
using OfficialMemo.Models.Dbo;
using System.Text.Json.Serialization;
using OfficialMemo.Models.Poco;
using OfficialMemo.Converters;
using System.Reflection;
using OfficialMemo.Models.ProcessModels.Poco;
using OfficialMemo.Logging;

namespace OfficialMemo.Context
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public virtual DbSet<EmployeePositionsDbo> EmployeePositions { get; set; } = null!;
        public virtual DbSet<OfficialMemoDbo> OfficialMemos { get; set; } = null!;
        public virtual DbSet<OfficialMemoProcessDataDbo> OfficialMemoProcessData { get; set; } = null!;
        public virtual DbSet<EmployeeDbo> Employees { get; set; } = null!;
        public virtual DbSet<ConfidenceTypeDbo> ConfidenceTypes { get; set; } = null!;
        public virtual DbSet<IndexNomenclatureDbo> IndexNomenclatures { get; set; } = null!;
        public virtual DbSet<ApprovalResultDbo> ApprovalResults { get; set; }
        public virtual DbSet<ReceivingResultDbo> ReceivingResults { get; set; }
        public virtual DbSet<ProcessMessage> MessageHistories { get; set; } = null!;
        public virtual DbSet<ProcessReportDbo> ProcessReports { get; set; } = null!;
        public virtual DbSet<UserProcessReportDbo> UserProcessReports { get; set; } = null!;
        public DbSet<BusinessProcessLogEntry> BusinessProcessLogEntries { get; set; }
        public virtual DbSet<DepCodeToRegCode> DepCodeToRegCodes { get; set; } = null!;
        public virtual DbSet<OfficialMemoApprover> OfficialMemoApprovers { get; set; } = null!;
        public virtual DbSet<OfficialMemoRecipient> OfficialMemoRecipients { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<Document[]>().HaveConversion<EfJsonConverter<Document[]>>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.HasDefaultSchema("OffMemo");

            modelBuilder.Entity<EmployeeDbo>(e =>
            {
                e.ToView("v_Employees", "OffMemo");
                e.HasKey(p => p.Login);

                e.Property(p => p.Id);
                e.Property(p => p.Code);
                e.Property(p => p.Login);
                e.Property(p => p.Iin);
                e.Property(p => p.Name);
                e.Property(p => p.Email);
                e.Property(p => p.Phones);
                e.Property(p => p.LocalPhone);
                e.Property(p => p.WorkStatus);
                e.Property(p => p.PositionKz);
                e.Property(p => p.PositionRu);
                e.Property(p => p.PositionEn);
                e.Property(p => p.PositionCode);
                e.Property(p => p.IsStaff).HasColumnType("bit");
            });

            modelBuilder.Entity<EmployeePositionsDbo>(e =>
            {
                e.ToTable("EmployeePositions", "OffMemo");
                e.HasKey(dbo => dbo.UserCode);
                e.Property(dbo => dbo.Kz).HasColumnName("PositionKz");
                e.Property(dbo => dbo.Ru).HasColumnName("PositionRu");
                e.Property(dbo => dbo.En).HasColumnName("PositionEn");
                e.Property(p => p.LocalPhone);
                e.HasOne(dbo => dbo.Employee)
                    .WithOne()
                    .HasForeignKey<EmployeePositionsDbo>(p => p.UserCode);
            });


            modelBuilder.Entity<ProcessReportDbo>(e =>
            {
                e.ToView("v_AllOfficialMemos", "OffMemo");
                e.HasKey(e => e.ProcessGuid);
            });
            modelBuilder.Entity<UserProcessReportDbo>(e =>
            {
                e.ToView("v_AllArchiveAndWorkOffMemo", "OffMemo");
                e.HasKey(e => new { e.ProcessGuid, e.UserCode });
            });


            modelBuilder.Entity<ProcessMessage>(e =>
            {
                e.ToView("v_MessageHistory", "OffMemo");
                e.HasKey(m => m.Id);
                e.Ignore(m => m.Children);

                e.Property(m => m.MessageDocuments).HasConversion<EfJsonConverter<Document[]>>();
                e.Property(m => m.ReplyDocuments).HasConversion<EfJsonConverter<Document[]>>();
                e.Property(m => m.ApprovalDocuments).HasConversion<EfJsonConverter<Document[]>>();

                e.Property(m => m.ResponseRecieved).HasColumnType("bit");

                e.Property(m => m.ApprovalDecision).HasColumnName("approvalDec");
                e.Property(m => m.ApprovalDecisionName).HasColumnName("approvalDecn");
            });

            modelBuilder.Entity<DepCodeToRegCode>(e =>
            {
                e.ToTable("DepCodeToRegCodes", "DocEx");
                e.HasKey(p => p.Code);
            });
        }
    }
}