using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace processtrak_backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangingKeysToCamelCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processes_Users_UserId",
                table: "Processes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserSessions",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "UserSessions",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "ExpiryTime",
                table: "UserSessions",
                newName: "expiryTime");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                newName: "IX_UserSessions_userId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Processes",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "ProcessId",
                table: "Processes",
                newName: "processId");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Processes",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "BurstTime",
                table: "Processes",
                newName: "burstTime");

            migrationBuilder.RenameColumn(
                name: "ArrivalTime",
                table: "Processes",
                newName: "arrivalTime");

            migrationBuilder.RenameIndex(
                name: "IX_Processes_UserId",
                table: "Processes",
                newName: "IX_Processes_userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Processes_Users_userId",
                table: "Processes",
                column: "userId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Users_userId",
                table: "UserSessions",
                column: "userId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processes_Users_userId",
                table: "Processes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessions_Users_userId",
                table: "UserSessions");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "UserSessions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "UserSessions",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "expiryTime",
                table: "UserSessions",
                newName: "ExpiryTime");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_userId",
                table: "UserSessions",
                newName: "IX_UserSessions_UserId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Processes",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "processId",
                table: "Processes",
                newName: "ProcessId");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "Processes",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "burstTime",
                table: "Processes",
                newName: "BurstTime");

            migrationBuilder.RenameColumn(
                name: "arrivalTime",
                table: "Processes",
                newName: "ArrivalTime");

            migrationBuilder.RenameIndex(
                name: "IX_Processes_userId",
                table: "Processes",
                newName: "IX_Processes_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Processes_Users_UserId",
                table: "Processes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessions_Users_UserId",
                table: "UserSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
