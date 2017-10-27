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
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///     Model for user likes
    /// </summary>
    public class LikeModel : BaseModel<BaseTrack>
    {
        /// <summary>
        ///     Setsup the like view model for a user
        /// </summary>
        /// <param name="user">The user to retrieve likes for</param>
        public LikeModel(BaseUser user)
        {
            User = user;
        }
        // User object that we will used to get the likes for

        public BaseUser User { get; set; }

        /// <summary>
        ///     Loads stream items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        // ReSharper disable once RedundantAssignment
        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            // Return a task that will get the items
            return await Task.Run(async () =>
            {
                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("LikeModelInfoPane") as InfoPane)?.ShowLoading();
                });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                // Check if the user is not logged in
                if (User != null)
                {
                    try
                    {
                        if (count <= 10)
                            count = 10;

                        if (count >= 50)
                            count = 50;

                        // Get the like tracks
                        var likeTracks = await SoundByteV3Service.Current.GetAsync<TrackListHolder>(
                            ServiceType.SoundCloud,
                            $"/users/{User.Id}/favorites", new Dictionary<string, string>
                            {
                                {"limit", count.ToString()},
                                {"cursor", Token},
                                {"linked_partitioning", "1"}
                            });

                        // Parse uri for offset
                        var param = new QueryParameterCollection(likeTracks.NextList);
                        var cursor = param.FirstOrDefault(x => x.Key == "cursor").Value;

                        // Get the likes cursor
                        Token = string.IsNullOrEmpty(cursor) ? "eol" : cursor;

                        // Make sure that there are tracks in the list
                        if (likeTracks.Tracks.Count > 0)
                        {
                            // Set the count variable
                            count = likeTracks.Tracks.Count;

                            // Loop though all the tracks on the UI thread
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                foreach (var track in likeTracks.Tracks)
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
                                (App.CurrentFrame?.FindName("LikeModelInfoPane") as InfoPane)?.ShowMessage(
                                    resources.GetString("LikeTracks_Header"),
                                    resources.GetString("LikeTracks_Content"), false);
                            });
                        }
                    }
                    catch (SoundByteException ex)
                    {
                        // Exception, most likely did not add any new items
                        count = 0;

                        // Exception, display error to the user
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            (App.CurrentFrame?.FindName("LikeModelInfoPane") as InfoPane)?.ShowMessage(
                                ex.ErrorTitle, ex.ErrorDescription);
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
                        (App.CurrentFrame?.FindName("LikeModelInfoPane") as InfoPane)?.ShowMessage(
                            resources.GetString("ErrorControl_LoginFalse_Header"),
                            resources.GetString("ErrorControl_LoginFalse_Content"), false);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("LikeModelInfoPane") as InfoPane)?.ClosePane();
                });

                // Return the result
                return count;
            });
        }
    }
}