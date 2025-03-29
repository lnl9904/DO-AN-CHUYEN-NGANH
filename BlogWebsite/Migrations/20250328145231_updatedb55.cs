using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
    public partial class updatedb55 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxArticles",
                table: "AspNetUsers",
                type: "int",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WritingPhaseID = table.Column<int>(type: "int", nullable: false),
                    WritingPhasesId = table.Column<int>(type: "int", nullable: true),
                    ArticleCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_assignments_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_assignments_writingPhases_WritingPhasesId",
                        column: x => x.WritingPhasesId,
                        principalTable: "writingPhases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "userCapacities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WritingPhaseID = table.Column<int>(type: "int", nullable: false),
                    WritingPhasesId = table.Column<int>(type: "int", nullable: true),
                    MaxAssignable = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userCapacities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userCapacities_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_userCapacities_writingPhases_WritingPhasesId",
                        column: x => x.WritingPhasesId,
                        principalTable: "writingPhases",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_assignments_ApplicationUserId",
                table: "assignments",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_assignments_WritingPhasesId",
                table: "assignments",
                column: "WritingPhasesId");

            migrationBuilder.CreateIndex(
                name: "IX_userCapacities_ApplicationUserId",
                table: "userCapacities",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_userCapacities_WritingPhasesId",
                table: "userCapacities",
                column: "WritingPhasesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assignments");

            migrationBuilder.DropTable(
                name: "userCapacities");

            migrationBuilder.DropColumn(
                name: "MaxArticles",
                table: "AspNetUsers");
        }
    }
}
