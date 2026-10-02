using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHire.Migrations
{
    /// <inheritdoc />
    public partial class AddScimExternalIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScimExternalId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScimExternalId",
                table: "Users");
        }
    }
}
