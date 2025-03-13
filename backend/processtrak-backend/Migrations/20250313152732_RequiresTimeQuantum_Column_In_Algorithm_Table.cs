using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace processtrak_backend.Migrations
{
    /// <inheritdoc />
    public partial class RequiresTimeQuantum_Column_In_Algorithm_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "requiresTimeQuantum",
                table: "Algorithms",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "requiresTimeQuantum",
                table: "Algorithms");
        }
    }
}
