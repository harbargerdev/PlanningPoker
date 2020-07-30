using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlanningPoker.Website.Migrations
{
    public partial class AddingActiveCardToGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ActiveCardCardId",
                table: "Games",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_ActiveCardCardId",
                table: "Games",
                column: "ActiveCardCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Cards_ActiveCardCardId",
                table: "Games",
                column: "ActiveCardCardId",
                principalTable: "Cards",
                principalColumn: "CardId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Cards_ActiveCardCardId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_ActiveCardCardId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ActiveCardCardId",
                table: "Games");
        }
    }
}
