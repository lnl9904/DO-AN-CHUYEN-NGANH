using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
    public partial class updatecodedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Download_path",
                table: "posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPost",
                table: "posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "posts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Royalty",
                table: "posts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "WritingPhaseID",
                table: "posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WritingPhasesId",
                table: "posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "registrationPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegisEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegisDeadlineStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegisDeadlineEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Is_Opening_registration = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registrationPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "writingPhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountArticles = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Is_Opening_registration = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationPeriodID = table.Column<int>(type: "int", nullable: false),
                    RegistrationPeriodsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_writingPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_writingPhases_registrationPeriods_RegistrationPeriodsId",
                        column: x => x.RegistrationPeriodsId,
                        principalTable: "registrationPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_posts_WritingPhasesId",
                table: "posts",
                column: "WritingPhasesId");

            migrationBuilder.CreateIndex(
                name: "IX_writingPhases_RegistrationPeriodsId",
                table: "writingPhases",
                column: "RegistrationPeriodsId");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_writingPhases_WritingPhasesId",
                table: "posts",
                column: "WritingPhasesId",
                principalTable: "writingPhases",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_writingPhases_WritingPhasesId",
                table: "posts");

            migrationBuilder.DropTable(
                name: "writingPhases");

            migrationBuilder.DropTable(
                name: "registrationPeriods");

            migrationBuilder.DropIndex(
                name: "IX_posts_WritingPhasesId",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "Download_path",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "IsPost",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "Royalty",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "WritingPhaseID",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "WritingPhasesId",
                table: "posts");
        }
    }
}
