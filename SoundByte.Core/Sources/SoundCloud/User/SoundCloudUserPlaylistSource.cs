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

namespace SoundByte.Core.Sources.SoundCloud.User
{
    [UsedImplicitly]
    public class SoundCloudUserPlaylistSource : ISource<BasePlaylist>
    {
        [CanBeNull]
        public BaseUser User { get; set; }
 
        public  async Task<SourceResponse<BasePlaylist>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
           

            // If the user is null, and we are NOT connected, then we must be logged out and trying
            // to access our resources,
            if (User == null)
            {
                if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud))
                    return new SourceResponse<BasePlaylist>(null, null, false, "Something went wrong", "Cannot get likes for a user who does not exist.");

                return new SourceResponse<BasePlaylist>(null, null, false, "Not logged in", "A connected SoundCloud account is required to view this content.");
            }

            // Convert SoundCloud specific playlists to base playlists
            var basePlaylists = new List<BasePlaylist>();
            string nextToken;
            string endpoint;
            ServiceType serviceType;

            // If we are connected and the connect user is the current user we get the logged
            // in users playlists URL.
            if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud) &&
                User?.UserId == SoundByteService.Current.GetConnectedUser(ServiceType.SoundCloud)?.UserId)
            {
                endpoint = $"/users/{SoundByteService.Current.GetConnectedUser(ServiceType.SoundCloud)?.UserId}/playlists/liked_and_owned";
                serviceType = ServiceType.SoundCloudV2;

                // Call the SoundCloud api and get the items
                var playlists = await SoundByteService.Current.GetAsync<UserLikePlaylistHolder>(serviceType, endpoint,
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
                nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

                playlists.Playlists.ForEach(x => basePlaylists.Add(x.Playlist.ToBasePlaylist()));             
            }
            else
            {
                endpoint = $"/users/{User.UserId}/playlists";
                serviceType = ServiceType.SoundCloud;

                // Call the SoundCloud api and get the items
                var playlists = await SoundByteService.Current.GetAsync<UserPlaylistHolder>(serviceType, endpoint,
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
                nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

              
                playlists.Playlists.ForEach(x => basePlaylists.Add(x.ToBasePlaylist()));  
            }

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

        private class UserLikePlaylistHolder
        {
            /// <summary>
            ///     List of playlists
            /// </summary>
            [JsonProperty("collection")]
            public List<LikePlaylistBootstrap> Playlists { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }

        [JsonObject]
        private class LikePlaylistBootstrap
        {
            /// <summary>
            ///     A playlist
            /// </summary>
            [JsonProperty("playlist")]
            public SoundCloudPlaylist Playlist { get; set; }
        }
    }
}
