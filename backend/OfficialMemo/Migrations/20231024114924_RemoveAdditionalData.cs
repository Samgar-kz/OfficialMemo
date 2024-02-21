using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficialMemo.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdditionalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalData",
                schema: "OffMemo",
                table: "OfficialMemo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalData",
                schema: "OffMemo",
                table: "OfficialMemo",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
