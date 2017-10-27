using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SoundByte.UWP.Migrations
{
    public partial class BaseItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(),
                    ArtworkLink = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FollowersCount = table.Column<double>(),
                    FollowingsCount = table.Column<double>(),
                    PermalinkUri = table.Column<string>(nullable: true),
                    PlaylistCount = table.Column<double>(),
                    ServiceType = table.Column<int>(),
                    TrackCount = table.Column<double>(),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<string>(),
                    ArtworkUrl = table.Column<string>(nullable: true),
                    AudioStreamUrl = table.Column<string>(nullable: true),
                    CommentCount = table.Column<double>(),
                    Created = table.Column<DateTime>(),
                    Description = table.Column<string>(nullable: true),
                    DislikeCount = table.Column<double>(),
                    Duration = table.Column<TimeSpan>(),
                    Genre = table.Column<string>(nullable: true),
                    Kind = table.Column<string>(nullable: true),
                    LastPlaybackDate = table.Column<DateTime>(),
                    LikeCount = table.Column<double>(),
                    Link = table.Column<string>(nullable: true),
                    ServiceType = table.Column<int>(),
                    Title = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    VideoStreamUrl = table.Column<string>(nullable: true),
                    ViewCount = table.Column<double>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tracks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_UserId",
                table: "Tracks",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
