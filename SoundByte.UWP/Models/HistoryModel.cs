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
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.UWP.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///     Model for the users play history
    /// </summary>
    public class HistoryModel : ObservableCollection<BaseTrack>, ISupportIncrementalLoading
    {
        /// <summary>
        ///     The position of the track, will be 'eol'
        ///     if there are no new tracks
        /// </summary>
        public string Token { get; private set; }

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
                    (App.CurrentFrame?.FindName("HistoryModelInfoPane") as InfoPane)?.ShowLoading();
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                // Check if the user is logged in
                if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud))
                {
                    try
                    {
                        var userPlayHistory = await SoundByteService.Instance.GetAsync<HistoryListHolder>(
                            "/me/play-history/tracks", new Dictionary<string, string>
                            {
                                {"limit", "50"},
                                {"offset", Token},
                                {"linked_partitioning", "1"}
                            }, true);

                        // Parse uri for offset
                        var param = new QueryParameterCollection(userPlayHistory.NextList);
                        var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                        // Get the history offset
                        Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                        // Make sure that there are tracks in the list
                        if (userPlayHistory.Tracks.Count > 0)
                        {
                            // Set the count variable
                            count = (uint) userPlayHistory.Tracks.Count;

                            // Loop though all the tracks on the UI thread
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                userPlayHistory.Tracks.ForEach(t => Add(t.Track.ToBaseTrack()));
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
                                (App.CurrentFrame?.FindName("HistoryModelInfoPane") as InfoPane)?.ShowMessage(
                                    "No History", "Listen to some music to get started.", "", false);
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
                            (App.CurrentFrame?.FindName("HistoryModelInfoPane") as InfoPane)?.ShowMessage(
                                ex.ErrorTitle, ex.ErrorDescription, ex.ErrorGlyph);
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
                        (App.CurrentFrame?.FindName("HistoryModelInfoPane") as InfoPane)?.ShowMessage(
                            resources.GetString("ErrorControl_LoginFalse_Header"),
                            resources.GetString("ErrorControl_LoginFalse_Content"), "", false);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("HistoryModelInfoPane") as InfoPane)?.ClosePane();
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