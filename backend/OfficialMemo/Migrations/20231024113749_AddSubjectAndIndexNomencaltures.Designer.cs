﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OfficialMemo.Context;

#nullable disable

namespace OfficialMemo.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20231024113749_AddSubjectAndIndexNomencaltures")]
    partial class AddSubjectAndIndexNomencaltures
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("OffMemo")
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OfficialMemo.Models.Dbo.ApprovalResultDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApproverCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Documents")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Result")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApproverCode");

                    b.HasIndex("MessageId");

                    b.ToTable("ApprovalResults", "OffMemo");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.ConfidenceTypeDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DisplayTextKz")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayTextRu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ConfidenceTypes", "DocEx");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.EmployeeDbo", b =>
                {
                    b.Property<string>("Login")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BranchCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("DepartmentCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Iin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsStaff")
                        .HasColumnType("bit");

                    b.Property<string>("LocalPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentDepartmentCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phones")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PositionEn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PositionKz")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PositionRu")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Login");

                    b.ToTable((string)null);

                    b.ToView("v_Employees", "DocEx");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.EmployeePositionsDbo", b =>
                {
                    b.Property<string>("UserCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("En")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PositionEn");

                    b.Property<string>("Kz")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PositionKz");

                    b.Property<string>("LocalPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ru")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PositionRu");

                    b.HasKey("UserCode");

                    b.ToTable("EmployeePositions", "DocEx");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.IndexNomenclatureDbo", b =>
                {
                    b.Property<string>("Index")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Index");

                    b.ToTable("IndexNomenclatures", "OffMemo");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.OfficialMemoApprover", b =>
                {
                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("ApproversLogin")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("OfficialMemoDboMessageGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Order", "ApproversLogin", "OfficialMemoDboMessageGuid");

                    b.HasIndex("ApproversLogin");

                    b.HasIndex("OfficialMemoDboMessageGuid");

                    b.ToTable("EmployeeDboOfficialMemoDbo", "OffMemo");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.OfficialMemoDbo", b =>
                {
                    b.Property<Guid>("MessageGuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AdditionalData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ApprovalRequired")
                        .HasColumnType("bit");

                    b.Property<string>("ApproveType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Attachments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ConfidenceTypeId")
                        .HasColumnType("int");

                    b.Property<string>("DocumentUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DueToDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExecutorCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IndexNomenclature")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("MessageDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("OfficialMemoRegNum")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OriginalDocumentUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Recipients")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegNum")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegisterCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RegistrarCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SignDataMessageGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SignerCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VerticalText")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MessageGuid");

                    b.HasIndex("ConfidenceTypeId");

                    b.HasIndex("ExecutorCode");

                    b.HasIndex("SignDataMessageGuid");

                    b.HasIndex("SignerCode");

                    b.ToTable("OfficialMemo", "OffMemo");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.OfficialMemoProcessDataDbo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Actual")
                        .HasColumnType("bit");

                    b.Property<string>("BranchId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BranchName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("DepId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DepName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExecutorCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FinishDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("FINISH_DATE");

                    b.Property<string>("InitiatorCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InitiatorName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("MessageGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OriginRequestGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OutRegNum")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProcessCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PROCESS_CODE");

                    b.Property<Guid>("ProcessGuid")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("PROCESS_GUID");

                    b.Property<string>("ProcessStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PROCESS_STATUS");

                    b.Property<decimal>("ProcessVersion")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("PROCESS_VERSION");

                    b.Property<string>("RegNum")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("START_DATE");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MessageGuid")
                        .IsUnique();

                    b.ToTable("OfficialMemoProcessData", "OffMemo");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.SignMessageDbo", b =>
                {
                    b.Property<Guid>("MessageGuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegisterSignature")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegisterSignedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegisterSignedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SignDocumentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SignType")
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Signature")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SignatureLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SignedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("SignedTime")
                        .HasColumnType("datetime2");

                    b.HasKey("MessageGuid");

                    b.HasIndex("SignedBy");

                    b.ToTable("SignedData", "OffMemo");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.ApprovalResultDbo", b =>
                {
                    b.HasOne("OfficialMemo.Models.Dbo.EmployeeDbo", "Approver")
                        .WithMany()
                        .HasForeignKey("ApproverCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OfficialMemo.Models.Dbo.OfficialMemoDbo", null)
                        .WithMany("ApprovalResults")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Approver");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.EmployeePositionsDbo", b =>
                {
                    b.HasOne("OfficialMemo.Models.Dbo.EmployeeDbo", "Employee")
                        .WithOne()
                        .HasForeignKey("OfficialMemo.Models.Dbo.EmployeePositionsDbo", "UserCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.OfficialMemoApprover", b =>
                {
                    b.HasOne("OfficialMemo.Models.Dbo.EmployeeDbo", "Approver")
                        .WithMany()
                        .HasForeignKey("ApproversLogin")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OfficialMemo.Models.Dbo.OfficialMemoDbo", "OfficialMemo")
                        .WithMany("OfficialMemoApprovers")
                        .HasForeignKey("OfficialMemoDboMessageGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Approver");

                    b.Navigation("OfficialMemo");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.OfficialMemoDbo", b =>
                {
                    b.HasOne("OfficialMemo.Models.Dbo.ConfidenceTypeDbo", "ConfidenceType")
                        .WithMany()
                        .HasForeignKey("ConfidenceTypeId");

                    b.HasOne("OfficialMemo.Models.Dbo.EmployeeDbo", "Executor")
                        .WithMany()
                        .HasForeignKey("ExecutorCode");

                    b.HasOne("OfficialMemo.Models.Dbo.SignMessageDbo", "SignData")
                        .WithMany()
                        .HasForeignKey("SignDataMessageGuid");

                    b.HasOne("OfficialMemo.Models.Dbo.EmployeeDbo", "Signer")
                        .WithMany()
                        .HasForeignKey("SignerCode");

                    b.Navigation("ConfidenceType");

                    b.Navigation("Executor");

                    b.Navigation("SignData");

                    b.Navigation("Signer");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.OfficialMemoProcessDataDbo", b =>
                {
                    b.HasOne("OfficialMemo.Models.Dbo.OfficialMemoDbo", "OfficialMemo")
                        .WithOne("ProcessData")
                        .HasForeignKey("OfficialMemo.Models.Dbo.OfficialMemoProcessDataDbo", "MessageGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OfficialMemo");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.SignMessageDbo", b =>
                {
                    b.HasOne("OfficialMemo.Models.Dbo.EmployeeDbo", "Signer")
                        .WithMany()
                        .HasForeignKey("SignedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Signer");
                });

            modelBuilder.Entity("OfficialMemo.Models.Dbo.OfficialMemoDbo", b =>
                {
                    b.Navigation("ApprovalResults");

                    b.Navigation("OfficialMemoApprovers");

                    b.Navigation("ProcessData")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}