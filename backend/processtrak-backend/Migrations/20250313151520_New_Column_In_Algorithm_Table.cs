using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace processtrak_backend.Migrations
{
    /// <inheritdoc />
    public partial class New_Column_In_Algorithm_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "displayName",
                table: "Algorithms",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "displayName",
                table: "Algorithms");
        }
    }
}
