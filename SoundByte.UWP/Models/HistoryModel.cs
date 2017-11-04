﻿/* |----------------------------------------------------------------|
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
using SoundByte.Core.Items.Track;
using SoundByte.UWP.DatabaseContexts;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///     Model for the users play history
    /// </summary>
    public class HistoryModel : BaseModel<BaseTrack>
    {
        private bool _firstTime = true;

        /// <summary>
        ///     Loads stream items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        // ReSharper disable once RedundantAssignment
        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            try
            {
                using (var db = new HistoryContext())
                {
                    if (_firstTime)
                        db.Database.Migrate();

                    _firstTime = false;

                    if (count <= 10)
                        count = 10;

                    if (count >= 50)
                        count = 50;

                    // Set the default token to zero
                    if (string.IsNullOrEmpty(Token))
                        Token = "0";

                    if (Token == "eol")
                        Token = "0";

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

                        if (Count == 0)
                        {
                            // No items tell the user
                            await ShowErrorMessageAsync("No History", "Listen to some music to get started.");
                        }

                    }

                    // Set the new token;
                    if (Token != "eol")
                        Token = (int.Parse(Token) + count).ToString();
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