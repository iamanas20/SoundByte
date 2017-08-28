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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Graphics.Printing;
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp;
using SoundByte.API.Endpoints;
using SoundByte.API.Exceptions;
using SoundByte.Core.Services;
using SoundByte.UWP.UserControls;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace SoundByte.UWP.Models
{
    public class YouTubeSearchModel : ObservableCollection<Track>, ISupportIncrementalLoading
    {
        /// <summary>
        /// The position of the track, will be 'eol'
        /// if there are no new trackss
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Filter the search
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Are there more items to load
        /// </summary>
        public bool HasMoreItems => Token != "eol";

        /// <summary>
        /// Refresh the list by removing any
        /// existing items and reseting the token.
        /// </summary>
        public void RefreshItems()
        {
            Token = null;
            Clear();
        }

        /// <summary>
        /// Loads search track items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // Return a task that will get the items
            return Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(Query))
                    return new LoadMoreItemsResult { Count = 0 };

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
                    var searchTracks = await SoundByteService.Instance.GetAsync<dynamic>(
                        ServiceType.YouTube, "search", new Dictionary<string, string>
                        {
                            {"part", "snippet"},
                            {"maxResults", count.ToString() },
                            { "q", Query },
                            { "pageToken", Token }
                        });

                    // Parse uri for offset
                    //   var param = new QueryParameterCollection(searchTracks.NextList);
                    var offset = (string)searchTracks.nextPageToken;

                    // Get the search offset
                    Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                    var client = new YoutubeClient();

                    // Make sure that there are tracks in the list
                    if (searchTracks.items.Count > 0)
                    {
                        // Set the count variable
                        count = (uint)searchTracks.items.Count;

                        foreach (var item in searchTracks.items)
                        {
                            if (item.id.kind == "youtube#video")
                            {
                                if (item.snippet.liveBroadcastContent == "none")
                                {
                                    VideoInfo video = await client.GetVideoInfoAsync((string)item.id.videoId);

                                    // Loop though all the tracks on the UI thread
                                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                                    {
                                        var track = new Track
                                        {
                                            ServiceType = ServiceType.YouTube,
                                            Id = item.id.videoId,
                                            Kind = "track",
                                            Duration = (int) video.Duration.TotalMilliseconds,
                                            CreationDate = item.snippet.publishedAt,
                                            Description = video.Description,
                                            LikesCount = (int) video.LikeCount,
                                            PlaybackCount = (int) video.ViewCount,
                                            ArtworkLink = video.ImageHighResUrl,
                                            Title = item.snippet.title,               
                                            VideoStreamUrl = video.MixedStreams.OrderBy(s => s.VideoQuality).Last()?.Url,
                                            User = new User
                                            {
                                                Username = item.snippet.channelTitle
                                            }
                                        };

                                        // Prefer 720p (still sounds good)
                                        var wantedQuality = video.MixedStreams.FirstOrDefault(x => x.VideoQuality == VideoQuality.Medium480)?.Url;

                                        if (string.IsNullOrEmpty(wantedQuality))
                                            wantedQuality = video.MixedStreams.OrderBy(s => s.VideoQuality).Last()?.Url;

                                        track.StreamUrl = wantedQuality;

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
                return new LoadMoreItemsResult { Count = count };
            }).AsAsyncOperation();
        }
    }
}
