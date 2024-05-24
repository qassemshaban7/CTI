using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSurveyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Surveys_AspNetUsers_UserId",
                table: "Surveys");

            migrationBuilder.DropForeignKey(
                name: "FK_Surveys_Courses_CourseId",
                table: "Surveys");

            migrationBuilder.DropIndex(
                name: "IX_Surveys_CourseId",
                table: "Surveys");

            migrationBuilder.DropIndex(
                name: "IX_Surveys_UserId",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Surveys");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Surveys",
                newName: "SurveyType");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0125941d-748f-4426-83e8-0352722473d6", "AQAAAAIAAYagAAAAELJYTBQf3x+XVRtNZodXjPRAUtvxN+aLll1ZrwIUgNbe9yMdAju2ZDNEJ1wqgEvjug==", "9f6926d2-7c38-4e30-be09-0dc0a4cfd445" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SurveyType",
                table: "Surveys",
                newName: "CourseId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Surveys",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8c7c588b-1d7d-49b4-bed0-add9dd9ef3fd", "AQAAAAIAAYagAAAAENIUjdvLoE2TVhEUtBQb9gvhL6VToDcIkMM67jCSOl/jJuNaOle2bjC/we/tQI47lQ==", "f8d30e04-ca86-47b0-a7b9-6678626bc144" });

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_CourseId",
                table: "Surveys",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_UserId",
                table: "Surveys",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_AspNetUsers_UserId",
                table: "Surveys",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_Courses_CourseId",
                table: "Surveys",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
