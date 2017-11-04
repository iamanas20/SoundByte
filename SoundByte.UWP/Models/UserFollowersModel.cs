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
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Holders;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.UWP.Models
{
    public class UserFollowersModel : BaseModel<BaseUser>
    {
        public UserFollowersModel(BaseUser user, string type)
        {
            User = user;
            Type = type;
        }

        /// <summary>
        ///     User object that we will used to get the follower / followings for
        /// </summary>
        public BaseUser User { get; set; }

        /// <summary>
        ///     What type of object is this (followers, followings)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Loads stream items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        // ReSharper disable once RedundantAssignment
        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            // Get the resource loader
            var resources = ResourceLoader.GetForViewIndependentUse();

            if (count <= 10)
                count = 10;

            if (count >= 50)
                count = 50;

            try
            {
                // Get the users playlists using the V2 API
                var userPlaylists = await SoundByteV3Service.Current.GetAsync<UserListHolder>(ServiceType.SoundCloud,
                    $"/users/{User.Id}/{Type}", new Dictionary<string, string>
                    {
                            {"limit", count.ToString()},
                            {"cursor", Token},
                            {"linked_partitioning", "1"}
                    });

                // Parse uri for offset
                var param = new QueryParameterCollection(userPlaylists.NextList);
                var offset = param.FirstOrDefault(x => x.Key == "cursor").Value;

                // Get the playlist cursor
                Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                // Make sure that there are tracks in the list
                if (userPlaylists.Users.Count > 0)
                {
                    // Set the count variable
                    count = userPlaylists.Users.Count;

                    // Loop though all the playlists on the UI thread
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        foreach (var user in userPlaylists.Users)
                        {
                            Add(user.ToBaseUser());
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
                    await ShowErrorMessageAsync(resources.GetString("UserPlaylists_Header"), resources.GetString("UserPlaylists_Content"));
                }
            }
            catch (SoundByteException ex)
            {
                // Exception, most likely did not add any new items
                count = 0;

                // Exception, display error to the user
                await ShowErrorMessageAsync(ex.ErrorTitle, ex.ErrorDescription);
            }

            return count;
        }
    }
}