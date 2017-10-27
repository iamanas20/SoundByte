using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SoundByte.UWP.DatabaseContexts;

namespace SoundByte.UWP.Migrations
{
    [DbContext(typeof(HistoryContext))]
    partial class HistoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("SoundByte.Core.Items.Track.BaseTrack", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ArtworkUrl");

                    b.Property<string>("AudioStreamUrl");

                    b.Property<double>("CommentCount");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<double>("DislikeCount");

                    b.Property<TimeSpan>("Duration");

                    b.Property<string>("Genre");

                    b.Property<bool>("IsLive");

                    b.Property<string>("Kind");

                    b.Property<DateTime>("LastPlaybackDate");

                    b.Property<double>("LikeCount");

                    b.Property<string>("Link");

                    b.Property<int>("ServiceType");

                    b.Property<string>("Title");

                    b.Property<string>("UserId");

                    b.Property<string>("VideoStreamUrl");

                    b.Property<double>("ViewCount");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("SoundByte.Core.Items.User.BaseUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ArtworkLink");

                    b.Property<string>("Country");

                    b.Property<string>("Description");

                    b.Property<double>("FollowersCount");

                    b.Property<double>("FollowingsCount");

                    b.Property<string>("PermalinkUri");

                    b.Property<double>("PlaylistCount");

                    b.Property<int>("ServiceType");

                    b.Property<double>("TrackCount");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SoundByte.Core.Items.Track.BaseTrack", b =>
                {
                    b.HasOne("SoundByte.Core.Items.User.BaseUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
