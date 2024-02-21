using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficialMemo.Migrations
{
    /// <inheritdoc />
    public partial class ToMain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recipients",
                schema: "OffMemo",
                table: "OfficialMemo");

            migrationBuilder.AlterColumn<int>(
                name: "AmountPage",
                schema: "OffMemo",
                table: "OfficialMemo",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RegistrarRequired",
                schema: "OffMemo",
                table: "OfficialMemo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ExecutorCode",
                schema: "OffMemo",
                table: "ApprovalResults",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ReceivingResults",
                schema: "OffMemo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiverCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExecutorCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivingResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceivingResults_OfficialMemo_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "OffMemo",
                        principalTable: "OfficialMemo",
                        principalColumn: "MessageGuid");
                });

            migrationBuilder.CreateTable(
                name: "RecipientDboOfficialMemoDbo",
                schema: "OffMemo",
                columns: table => new
                {
                    Order = table.Column<int>(type: "int", nullable: false),
                    RecipientsLogin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OfficialMemoDboMessageGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipientDboOfficialMemoDbo", x => new { x.Order, x.RecipientsLogin, x.OfficialMemoDboMessageGuid });
                    table.ForeignKey(
                        name: "FK_RecipientDboOfficialMemoDbo_OfficialMemo_OfficialMemoDboMessageGuid",
                        column: x => x.OfficialMemoDboMessageGuid,
                        principalSchema: "OffMemo",
                        principalTable: "OfficialMemo",
                        principalColumn: "MessageGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalResults_ExecutorCode",
                schema: "OffMemo",
                table: "ApprovalResults",
                column: "ExecutorCode");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_ProcessName",
                schema: "DocEx",
                table: "LogEntries",
                column: "ProcessName");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingResults_ExecutorCode",
                schema: "OffMemo",
                table: "ReceivingResults",
                column: "ExecutorCode");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingResults_MessageId",
                schema: "OffMemo",
                table: "ReceivingResults",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivingResults_ReceiverCode",
                schema: "OffMemo",
                table: "ReceivingResults",
                column: "ReceiverCode");

            migrationBuilder.CreateIndex(
                name: "IX_RecipientDboOfficialMemoDbo_OfficialMemoDboMessageGuid",
                schema: "OffMemo",
                table: "RecipientDboOfficialMemoDbo",
                column: "OfficialMemoDboMessageGuid");

            migrationBuilder.CreateIndex(
                name: "IX_RecipientDboOfficialMemoDbo_RecipientsLogin",
                schema: "OffMemo",
                table: "RecipientDboOfficialMemoDbo",
                column: "RecipientsLogin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepCodeToRegCodes",
                schema: "DocEx");

            migrationBuilder.DropTable(
                name: "LogEntries",
                schema: "DocEx");

            migrationBuilder.DropTable(
                name: "ReceivingResults",
                schema: "OffMemo");

            migrationBuilder.DropTable(
                name: "RecipientDboOfficialMemoDbo",
                schema: "OffMemo");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalResults_ExecutorCode",
                schema: "OffMemo",
                table: "ApprovalResults");

            migrationBuilder.DropColumn(
                name: "RegistrarRequired",
                schema: "OffMemo",
                table: "OfficialMemo");

            migrationBuilder.DropColumn(
                name: "ExecutorCode",
                schema: "OffMemo",
                table: "ApprovalResults");

            migrationBuilder.AlterColumn<string>(
                name: "AmountPage",
                schema: "OffMemo",
                table: "OfficialMemo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Recipients",
                schema: "OffMemo",
                table: "OfficialMemo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
