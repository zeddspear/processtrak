using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace processtrak_backend.Migrations
{
    /// <inheritdoc />
    public partial class MakingJSONColumnsInSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processes_Schedules_Scheduleid",
                table: "Processes");

            migrationBuilder.DropTable(
                name: "AlgorithmSchedule");

            migrationBuilder.DropIndex(
                name: "IX_Processes_Scheduleid",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "Scheduleid",
                table: "Processes");

            migrationBuilder.AddColumn<Guid>(
                name: "Algorithmid",
                table: "Schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlgorithmsJson",
                table: "Schedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProcessesJson",
                table: "Schedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Algorithmid",
                table: "Schedules",
                column: "Algorithmid");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Algorithms_Algorithmid",
                table: "Schedules",
                column: "Algorithmid",
                principalTable: "Algorithms",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Algorithms_Algorithmid",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_Algorithmid",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Algorithmid",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "AlgorithmsJson",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "ProcessesJson",
                table: "Schedules");

            migrationBuilder.AddColumn<Guid>(
                name: "Scheduleid",
                table: "Processes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AlgorithmSchedule",
                columns: table => new
                {
                    algorithmsid = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduleRunsid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmSchedule", x => new { x.algorithmsid, x.scheduleRunsid });
                    table.ForeignKey(
                        name: "FK_AlgorithmSchedule_Algorithms_algorithmsid",
                        column: x => x.algorithmsid,
                        principalTable: "Algorithms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlgorithmSchedule_Schedules_scheduleRunsid",
                        column: x => x.scheduleRunsid,
                        principalTable: "Schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Processes_Scheduleid",
                table: "Processes",
                column: "Scheduleid");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmSchedule_scheduleRunsid",
                table: "AlgorithmSchedule",
                column: "scheduleRunsid");

            migrationBuilder.AddForeignKey(
                name: "FK_Processes_Schedules_Scheduleid",
                table: "Processes",
                column: "Scheduleid",
                principalTable: "Schedules",
                principalColumn: "id");
        }
    }
}
