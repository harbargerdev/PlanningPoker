using Microsoft.EntityFrameworkCore.Migrations;

namespace PlanningPoker.Website.Migrations
{
    public partial class AddIsLockedColumnToCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Cards",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Cards");
        }
    }
}
