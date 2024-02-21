using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficialMemo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "OffMemo");

            migrationBuilder.CreateTable(
                name: "SignedData",
                schema: "OffMemo",
                columns: table => new
                {
                    MessageGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterSignedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignDocumentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterSignedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignType = table.Column<string>(type: "varchar(30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignedData", x => x.MessageGuid);
                });

            migrationBuilder.CreateTable(
                name: "OfficialMemo",
                schema: "OffMemo",
                columns: table => new
                {
                    MessageGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutorCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SignerCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RegistrarCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recipients = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfidenceTypeId = table.Column<int>(type: "int", nullable: true),
                    VerticalText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attachments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalRequired = table.Column<bool>(type: "bit", nullable: false),
                    ApproveType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisterCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OfficialMemoRegNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DueToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MessageDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalDocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignDataMessageGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialMemo", x => x.MessageGuid);
                    table.ForeignKey(
                        name: "FK_OfficialMemo_ConfidenceTypes_ConfidenceTypeId",
                        column: x => x.ConfidenceTypeId,
                        principalSchema: "DocEx",
                        principalTable: "ConfidenceTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OfficialMemo_SignedData_SignDataMessageGuid",
                        column: x => x.SignDataMessageGuid,
                        principalSchema: "OffMemo",
                        principalTable: "SignedData",
                        principalColumn: "MessageGuid");
                });

            migrationBuilder.CreateTable(
                name: "ApprovalResults",
                schema: "OffMemo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApproverCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalResults_OfficialMemo_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "OffMemo",
                        principalTable: "OfficialMemo",
                        principalColumn: "MessageGuid");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeDboOfficialMemoDbo",
                schema: "OffMemo",
                columns: table => new
                {
                    Order = table.Column<int>(type: "int", nullable: false),
                    ApproversLogin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OfficialMemoDboMessageGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDboOfficialMemoDbo", x => new { x.Order, x.ApproversLogin, x.OfficialMemoDboMessageGuid });
                    table.ForeignKey(
                        name: "FK_EmployeeDboOfficialMemoDbo_OfficialMemo_OfficialMemoDboMessageGuid",
                        column: x => x.OfficialMemoDboMessageGuid,
                        principalSchema: "OffMemo",
                        principalTable: "OfficialMemo",
                        principalColumn: "MessageGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfficialMemoProcessData",
                schema: "OffMemo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PROCESS_GUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PROCESS_CODE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PROCESS_VERSION = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    START_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FINISH_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PROCESS_STATUS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegNum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InitiatorCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InitiatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Actual = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    MessageGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExecutorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutRegNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginRequestGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialMemoProcessData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficialMemoProcessData_OfficialMemo_MessageGuid",
                        column: x => x.MessageGuid,
                        principalSchema: "OffMemo",
                        principalTable: "OfficialMemo",
                        principalColumn: "MessageGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalResults_ApproverCode",
                schema: "OffMemo",
                table: "ApprovalResults",
                column: "ApproverCode");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalResults_MessageId",
                schema: "OffMemo",
                table: "ApprovalResults",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDboOfficialMemoDbo_ApproversLogin",
                schema: "OffMemo",
                table: "EmployeeDboOfficialMemoDbo",
                column: "ApproversLogin");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDboOfficialMemoDbo_OfficialMemoDboMessageGuid",
                schema: "OffMemo",
                table: "EmployeeDboOfficialMemoDbo",
                column: "OfficialMemoDboMessageGuid");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialMemo_ConfidenceTypeId",
                schema: "OffMemo",
                table: "OfficialMemo",
                column: "ConfidenceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialMemo_ExecutorCode",
                schema: "OffMemo",
                table: "OfficialMemo",
                column: "ExecutorCode");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialMemo_SignDataMessageGuid",
                schema: "OffMemo",
                table: "OfficialMemo",
                column: "SignDataMessageGuid");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialMemo_SignerCode",
                schema: "OffMemo",
                table: "OfficialMemo",
                column: "SignerCode");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialMemoProcessData_MessageGuid",
                schema: "OffMemo",
                table: "OfficialMemoProcessData",
                column: "MessageGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SignedData_SignedBy",
                schema: "OffMemo",
                table: "SignedData",
                column: "SignedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalResults",
                schema: "OffMemo");

            migrationBuilder.DropTable(
                name: "EmployeeDboOfficialMemoDbo",
                schema: "OffMemo");

            migrationBuilder.DropTable(
                name: "EmployeePositions",
                schema: "DocEx");

            migrationBuilder.DropTable(
                name: "IndexNomenclatures",
                schema: "OffMemo");

            migrationBuilder.DropTable(
                name: "OfficialMemoProcessData",
                schema: "OffMemo");

            migrationBuilder.DropTable(
                name: "OfficialMemo",
                schema: "OffMemo");

            migrationBuilder.DropTable(
                name: "ConfidenceTypes",
                schema: "DocEx");

            migrationBuilder.DropTable(
                name: "SignedData",
                schema: "OffMemo");
        }
    }
}
