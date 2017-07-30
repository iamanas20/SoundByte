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

using SoundByte.Core.API.Holders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///  Model for the users stream
    /// </summary>
    public class StreamModel : ObservableCollection<StreamItem>, ISupportIncrementalLoading
    {
        /// <summary>
        /// The position of the track, will be 'eol'
        /// if there are no new tracks
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Are there more items to load
        /// </summary>
        public bool HasMoreItems => Token != "eol";

        /// <summary>
        /// Refresh the list by removing any
        /// existing items and reseting the token.
        /// </summary>
        public void RefreshItems()
        {
            Token = null;
            Clear();
        }

        /// <summary>
        /// Loads stream items from the souncloud api
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
                    (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ShowLoading();    
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                // Check if the user is not logged in
                if (SoundByteService.Current.IsSoundCloudAccountConnected)
                {
                    try
                    {
                        // Get items from the users stream
                        var streamTracks = await SoundByteService.Current.GetAsync<StreamTrackHolder>("/e1/me/stream", new Dictionary<string, string>
                        {
                            { "limit", "50" },
                            { "cursor", Token }
                        });

                        // Parse uri for offset
                        var param = new QueryParameterCollection(streamTracks.NextList);
                        var cursor = param.FirstOrDefault(x => x.Key == "cursor").Value;

                        // Get the stream cursor
                        Token = string.IsNullOrEmpty(cursor) ? "eol" : cursor;

                        // Make sure that there are tracks in the list
                        if (streamTracks.Items.Count > 0)
                        {
                            // Set the count variable
                            count = (uint)streamTracks.Items.Count;

                            // Loop though all the tracks on the UI thread
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                streamTracks.Items.ForEach(Add);
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
                                (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ShowMessage(resources.GetString("StreamTracks_Header"), resources.GetString("StreamTracks_Content"), "", false);
                            });
                        }
                    }
                    catch (Core.API.Exceptions.SoundByteException ex)
                    {
                        // Exception, most likely did not add any new items
                        count = 0;

                        // Reset the token
                        Token = "eol";

                        // Exception, display error to the user
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ShowMessage(ex.ErrorTitle, ex.ErrorDescription, ex.ErrorGlyph);
                        });
                    }
                }
                else
                {
                    // Not logged in, so no new items
                    count = 0;

                    // Reset the token
                    Token = "eol";

                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ShowMessage(resources.GetString("ErrorControl_LoginFalse_Header"), resources.GetString("ErrorControl_LoginFalse_Content"), "", false);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("StreamModelInfoPane") as InfoPane)?.ClosePane();
                });

                // Return the result
                return new LoadMoreItemsResult { Count = count };
            }).AsAsyncOperation();
        }
    }
}
