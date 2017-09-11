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
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    public class FanburstSearchModel : BaseTrackModel
    {
        /// <summary>
        ///     What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            // Return a task that will get the items
            return await Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(Query))
                    return 0;

                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("FanburstSearchModelInfoPane") as InfoPane)?.ShowLoading();
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                try
                {
                    // Search for matching tracks
                    var searchTracks = await SoundByteV3Service.Current.GetAsync<List<FanburstTrack>>(
                        ServiceType.Fanburst, "tracks/search", new Dictionary<string, string>
                        {
                            {"query", WebUtility.UrlEncode(Query)},
                            {"per_page", count.ToString()}
                        });

                    // Parse uri for offset
                    //   var param = new QueryParameterCollection(searchTracks.NextList);
                    var offset = "eol"; //param.FirstOrDefault(x => x.Key == "offset").Value;

                    // Get the search offset
                    Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                    // Make sure that there are tracks in the list
                    if (searchTracks.Count > 0)
                    {
                        // Set the count variable
                        count = searchTracks.Count;

                        // Loop though all the tracks on the UI thread
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            foreach (var item in searchTracks)
                                Add(item.ToBaseTrack());
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
                            (App.CurrentFrame?.FindName("FanburstSearchModelInfoPane") as InfoPane)?.ShowMessage(
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
                        (App.CurrentFrame?.FindName("FanburstSearchModelInfoPane") as InfoPane)?.ShowMessage(
                            ex.ErrorTitle, ex.ErrorDescription, ex.ErrorGlyph);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("FanburstSearchModelInfoPane") as InfoPane)?.ClosePane();
                });

                // Return the result
                return count;
            });
        }
    }
}