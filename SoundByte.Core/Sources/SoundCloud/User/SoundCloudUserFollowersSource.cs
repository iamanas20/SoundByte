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
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundCloud.User
{
    [UsedImplicitly]
    public class SoundCloudUserFollowersSource : ISource<BaseUser>
    {
        /// <summary>
        ///     User object that we will used to get the follower / followings for
        /// </summary>
        [CanBeNull]
        public BaseUser User { get; set; }

        /// <summary>
        ///     What type of object is this (followers, followings)
        /// </summary>
        [CanBeNull]
        public string Type { get; set; }

        public async Task<SourceResponse<BaseUser>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the SoundCloud API and get the items
            var followings = await SoundByteService.Current.GetAsync<UserListHolder>(ServiceType.SoundCloud, $"/users/{User.UserId}/{Type}",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"cursor", token}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no users
            if (!followings.Users.Any())
            {
                return new SourceResponse<BaseUser>(null, null, false, "No users", "This user does not follow anything  have anyone following them");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(followings.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "cursor").Value;

            // Convert SoundCloud specific tracks to base tracks
            var baseUsers = new List<BaseUser>();
            followings.Users.ForEach(x => baseUsers.Add(x.ToBaseUser()));

            // Return the items
            return new SourceResponse<BaseUser>(baseUsers, nextToken);
        }

        [JsonObject]
        private class UserListHolder
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
