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
using SoundByte.Core.Services;
using SoundByte.UWP.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    public class SearchTrackModel : BaseTrackModel
    {
        /// <summary>
        ///     What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        ///     Filter the search
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        ///     Loads search track items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            // Return a task that will get the items
            return await Task.Run(async () =>
            {
                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("SearchTrackModelInfoPane") as InfoPane)?.ShowLoading();
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                try
                {
                    // Search for matching tracks
                    var searchTracks = await SoundByteV3Service.Current.GetAsync<TrackListHolder>(ServiceType.SoundCloud,  "/tracks",
                        new Dictionary<string, string>
                        {
                            {"limit", SettingsService.TrackLimitor.ToString()},
                            {"linked_partitioning", "1"},
                            {"offset", Token},
                            {"q", WebUtility.UrlEncode(Query) + "&" + Filter}
                        });

                    // Parse uri for offset
                    var param = new QueryParameterCollection(searchTracks.NextList);
                    var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                    // Get the search offset
                    Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                    // Make sure that there are tracks in the list
                    if (searchTracks.Tracks.Count > 0)
                    {
                        // Set the count variable
                        count = searchTracks.Tracks.Count;

                        // Loop though all the tracks on the UI thread
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            foreach (var track in searchTracks.Tracks)
                            {
                                Add(track.ToBaseTrack());
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
                            (App.CurrentFrame?.FindName("SearchTrackModelInfoPane") as InfoPane)?.ShowMessage(
                                resources.GetString("SearchTrack_Header"),
                                resources.GetString("SearchTrack_Content"), "", false);
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
                        (App.CurrentFrame?.FindName("SearchTrackModelInfoPane") as InfoPane)?.ShowMessage(
                            ex.ErrorTitle, ex.ErrorDescription, ex.ErrorGlyph);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("SearchTrackModelInfoPane") as InfoPane)?.ClosePane();
                });

                // Return the result
                return count;
            });
        }
    }
}