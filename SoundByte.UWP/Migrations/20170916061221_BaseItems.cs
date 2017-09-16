using System;
using System.Collections.Generic;
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
                    Id = table.Column<string>(nullable: false),
                    ArtworkLink = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FollowersCount = table.Column<double>(nullable: false),
                    FollowingsCount = table.Column<double>(nullable: false),
                    PermalinkUri = table.Column<string>(nullable: true),
                    PlaylistCount = table.Column<double>(nullable: false),
                    ServiceType = table.Column<int>(nullable: false),
                    TrackCount = table.Column<double>(nullable: false),
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
                    Id = table.Column<string>(nullable: false),
                    ArtworkUrl = table.Column<string>(nullable: true),
                    AudioStreamUrl = table.Column<string>(nullable: true),
                    CommentCount = table.Column<double>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DislikeCount = table.Column<double>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    Genre = table.Column<string>(nullable: true),
                    Kind = table.Column<string>(nullable: true),
                    LastPlaybackDate = table.Column<DateTime>(nullable: false),
                    LikeCount = table.Column<double>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    ServiceType = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    VideoStreamUrl = table.Column<string>(nullable: true),
                    ViewCount = table.Column<double>(nullable: false)
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
