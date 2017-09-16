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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.Exceptions;
using SoundByte.UWP.DatabaseContexts;
using SoundByte.UWP.UserControls;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///     Model for the users play history
    /// </summary>
    public class HistoryModel : BaseTrackModel
    {
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
                    (App.CurrentFrame?.FindName("HistoryModelInfoPane") as InfoPane)?.ShowLoading();
                });

                try
                {
                    using (var db = new HistoryContext())
                    {
                        // At least 10 items
                        if (count <= 10)
                            count = 10;

                        // Set the default token to zero
                        if (string.IsNullOrEmpty(Token))
                            Token = "-" + count;

                        // Set the new token;
                        Token = (int.Parse(Token) + count).ToString();

                        // Get items in date descending order, skip the token and take the count
                        var items = db.Tracks.Include(x => x.User).OrderByDescending(x => x.LastPlaybackDate).Skip(int.Parse(Token)).Take(count);

                        if (items.Any())
                        {
                            // Set the count variable
                            count = items.Count();

                            // Loop though all the tracks on the UI thread
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                foreach (var track in items)
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
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                (App.CurrentFrame?.FindName("HistoryModelInfoPane") as InfoPane)?.ShowMessage(
                                    "No History", "Listen to some music to get started.", "", false);
                            });
                        }
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
                        (App.CurrentFrame?.FindName("HistoryModelInfoPane") as InfoPane)?.ShowMessage(
                            ex.ErrorTitle, ex.ErrorDescription, ex.ErrorGlyph);
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (App.CurrentFrame?.FindName("HistoryModelInfoPane") as InfoPane)?.ClosePane();
                });

                // Return the result
                return count;
            });
        }
    }
}