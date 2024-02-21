using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficialMemo.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectAndIndexNomencaltures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IndexNomenclature",
                schema: "OffMemo",
                table: "OfficialMemo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                schema: "OffMemo",
                table: "OfficialMemo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndexNomenclature",
                schema: "OffMemo",
                table: "OfficialMemo");

            migrationBuilder.DropColumn(
                name: "Subject",
                schema: "OffMemo",
                table: "OfficialMemo");
        }
    }
}
