using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedMaintainersToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseUser",
                schema: "data",
                columns: table => new
                {
                    MaintainedCoursesId = table.Column<int>(type: "integer", nullable: false),
                    MaintainersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseUser", x => new { x.MaintainedCoursesId, x.MaintainersId });
                    table.ForeignKey(
                        name: "FK_CourseUser_AspNetUsers_MaintainersId",
                        column: x => x.MaintainersId,
                        principalSchema: "data",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseUser_Courses_MaintainedCoursesId",
                        column: x => x.MaintainedCoursesId,
                        principalSchema: "data",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseUser_MaintainersId",
                schema: "data",
                table: "CourseUser",
                column: "MaintainersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseUser",
                schema: "data");
        }
    }
}
