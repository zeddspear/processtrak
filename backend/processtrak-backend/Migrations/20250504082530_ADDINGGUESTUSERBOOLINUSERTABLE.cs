using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace processtrak_backend.Migrations
{
    /// <inheritdoc />
    public partial class ADDINGGUESTUSERBOOLINUSERTABLE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isGuest",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isGuest",
                table: "Users");
        }
    }
}
