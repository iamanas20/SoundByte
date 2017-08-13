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
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using SoundByte.Core.Services;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    ///     This class contains helper functions for creating
    ///     and managing jumplists
    /// </summary>
    public static class JumplistHelper
    {
        // The Systems jumplist
        private static JumpList _systemJumpList;

        /// <summary>
        ///     Adds a recent item to the jumplist
        /// </summary>
        /// <param name="args">Arguments to pass when the app opens</param>
        /// <param name="name">Name of the item</param>
        /// <param name="description">Hover description</param>
        /// <param name="grp">Grouping</param>
        /// <param name="image">Image to display</param>
        public static async Task AddRecentAsync(string args, string name, string description, string grp, Uri image)
        {
            try
            {
                // Check if jumplists are supported
                if (!JumpList.IsSupported()) return;

                // Load the jumplist
                _systemJumpList = await JumpList.LoadCurrentAsync();

                // Change the kind to recent items
                _systemJumpList.SystemGroupKind = JumpListSystemGroupKind.Recent;

                // Check that the item is not already added
                if (_systemJumpList.Items.FirstOrDefault(x => x.Arguments == args) != null) return;

                // Loop through all the items and remove any items that will cause the jumplist
                // to go over 5 items (we only want 5 recent items max).
                while (_systemJumpList.Items.Count(x => x.GroupName == grp) >= 5)
                {
                    // Get a item
                    var recentItem = _systemJumpList.Items.FirstOrDefault(x => x.GroupName == grp);
                    // Check that the item is not null
                    if (recentItem != null)
                        _systemJumpList.Items.Remove(recentItem);
                }

                // Create a new jumplist item
                var item = JumpListItem.CreateWithArguments(args, name);
                item.Description = description;
                item.GroupName = grp;
                item.Logo = image;

                // Add the item to the jumplist
                _systemJumpList.Items.Add(item);

                // Save the jumplist
                await _systemJumpList.SaveAsync();
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.TrackException(ex);
            }
        }

        /// <summary>
        ///     Removes all jumplist items
        /// </summary>
        public static async Task RemoveAllAsync()
        {
            // Check if jumplists are supported
            if (!JumpList.IsSupported()) return;
            // Load the jumplist Items
            _systemJumpList = await JumpList.LoadCurrentAsync();
            // Clear all jumplist items
            _systemJumpList.Items.Clear();
            // Save the Jumplist Items
            await _systemJumpList.SaveAsync();
        }
    }
}