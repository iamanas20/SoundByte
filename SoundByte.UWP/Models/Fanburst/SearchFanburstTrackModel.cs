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

namespace SoundByte.UWP.Models.Fanburst
{
    public class SearchFanburstTrackModel : BaseModel<BaseTrack>
    {
        /// <summary>
        ///     What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            if (string.IsNullOrEmpty(Query))
                return 0;

            // Get the resource loader
            var resources = ResourceLoader.GetForViewIndependentUse();

            if (count <= 10)
                count = 10;

            if (count >= 50)
                count = 50;

            try
            {
                // Search for matching tracks
                var searchTracks = await SoundByteV3Service.Current.GetAsync<List<FanburstTrack>>(
                    ServiceType.Fanburst, "tracks/search", new Dictionary<string, string>
                    {
                            {"query", WebUtility.UrlEncode(Query)},
                            {"per_page", count.ToString()}
                    });

                // Get the search offset
                Token = "eol";

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

                    // No item message
                    await ShowErrorMessageAsync(resources.GetString("SearchTrack_Header"), resources.GetString("SearchTrack_Content"));
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


            // Return the result
            return count;
        }
    }
}