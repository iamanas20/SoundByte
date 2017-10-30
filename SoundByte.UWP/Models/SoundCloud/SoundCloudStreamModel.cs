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
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.UWP.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models.SoundCloud
{
    /// <summary>
    ///     Model for the users stream
    /// </summary>
    public class SoundCloudStreamModel : ObservableCollection<GroupedItem>, ISupportIncrementalLoading
    {
        /// <summary>
        ///     The position of the track, will be 'eol'
        ///     if there are no new tracks
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        ///     Are there more items to load
        /// </summary>
        public bool HasMoreItems => Token != "eol";

        /// <summary>
        ///     Loads stream items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        // ReSharper disable once RedundantAssignment
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // Return a task that will get the items
            return Task.Run(async () =>
            {
                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ShowLoading();
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                // Check if the user is not logged in
                if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud))
                {
                    try
                    {
                        // At least 20 tracks at once
                        if (count <= 10)
                            count = 10;

                        if (count >= 50)
                            count = 50;

                        // Get items from the users stream
                        var streamTracks = await SoundByteV3Service.Current.GetAsync<StreamTrackHolder>(ServiceType.SoundCloud, "/e1/me/stream",
                            new Dictionary<string, string>
                            {
                                {"limit", count.ToString()},
                                {"cursor", Token}
                            });

                        // Parse uri for offset
                        var param = new QueryParameterCollection(streamTracks.NextList);
                        var cursor = param.FirstOrDefault(x => x.Key == "cursor").Value;

                        // Get the stream cursor
                        Token = string.IsNullOrEmpty(cursor) ? "eol" : cursor;

                        // Make sure that there are tracks in the list
                        if (streamTracks.Items.Count > 0)
                        {
                            // Set the count variable
                            count = (uint) streamTracks.Items.Count;

                            // Loop though all the tracks on the UI thread
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                foreach (var item in streamTracks.Items)
                                {
                                    var type = ItemType.Track;

                                    switch(item.Type)
                                    {
                                        case "track-repost":
                                        case "track":
                                            type = ItemType.Track;
                                            break;
                                        case "playlist-repost":
                                        case "playlist":
                                            type = ItemType.Playlist;
                                            break;
                                    }                          

                                    Add(new GroupedItem
                                    {
                                        Type = type,
                                        Track = item.Track?.ToBaseTrack(),
                                        Playlist = item.Playlist?.ToBasePlaylist(),
                                        User = item.User?.ToBaseUser()
                                    });
                                }
                            });
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
                                (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ShowMessage(
                                    resources.GetString("StreamTracks_Header"),
                                    resources.GetString("StreamTracks_Content"), false);
                            });
                        }
                    }
                    catch (SoundByteException ex)
                    {
                        // Exception, most likely did not add any new items
                        count = 0;

                        // Reset the token
                        Token = "eol";

                        TelemetryService.Instance.TrackException(ex, false);

                        // Exception, display error to the user
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ShowMessage(
                                ex.ErrorTitle, ex.ErrorDescription);
                        });
                    }
                }
                else
                {
                    // Not logged in, so no new items
                    count = 0;

                    // Reset the token
                    Token = "eol";

                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ShowMessage(
                            resources.GetString("ErrorControl_LoginFalse_Header"),
                            resources.GetString("ErrorControl_LoginFalse_Content"), false);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ClosePane();
                });

                // Return the result
                return new LoadMoreItemsResult {Count = count};
            }).AsAsyncOperation();
        }

        /// <summary>
        ///     Refresh the list by removing any
        ///     existing items and reseting the token.
        /// </summary>
        public void RefreshItems()
        {
            Token = null;
            Clear();
        }

        private class StreamTrackHolder
        {
            /// <summary>
            ///     List of stream items
            /// </summary>
            [JsonProperty("collection")]
            public List<StreamItem> Items { get; set; }

            /// <summary>
            ///     Next items in the list
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }

        /// <summary>
        ///     A stream collection containing all items that may be on the users stream
        /// </summary>
        [JsonObject]
        private class StreamItem
        {
            /// <summary>
            ///     Track detail
            /// </summary>
            [JsonProperty("track")]
            public SoundCloudTrack Track { get; set; }

            /// <summary>
            ///     User detail
            /// </summary>
            [JsonProperty("user")]
            public SoundCloudUser User { get; set; }

            /// <summary>
            ///     Playlist detail
            /// </summary>
            [JsonProperty("playlist")]
            public SoundCloudPlaylist Playlist { get; set; }

            /// <summary>
            ///     When this object was created
            /// </summary>
            [JsonProperty("created_at")]
            public string CreatedAt { get; set; }

            /// <summary>
            ///     What type of object this is
            /// </summary>
            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }
}