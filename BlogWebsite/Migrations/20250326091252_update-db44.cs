using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
    public partial class updatedb44 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "writingPhases",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "writingPhases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "registrationPeriods",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "registrationPeriods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_writingPhases_ApplicationUserId",
                table: "writingPhases",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_registrationPeriods_ApplicationUserId",
                table: "registrationPeriods",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_registrationPeriods_AspNetUsers_ApplicationUserId",
                table: "registrationPeriods",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_writingPhases_AspNetUsers_ApplicationUserId",
                table: "writingPhases",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_registrationPeriods_AspNetUsers_ApplicationUserId",
                table: "registrationPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_writingPhases_AspNetUsers_ApplicationUserId",
                table: "writingPhases");

            migrationBuilder.DropIndex(
                name: "IX_writingPhases_ApplicationUserId",
                table: "writingPhases");

            migrationBuilder.DropIndex(
                name: "IX_registrationPeriods_ApplicationUserId",
                table: "registrationPeriods");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "writingPhases");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "writingPhases");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "registrationPeriods");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "registrationPeriods");
        }
    }
}
