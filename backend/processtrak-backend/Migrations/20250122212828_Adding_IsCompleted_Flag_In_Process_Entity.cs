using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace processtrak_backend.Migrations
{
    /// <inheritdoc />
    public partial class Adding_IsCompleted_Flag_In_Process_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCompleted",
                table: "Processes",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCompleted",
                table: "Processes");
        }
    }
}
