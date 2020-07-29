using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlanningPoker.Website.Migrations
{
    public partial class CreatePlanningPokerDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    CardId = table.Column<byte[]>(nullable: false),
                    CardSource = table.Column<string>(nullable: true),
                    CardNumber = table.Column<string>(nullable: true),
                    DeveloperSize = table.Column<int>(nullable: false),
                    DeveloperVotes = table.Column<int>(nullable: false),
                    TestingSize = table.Column<int>(nullable: false),
                    TestingVotes = table.Column<int>(nullable: false),
                    StorySize = table.Column<int>(nullable: false),
                    GameId = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardId);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<byte[]>(nullable: false),
                    PlayerName = table.Column<string>(nullable: true),
                    PlayerType = table.Column<int>(nullable: false),
                    GameId = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<byte[]>(nullable: false),
                    GameName = table.Column<string>(nullable: true),
                    GameTime = table.Column<DateTime>(nullable: false),
                    GameMasterPlayerId = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Games_Players_GameMasterPlayerId",
                        column: x => x.GameMasterPlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_GameId",
                table: "Cards",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameMasterPlayerId",
                table: "Games",
                column: "GameMasterPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId",
                table: "Players",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Games_GameId",
                table: "Cards",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Games_GameId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
