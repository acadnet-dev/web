using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "data",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                schema: "data",
                table: "Categories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Courses",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CourseId",
                schema: "data",
                table: "Categories",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Courses_CourseId",
                schema: "data",
                table: "Categories",
                column: "CourseId",
                principalSchema: "data",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Courses_CourseId",
                schema: "data",
                table: "Categories");

            migrationBuilder.DropTable(
                name: "Courses",
                schema: "data");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CourseId",
                schema: "data",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CourseId",
                schema: "data",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "data",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
