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
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Models.Search
{
    public class SearchSoundCloudTrackModel : BaseModel<BaseTrack>
    {
        /// <summary>
        ///     What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        ///     Loads search track items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            if (string.IsNullOrEmpty(Query))
                return 0;

            // Get the resource loader
            var resources = ResourceLoader.GetForViewIndependentUse();

            // At least 10 tracks at once
            if (count < 10)
                count = 10;

            try
            {
                // Search for matching tracks
                var searchTracks = await SoundCloudTrack.SearchAsync(Query, (uint)count, Token);

                // Get the search offset
                Token = string.IsNullOrEmpty(searchTracks.Token) ? "eol" : searchTracks.Token;

                // Make sure that there are tracks in the list
                if (searchTracks.Tracks.Count() > 0)
                {
                    // Set the count variable
                    count = searchTracks.Tracks.Count();

                    // Loop though all the tracks on the UI thread
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        foreach (var track in searchTracks.Tracks)
                        {
                            Add(track);
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