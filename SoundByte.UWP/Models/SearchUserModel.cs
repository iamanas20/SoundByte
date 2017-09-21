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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Holders;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.UWP.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    public class SearchUserModel : ObservableCollection<BaseUser>, ISupportIncrementalLoading
    {
        /// <summary>
        ///     The position of the track, will be 'eol'
        ///     if there are no new trackss
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        ///     What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        ///     Are there more items to load
        /// </summary>
        public bool HasMoreItems => Token != "eol";

        /// <summary>
        ///     Loads search user items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // Return a task that will get the items
            return Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(Query))
                    return new LoadMoreItemsResult {Count = 0};

                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("SearchUserModelInfoPane") as InfoPane)?.ShowLoading();
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                try
                {
                    // Get the searched users
                    var searchUsers = await SoundByteV3Service.Current.GetAsync<UserListHolder>(ServiceType.SoundCloud,"/users",
                        new Dictionary<string, string>
                        {
                            {"limit", SettingsService.TrackLimitor.ToString()},
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
                        count = (uint) searchUsers.Users.Count;

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
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            (App.CurrentFrame?.FindName("SearchUserModelInfoPane") as InfoPane)?.ShowMessage(
                                resources.GetString("SearchUser_Header"), resources.GetString("SearchUser_Content"), false);
                        });
                    }
                }
                catch (SoundByteException ex)
                {
                    // Exception, most likely did not add any new items
                    count = 0;

                    // Reset the token
                    Token = "eol";

                    // Exception, display error to the user
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        (App.CurrentFrame?.FindName("SearchUserModelInfoPane") as InfoPane)?.ShowMessage(
                            ex.ErrorTitle, ex.ErrorDescription);
                    });
                }


                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("SearchUserModelInfoPane") as InfoPane)?.ClosePane();
                });

                // Return the result
                return new LoadMoreItemsResult {Count = count};
            }).AsAsyncOperation();
        }

        /// <summary>
        ///     Refresh the list by removing any
        ///     existing items and reseting the token.
        /// </summary>
        public void RefreshItems()
        {
            Token = null;
            Clear();
        }
    }
}