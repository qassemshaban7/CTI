using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CTI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuestionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_Answers_AnswerId",
                table: "Results");

            migrationBuilder.DropForeignKey(
                name: "FK_Results_Questions_QuestionId",
                table: "Results");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Results_AnswerId",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "Results");

            migrationBuilder.RenameColumn(
                name: "QuestionName",
                table: "Questions",
                newName: "QuestionText");

            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "Results",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstAnswer",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FourthAnswer",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionType",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SecondAnswer",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdAnswer",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b561e14-8950-42cd-8ceb-56a9588d2d06", "AQAAAAIAAYagAAAAEBgOo7TI23uOSTHORTLWpBUhcTr4RNnxbsyEqgh01//kjb5yw4abPOc/Oq1T/rGE0w==", "03ce08b4-7f86-40e4-ae36-bdbbe1319b3b" });

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Questions_QuestionId",
                table: "Results",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_Questions_QuestionId",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "Answer",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "FirstAnswer",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "FourthAnswer",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "QuestionType",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "SecondAnswer",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ThirdAnswer",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "QuestionText",
                table: "Questions",
                newName: "QuestionName");

            migrationBuilder.AddColumn<int>(
                name: "AnswerId",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ecc07b18-f55e-4f6b-95bd-0e84f556135f",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0125941d-748f-4426-83e8-0352722473d6", "AQAAAAIAAYagAAAAELJYTBQf3x+XVRtNZodXjPRAUtvxN+aLll1ZrwIUgNbe9yMdAju2ZDNEJ1wqgEvjug==", "9f6926d2-7c38-4e30-be09-0dc0a4cfd445" });

            migrationBuilder.CreateIndex(
                name: "IX_Results_AnswerId",
                table: "Results",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Answers_AnswerId",
                table: "Results",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Questions_QuestionId",
                table: "Results",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }
    }
}
