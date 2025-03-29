using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
    public partial class lnl2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_writingPhases_WritingPhasesId",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_WritingPhasesId",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "WritingPhasesId",
                table: "posts");

            migrationBuilder.CreateIndex(
                name: "IX_posts_WritingPhaseID",
                table: "posts",
                column: "WritingPhaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_writingPhases_WritingPhaseID",
                table: "posts",
                column: "WritingPhaseID",
                principalTable: "writingPhases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_writingPhases_WritingPhaseID",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_WritingPhaseID",
                table: "posts");

            migrationBuilder.AddColumn<int>(
                name: "WritingPhasesId",
                table: "posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_posts_WritingPhasesId",
                table: "posts",
                column: "WritingPhasesId");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_writingPhases_WritingPhasesId",
                table: "posts",
                column: "WritingPhasesId",
                principalTable: "writingPhases",
                principalColumn: "Id");
        }
    }
}
