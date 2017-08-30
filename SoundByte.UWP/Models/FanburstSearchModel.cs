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
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.API.Endpoints;
using SoundByte.API.Exceptions;
using SoundByte.Core.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    public class FanburstSearchModel : ObservableCollection<Track>, ISupportIncrementalLoading
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

        // --- TEMP BECAUSE WE CANNOT USE DYNAMIC IN RELEASE MODE ---- //
        public class FBImages
        {
            public string square_150 { get; set; }
            public string square_250 { get; set; }
            public string square_500 { get; set; }
        }

        public class FBUser
        {
            public string id { get; set; }
            public string name { get; set; }
            public string permalink { get; set; }
            public string url { get; set; }
            public string avatar_url { get; set; }
            public string location { get; set; }
        }

        public class FBRootObject
        {
            public string id { get; set; }
            public string title { get; set; }
            public string permalink { get; set; }
            public int duration { get; set; }
            public string url { get; set; }
            public string published_at { get; set; }
            public bool @private { get; set; }
            public bool downloadable { get; set; }
            public string image_url { get; set; }
            public FBImages images { get; set; }
            public string stream_url { get; set; }
            public FBUser user { get; set; }
        }


        /// <summary>
        ///     Loads search track items from the souncloud api
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
                    (App.CurrentFrame?.FindName("FanburstSearchModelInfoPane") as InfoPane)?.ShowLoading();
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                try
                {
                    // Search for matching tracks
                    var searchTracks = await SoundByteService.Instance.GetAsync<List<FBRootObject>>(
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
                        count = (uint) searchTracks.Count;

                        // Loop though all the tracks on the UI thread
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            foreach (var item in searchTracks)
                                Add(new Track
                                {
                                    ServiceType = ServiceType.Fanburst,
                                    Id = item.id,
                                    Title = item.title,
                                    PermalinkUri = item.permalink,
                                    Duration = TimeSpan.FromSeconds(item.duration).TotalMilliseconds,
                                    CreationDate = DateTime.Parse(item.published_at),
                                    Kind = "track",
                                    User = new User
                                    {
                                        Id = item.user.id,
                                        Username = item.user.name,
                                        Country = item.user.location,
                                        ArtworkLink = item.user.avatar_url
                                    },
                                    ArtworkLink = item.images.square_500
                                });
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