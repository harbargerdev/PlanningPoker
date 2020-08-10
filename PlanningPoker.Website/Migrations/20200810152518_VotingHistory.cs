using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlanningPoker.Website.Migrations
{
    public partial class VotingHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vote",
                columns: table => new
                {
                    VoteId = table.Column<byte[]>(nullable: false),
                    CardId = table.Column<byte[]>(nullable: true),
                    PlayerId = table.Column<byte[]>(nullable: true),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vote", x => x.VoteId);
                    table.ForeignKey(
                        name: "FK_Vote_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vote_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vote_CardId",
                table: "Vote",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_PlayerId",
                table: "Vote",
                column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vote");
        }
    }
}
