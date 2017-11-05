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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundCloud
{
    [UsedImplicitly]
    public class UserSoundCloudPlaylistSource : ISource<BasePlaylist>
    {
        [CanBeNull]
        public BaseUser User { get; set; }
 
        public  async Task<SourceResponse<BasePlaylist>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            string endpoint;
            ServiceType serviceType;

            // If the user is null, and we are NOT connected, then we must be logged out and trying
            // to access our resources,
            if (User == null)
            {
                if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud))
                    return new SourceResponse<BasePlaylist>(null, null, false, "Something went wrong", "Cannot get likes for a user who does not exist.");

                return new SourceResponse<BasePlaylist>(null, null, false, "Not logged in", "A connected SoundCloud account is required to view this content.");
            }

            // If we are connected and the connect user is the current user we get the logged
            // in users playlists URL.
            if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud) &&
                User?.Id == SoundByteV3Service.Current.GetConnectedUser(ServiceType.SoundCloud)?.Id)
            {
                endpoint = $"/users/{SoundByteV3Service.Current.GetConnectedUser(ServiceType.SoundCloud)?.Id}/playlists/liked_and_owned";
                serviceType = ServiceType.SoundCloudV2;
            }
            else
            {
                endpoint = $"/users/{User.Id}/playlists";
                serviceType = ServiceType.SoundCloud;
            }

            // Call the SoundCloud api and get the items
            var playlists = await SoundByteV3Service.Current.GetAsync<UserPlaylistHolder>(serviceType, endpoint,
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"offset", token},
                    {"linked_partitioning", "1"}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!playlists.Playlists.Any())
            {
                return new SourceResponse<BasePlaylist>(null, null, false, "Nothing to hear here", "This user has uploaded no playlists");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(playlists.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific playlists to base playlists
            var basePlaylists = new List<BasePlaylist>();
            playlists.Playlists.ForEach(x => basePlaylists.Add(x.ToBasePlaylist()));

            // Return the items
            return new SourceResponse<BasePlaylist>(basePlaylists, nextToken);
        }

        [JsonObject]
        private class UserPlaylistHolder
        {
            /// <summary>
            ///     List of playlists
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudPlaylist> Playlists { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}
