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
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.UWP.Models.Fanburst
{
    public class FanburstExploreModel : BaseModel<BaseTrack>
    {
        // ReSharper disable once RedundantAssignment
        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            try
            {
                // Search for matching tracks
                var searchTracks = await SoundByteV3Service.Current.GetAsync<List<FanburstTrack>>(
                    ServiceType.Fanburst, "tracks/trending");

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
                    await ShowErrorMessageAsync("Fanburst", "No Content");
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
