using Microsoft.EntityFrameworkCore.Migrations;

namespace PlanningPoker.Website.Migrations
{
    public partial class AddingIsFinishedToCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Cards",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Cards");
        }
    }
}
