/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.UWP.UserControls;
using SoundByte.YouTubeParser;
using SoundByte.YouTubeParser.Models;
using SoundByte.YouTubeParser.Models.MediaStreams;

namespace SoundByte.UWP.Models
{
    public class YouTubeSearchModel : BaseTrackModel
    {
        /// <summary>
        /// What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Loads search track items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            // Return a task that will get the items
            return await Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(Query))
                    return 0;

                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("YouTubeSearchModelInfoPane") as InfoPane)?.ShowLoading();
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                // only 10 tracks at once
                count = 10;

                try
                {
                    // Search for matching tracks
                    var searchTracks = await SoundByteV3Service.Current.GetAsync<YouTubeSearchList>(
                        ServiceType.YouTube, "search", new Dictionary<string, string>
                        {
                            {"part", "snippet"},
                            {"maxResults", count.ToString()},
                            {"q", Query},
                            {"pageToken", Token}
                        });

                    var offset = searchTracks.NextPageToken;

                    // Get the search offset
                    Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                    var client = new YoutubeClient();

                    // Make sure that there are tracks in the list
                    if (searchTracks.Items.Count > 0)
                    {
                        // Set the count variable
                        count = searchTracks.Items.Count;

                        foreach (var item in searchTracks.Items)
                        {
                            if (item.Id.Kind == "youtube#video")
                            {
                                if (item.Snippet.LiveBroadcastContent == "none")
                                {
                                    // If an item of the same ID has already been added, skip it
                                    if (this.FirstOrDefault(x => x.Id == item.Id.VideoId) != null)
                                        continue;

                                    VideoInfo video = await client.GetVideoInfoAsync(item.Id.VideoId);

                                    // Loop though all the tracks on the UI thread
                                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                                    {
                                        // Convert to a base track
                                        var track = item.ToBaseTrack();

                                        track.ArtworkUrl = item.Snippet.Thumbnails.DefaultSize.Url;
                                        track.User = new BaseUser
                                        {
                                            Username = item.Snippet.ChannelTitle
                                        };

                                        // Add missing details
                                        track.Duration = video.Duration;
                                        track.ViewCount = video.ViewCount;
                                        track.ArtworkUrl = video.ImageHighResUrl;
                                        track.AudioStreamUrl =
                                            video.AudioStreams.OrderBy(q => q.AudioEncoding).Last()?.Url;

                                        // 720p is max quality we want
                                        var wantedQuality =
                                            video.VideoStreams
                                                .FirstOrDefault(x => x.VideoQuality == VideoQuality.High720)?.Url;

                                        // If quality is not there, just get the highest (480p for example).
                                        if (string.IsNullOrEmpty(wantedQuality))
                                            wantedQuality = video.VideoStreams.OrderBy(s => s.VideoQuality).Last()?.Url;

                                        track.VideoStreamUrl = wantedQuality;

                                        // Add the track
                                        Add(track);
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        // There are no items, so we added no items
                        count = 0;

                        // Reset the token
                        Token = "eol";

                        // No items tell the user
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            (App.CurrentFrame?.FindName("YouTubeSearchModelInfoPane") as InfoPane)?.ShowMessage(
                                resources.GetString("SearchTrack_Header"),
                                resources.GetString("SearchTrack_Content"), "", false);
                        });
                    }
                }
                catch (SoundByteException ex)
                {
                    // Exception, most likely did not add any new items
                    count = 0;

                    // Reset the token
                    Token = "eol";

                    // Exception, display error to the user
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        (App.CurrentFrame?.FindName("YouTubeSearchModelInfoPane") as InfoPane)?.ShowMessage(
                            ex.ErrorTitle, ex.ErrorDescription, ex.ErrorGlyph);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("YouTubeSearchModelInfoPane") as InfoPane)?.ClosePane();
                });

                // Return the result
                return count;
            });
        }

        [JsonObject]
        public class YouTubeSearchList
        {
            [JsonProperty("nextPageToken")]
            public string NextPageToken { get; set; }

            [JsonProperty("items")]
            public List<YouTubeTrack> Items { get; set; }
        }
    }
}
