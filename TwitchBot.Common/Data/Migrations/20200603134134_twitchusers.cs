using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchBot.Common.Migrations
{
    public partial class twitchusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TwitchBot");

            migrationBuilder.CreateTable(
                name: "TwitchUser",
                schema: "TwitchBot",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false),
                    TwitchUserName = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchUser", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwitchUser",
                schema: "TwitchBot");
        }
    }
}
