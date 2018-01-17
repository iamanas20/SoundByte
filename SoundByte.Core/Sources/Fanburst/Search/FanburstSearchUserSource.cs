using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.Fanburst.Search
{
    [UsedImplicitly]
    public class FanburstSearchUserSource : ISource<BaseUser>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public async Task<SourceResponse<BaseUser>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the Fanburst API and get the items
            var users = await SoundByteService.Current.GetAsync<List<FanburstUser>>(ServiceType.Fanburst, "users/search",
                new Dictionary<string, string>
                {
                    {"query", WebUtility.UrlEncode(SearchQuery)},
                    {"per_page", count.ToString()}
                }, cancellationToken).ConfigureAwait(false);


            // If there are no users
            if (!users.Response.Any())
            {
                return new SourceResponse<BaseUser>(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // Convert Fanburst specific users to base users
            var baseUsers = new List<BaseUser>();
            users.Response.ForEach(x => baseUsers.Add(x.ToBaseUser()));

            // Return the items
            return new SourceResponse<BaseUser>(baseUsers, "eol");
        }
    }
}
