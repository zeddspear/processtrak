using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace processtrak_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddinMorePropertiesToProcessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Processes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "completionTime",
                table: "Processes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "remainingTime",
                table: "Processes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "responseTime",
                table: "Processes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "startTime",
                table: "Processes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "turnaroundTime",
                table: "Processes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "waitingTime",
                table: "Processes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "completionTime",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "remainingTime",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "responseTime",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "startTime",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "turnaroundTime",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "waitingTime",
                table: "Processes");
        }
    }
}
