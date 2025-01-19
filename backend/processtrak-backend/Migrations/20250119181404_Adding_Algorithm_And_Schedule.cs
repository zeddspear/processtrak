using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace processtrak_backend.Migrations
{
    /// <inheritdoc />
    public partial class Adding_Algorithm_And_Schedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Scheduleid",
                table: "Processes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Algorithms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Algorithms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    startTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    totalExecutionTime = table.Column<int>(type: "integer", nullable: false),
                    averageWaitingTime = table.Column<int>(type: "integer", nullable: false),
                    averageTurnaroundTime = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processes_Schedules_Scheduleid",
                table: "Processes");

            migrationBuilder.DropTable(
                name: "AlgorithmSchedule");

            migrationBuilder.DropTable(
                name: "Algorithms");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Processes_Scheduleid",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "Scheduleid",
                table: "Processes");
        }
    }
}
