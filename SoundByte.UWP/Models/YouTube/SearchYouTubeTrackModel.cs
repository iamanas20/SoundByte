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
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.UWP.Models.YouTube
{
    public class SearchYouTubeTrackModel : BaseModel<BaseTrack>
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

            if (string.IsNullOrEmpty(Query))
                return 0;

            // Get the resource loader
            var resources = ResourceLoader.GetForViewIndependentUse();

            if (count <= 10)
                count = 10;

            if (count >= 50)
                count = 50;

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

                // Make sure that there are tracks in the list
                if (searchTracks.Items.Count > 0)
                {
                    // Set the count variable
                    count = searchTracks.Items.Count;

                    foreach (var item in searchTracks.Items)
                    {
                        if (item.Id.Kind == "youtube#video")
                        {                          
                            // Loop though all the tracks on the UI thread
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                // Convert to a base track
                                var track = item.ToBaseTrack();

                                // Are we a live stream
                                track.IsLive = item.Snippet.LiveBroadcastContent != "none";

                                // Add the track
                                Add(track);
                            });
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
                    await ShowErrorMessageAsync(resources.GetString("SearchTrack_Header"),
                        resources.GetString("SearchTrack_Content"));
                }
            }
            catch (SoundByteException ex)
            {
                // Exception, most likely did not add any new items
                count = 0;

                // Reset the token
                Token = "eol";

                await ShowErrorMessageAsync(ex.ErrorTitle, ex.ErrorDescription);
            }

            // Return the result
            return count;
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
