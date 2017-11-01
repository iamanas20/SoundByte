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
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Holders;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.UWP.Models.SoundCloud
{
    public class SearchSoundCloudUserModel : BaseModel<BaseUser>
    {
        /// <summary>
        ///     What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

    
        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            if (string.IsNullOrEmpty(Query))
                return 0;

            if (count <= 10)
                count = 10;

            if (count >= 50)
                count = 50;

            // Get the resource loader
            var resources = ResourceLoader.GetForViewIndependentUse();

            try
            {
                // Get the searched users
                var searchUsers = await SoundByteV3Service.Current.GetAsync<UserListHolder>(ServiceType.SoundCloud, "/users",
                    new Dictionary<string, string>
                    {
                        {"limit", count.ToString() },
                        {"linked_partitioning", "1"},
                        {"offset", Token},
                        {"q", WebUtility.UrlEncode(Query)}
                    });

                // Parse uri for offset
                var param = new QueryParameterCollection(searchUsers.NextList);
                var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                // Get the stream cursor
                Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                // Make sure that there are users in the list
                if (searchUsers.Users.Count > 0)
                {
                    // Set the count variable
                    count = searchUsers.Users.Count;

                    // Loop though all the tracks on the UI thread
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        foreach (var user in searchUsers.Users)
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
                    await ShowErrorMessageAsync(resources.GetString("SearchUser_Header"), resources.GetString("SearchUser_Content"));
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

            return count;
        }
    }
}