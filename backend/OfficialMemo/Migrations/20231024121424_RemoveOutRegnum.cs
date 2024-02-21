using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficialMemo.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOutRegnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutRegNum",
                schema: "OffMemo",
                table: "OfficialMemoProcessData");

            migrationBuilder.RenameColumn(
                name: "OfficialMemoRegNum",
                schema: "OffMemo",
                table: "OfficialMemo",
                newName: "AmountPage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AmountPage",
                schema: "OffMemo",
                table: "OfficialMemo",
                newName: "OfficialMemoRegNum");

            migrationBuilder.AddColumn<string>(
                name: "OutRegNum",
                schema: "OffMemo",
                table: "OfficialMemoProcessData",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
