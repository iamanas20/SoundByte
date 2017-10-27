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
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Holders;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///     Gets the logged in users playlists and playlist likes
    /// </summary>
    public class UserPlaylistModel : ObservableCollection<BasePlaylist>, ISupportIncrementalLoading
    {
        /// <summary>
        ///     The position of the track, will be 'eol'
        ///     if there are no new tracks
        /// </summary>
        public string Token { get; protected set; }

        /// <summary>
        ///     Are there more items to load
        /// </summary>
        public bool HasMoreItems => Token != "eol";

        /// <summary>
        ///     Loads stream items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        // ReSharper disable once RedundantAssignment
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // Return a task that will get the items
            return Task.Run(async () =>
            {
                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("UserPlaylistModelInfoPane") as InfoPane)?.ShowLoading();
                });

                if (count <= 10)
                    count = 10;

                if (count >= 50)
                    count = 50;

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                // Check if the user is not logged in
                if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud))
                {
                    try
                    {
                        // Get the users playlists using the V2 API
                        var userPlaylists = await SoundByteV3Service.Current.GetAsync<PlaylistHolder>(ServiceType.SoundCloudV2,
                            $"/users/{SoundByteV3Service.Current.GetConnectedUser(ServiceType.SoundCloud)?.Id}/playlists/liked_and_owned",
                            new Dictionary<string, string>
                            {
                                {"limit", "50"},
                                {"offset", Token},
                                {"linked_partitioning", "1"}
                            });

                        // Parse uri for offset
                        var param = new QueryParameterCollection(userPlaylists.NextList);
                        var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                        // Get the playlist cursor
                        Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                        // Make sure that there are tracks in the list
                        if (userPlaylists.Playlists.Count > 0)
                        {
                            // Set the count variable
                            count = (uint) userPlaylists.Playlists.Count;

                            // Loop though all the playlists on the UI thread
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                userPlaylists.Playlists.ForEach(p => Add(p.Playlist.ToBasePlaylist()));
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
                                (App.CurrentFrame?.FindName("UserPlaylistModelInfoPane") as InfoPane)?.ShowMessage(
                                    resources.GetString("UserPlaylists_Header"),
                                    resources.GetString("UserPlaylists_Content"), false);
                            });
                        }
                    }
                    catch (SoundByteException ex)
                    {
                        // Exception, most likely did not add any new items
                        count = 0;

                        // Exception, display error to the user
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            (App.CurrentFrame?.FindName("UserPlaylistModelInfoPane") as InfoPane)?.ShowMessage(
                                ex.ErrorTitle, ex.ErrorDescription);
                        });
                    }
                }
                else
                {
                    // Not logged in, so no new items
                    count = 0;

                    // Reset the token
                    Token = "eol";

                    // No items tell the user
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        (App.CurrentFrame?.FindName("UserPlaylistModelInfoPane") as InfoPane)?.ShowMessage(
                            resources.GetString("ErrorControl_LoginFalse_Header"),
                            resources.GetString("ErrorControl_LoginFalse_Content"), false);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("UserPlaylistModelInfoPane") as InfoPane)?.ClosePane();
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