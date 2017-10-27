using Microsoft.EntityFrameworkCore.Migrations;

namespace SoundByte.UWP.Migrations
{
    public partial class AddIsLive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                "IsLive",
                "Tracks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "IsLive",
                "Tracks");
        }
    }
}
