using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedJsonFieldsFromSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildStatus",
                schema: "data",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "data",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "StatusHistoryJson",
                schema: "data",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "TestResultsJson",
                schema: "data",
                table: "Submissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuildStatus",
                schema: "data",
                table: "Submissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "data",
                table: "Submissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusHistoryJson",
                schema: "data",
                table: "Submissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestResultsJson",
                schema: "data",
                table: "Submissions",
                type: "text",
                nullable: true);
        }
    }
}
