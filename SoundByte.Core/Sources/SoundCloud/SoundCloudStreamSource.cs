/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundCloud
{
    [UsedImplicitly]
    public class SoundCloudStreamSource : ISource<GroupedItem>
    {
        public async Task<SourceResponse<GroupedItem>> GetItemsAsync(int count, string token, CancellationTokenSource cancellationToken = null)
        {
            if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud))
            {
                // Call the SoundCloud API and get the items
                var items = await SoundByteService.Current.GetAsync<StreamTrackHolder>(ServiceType.SoundCloud, "/e1/me/stream",
                    new Dictionary<string, string>
                    {
                        {"limit", count.ToString()},
                        {"linked_partitioning", "1"},
                        {"cursor", token}
                    }, cancellationToken).ConfigureAwait(false);

                // If there are no tracks
                if (!items.Items.Any())
                {
                    return new SourceResponse<GroupedItem>(null, null, false, "No items", "Follow someone to get started.");
                }

                // Parse uri for cursor
                var param = new QueryParameterCollection(items.NextList);
                var nextToken = param.FirstOrDefault(x => x.Key == "cursor").Value;

                // Convert the items to base items
                var baseItems = new List<GroupedItem>();
                foreach (var item in items.Items)
                {
                    var type = ItemType.Unknown;

                    // Set the type depending on the SoundCloud String
                    // and if the object exists.
                    switch (item.Type)
                    {
                        case "track-repost":
                        case "track":
                            if (item.Track != null)
                                type = ItemType.Track;
                            break;
                        case "playlist-repost":
                        case "playlist":
                            if (item.Playlist != null)
                                type = ItemType.Playlist;
                            break;
                    }

                    // Only add the item if it's a known type
                    if (type != ItemType.Unknown)
                    {
                        baseItems.Add(new GroupedItem
                        {
                            Type = type,
                            Track = item.Track?.ToBaseTrack(),
                            Playlist = item.Playlist?.ToBasePlaylist(),
                            User = item.User?.ToBaseUser()
                        });
                    }
                }

                // Return the items
                return new SourceResponse<GroupedItem>(baseItems, nextToken);
            }

            return new SourceResponse<GroupedItem>(null, null, false, "SoundCloud Service not Connected", "Please connect your SoundCloud account in order to access items.");
        }

        [JsonObject]
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