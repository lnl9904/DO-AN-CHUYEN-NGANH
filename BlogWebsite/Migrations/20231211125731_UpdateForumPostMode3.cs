﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class UpdateForumPostMode3 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "AnswerCount",
				table: "forumPosts",
				type: "int",
				nullable: false,
				defaultValue: 0);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "AnswerCount",
				table: "forumPosts");
		}
	}
}
