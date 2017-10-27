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
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Holders;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///     Model for the soundcloud charts
    /// </summary>
    public class ChartModel : BaseModel<BaseTrack>
    {
        // The genre to search for
        private string _genre = "all-music";

        // The kind to search for
        private string _kind = "top";

        /// <summary>
        ///     The genre to search for.
        ///     Note: By changing this variable will update
        ///     the model.
        /// </summary>
        public string Genre
        {
            get => _genre;
            set
            {
                _genre = value;
                RefreshItems();
            }
        }

        /// <summary>
        ///     The kind of item to search for
        ///     Note: By changing this variable it will
        ///     update the model.
        /// </summary>
        public string Kind
        {
            get => _kind;
            set
            {
                _kind = value;
                RefreshItems();
            }
        }

        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            return await Task.Run(async () =>
            {
                if (count <= 10)
                    count = 10;

                if (count >= 50)
                    count = 50;

                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("ChartModelInfoPane") as InfoPane)?.ShowLoading();
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                try
                {
                    // Get the trending tracks
                    var exploreTracks = await SoundByteV3Service.Current.GetAsync<ExploreTrackHolder>(ServiceType.SoundCloudV2, "/charts",
                        new Dictionary<string, string>
                        {
                            {"genre", "soundcloud%3Agenres%3A" + _genre},
                            {"kind", _kind},
                            {"limit", "50"},
                            {"offset", Token},
                            {"linked_partitioning", "1"}
                        });

                    // Parse uri for offset
                    var param = new QueryParameterCollection(exploreTracks.NextList);
                    var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                    // Get the stream offset
                    Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                    // Make sure that there are tracks in the list
                    if (exploreTracks.Items.Count > 0)
                    {
                        // Set the count variable
                        count = exploreTracks.Items.Count;

                        // Loop though all the tracks on the UI thread
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            exploreTracks.Items.ForEach(t => Add(t.Track.ToBaseTrack()));
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
                            (App.CurrentFrame?.FindName("ChartModelInfoPane") as InfoPane)?.ShowMessage(
                                resources.GetString("ExploreTracks_Header"),
                                resources.GetString("ExploreTracks_Content"), false);
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
                        (App.CurrentFrame?.FindName("ChartModelInfoPane") as InfoPane)?.ShowMessage(ex.ErrorTitle,
                            ex.ErrorDescription);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("ChartModelInfoPane") as InfoPane)?.ClosePane();
                });

                // Return the result
                return count;
            });
        }
    }
}