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
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Holders;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;

namespace SoundByte.UWP.Models
{
    public class TrackModel : BaseModel<BaseTrack>
    {
        /// <summary>
        ///     Setsup a new view model for playlists
        /// </summary>
        /// <param name="user">The user to retrieve playlists for</param>
        public TrackModel(BaseUser user)
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
            if (count <= 10)
                count = 10;

            if (count >= 50)
                count = 50;

            try
            {
                // Get the users track
                var userTracks = await SoundByteV3Service.Current.GetAsync<TrackListHolder>(ServiceType.SoundCloud,
                    $"/users/{User.Id}/tracks", new Dictionary<string, string>
                    {
                            {"limit", count.ToString()},
                            {"offset", Token},
                            {"linked_partitioning", "1"}
                    });

                // Parse uri for offset
                var param = new QueryParameterCollection(userTracks.NextList);
                var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                // Get the track cursor
                Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                // Make sure that there are tracks in the list
                if (userTracks.Tracks.Count > 0)
                {
                    // Set the count variable
                    count = userTracks.Tracks.Count;

                    // Loop though all the tracks on the UI thread
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        foreach (var track in userTracks.Tracks)
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

                    await ShowErrorMessageAsync("Nothing to hear here", "Follow " + User.Username + " for updates on sounds they share in the future.");
                }
            }
            catch (SoundByteException ex)
            {
                // Exception, most likely did not add any new items
                count = 0;

                // Exception, display error to the user
                await ShowErrorMessageAsync(ex.ErrorTitle, ex.ErrorDescription);
            }

            // Return the result
            return count;
        }
    }
}