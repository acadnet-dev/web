using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SolutionSubmissionId",
                schema: "data",
                table: "Problems",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Submissions",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    BuildStatus = table.Column<string>(type: "text", nullable: false),
                    TestResultsJson = table.Column<string>(type: "text", nullable: false),
                    StatusHistoryJson = table.Column<string>(type: "text", nullable: false),
                    ProblemId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "data",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Submissions_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalSchema: "data",
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Problems_SolutionSubmissionId",
                schema: "data",
                table: "Problems",
                column: "SolutionSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ProblemId",
                schema: "data",
                table: "Submissions",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UserId",
                schema: "data",
                table: "Submissions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Submissions_SolutionSubmissionId",
                schema: "data",
                table: "Problems",
                column: "SolutionSubmissionId",
                principalSchema: "data",
                principalTable: "Submissions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Submissions_SolutionSubmissionId",
                schema: "data",
                table: "Problems");

            migrationBuilder.DropTable(
                name: "Submissions",
                schema: "data");

            migrationBuilder.DropIndex(
                name: "IX_Problems_SolutionSubmissionId",
                schema: "data",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "SolutionSubmissionId",
                schema: "data",
                table: "Problems");
        }
    }
}
