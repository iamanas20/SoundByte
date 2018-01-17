using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundCloud.Search
{
    [UsedImplicitly]
    public class SoundCloudSearchUserSource : ISource<BaseUser>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<BaseUser>> GetItemsAsync(int count, string token, CancellationTokenSource cancellationToken = null)
        {
            // Call the SoundCloud API and get the items
            var users = await SoundByteService.Current.GetAsync<SearchUserHolder>(ServiceType.SoundCloud, "/users",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"offset", token},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no users
            if (!users.Response.Users.Any())
            {
                return new SourceResponse<BaseUser>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // Parse uri for offset
            var param = new QueryParameterCollection(users.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific users to base users
            var baseUsers = new List<BaseUser>();
            users.Response.Users.ForEach(x => baseUsers.Add(x.ToBaseUser()));

            // Return the items
            return new SourceResponse<BaseUser>(baseUsers, nextToken);
        }

        [JsonObject]
        private class SearchUserHolder
        {
            /// <summary>
            ///     List of users
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudUser> Users { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}
