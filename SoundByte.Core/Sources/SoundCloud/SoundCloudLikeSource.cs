using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundCloud
{
    [UsedImplicitly]
    public class SoundCloudLikeSource : ISource<BaseTrack>
    {
        /// <summary>
        ///     User to get soundcloud likes for
        /// </summary>
        [CanBeNull]
        public BaseUser User { get; set; }

        public Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "u", User?.UserId }
            };
        }

        public void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("u", out var userId);
            User = new BaseUser
            {
                UserId = userId.ToString()
            };
        }

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the SoundCloud API and get the items
            var tracks = await SoundByteService.Current.GetAsync<LikeTrackHolder>(ServiceType.SoundCloud, $"/users/{User?.UserId}/favorites",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"cursor", token}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Response.Tracks.Any())
            {
                return new SourceResponse<BaseTrack>(null, null, false, "No likes", "This user has not liked any tracks");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(tracks.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "cursor").Value;

            // Convert SoundCloud specific tracks to base tracks
            var baseTracks = new List<BaseTrack>();
            tracks.Response.Tracks.ForEach(x => baseTracks.Add(x.ToBaseTrack()));

            // Return the items
            return new SourceResponse<BaseTrack>(baseTracks, nextToken);
        }

        [JsonObject]
        private class LikeTrackHolder
        {
            /// <summary>
            ///     Collection of tracks
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudTrack> Tracks { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}
