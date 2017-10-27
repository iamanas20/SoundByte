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
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;

namespace SoundByte.UWP.Models.SoundCloud
{
    public class SearchSoundCloudPlaylistModel : BaseModel<BasePlaylist>
    {
        /// <summary>
        ///     What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            // If the query is empty, tell the user that they can search something
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
                // Get the searched playlists
                var searchPlaylists = await SoundByteV3Service.Current.GetAsync<SearchPlaylistHolder>(ServiceType.SoundCloud, "/playlists",
                    new Dictionary<string, string>
                    {
                            {"limit", count.ToString()},
                            {"linked_partitioning", "1"},
                            {"offset", Token},
                            {"q", WebUtility.UrlEncode(Query)}
                    });

                // Parse uri for offset
                var param = new QueryParameterCollection(searchPlaylists.NextList);
                var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                // Get the search playlists offset
                Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                // Make sure that there are playlists in the list
                if (searchPlaylists.Playlists.Count > 0)
                {
                    // Set the count variable
                    count = searchPlaylists.Playlists.Count;

                    // Loop though all the search playlists on the UI thread
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        foreach (var playlist in searchPlaylists.Playlists)
                        {
                            Add(playlist.ToBasePlaylist());
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
                    await ShowErrorMessageAsync(resources.GetString("SearchPlaylist_Header"),
                        resources.GetString("SearchPlaylist_Content"));
                }
            }
            catch (SoundByteException ex)
            {
                // Exception, most likely did not add any new items
                count = 0;

                // Reset the token
                Token = "eol";

                // Exception, display error to the user
                await ShowErrorMessageAsync(ex.ErrorTitle, ex.ErrorDescription);
            }

            // Return the result
            return count;
        }
    }
}