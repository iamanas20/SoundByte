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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using SoundByte.Core.Items.Track;

namespace SoundByte.UWP.Models
{
    public class BaseTrackModel : ObservableCollection<BaseTrack>, ISupportIncrementalLoading
    {
        public string Token { get; set; }

        protected virtual async Task<int> LoadMoreItemsAsync(int count)
        {
            return await Task.Run(() => 0);
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return Task.Run(async () =>
            {
                var addedCount = await LoadMoreItemsAsync((int) count);
                return new LoadMoreItemsResult {Count = (uint) addedCount};
            }).AsAsyncOperation();
        }

        public bool HasMoreItems => Token != "eol";

        /// <summary>
        ///     Refresh the list by removing any
        ///     existing items and reseting the token.
        /// </summary>
        public async void RefreshItems()
        {
            Token = string.Empty;
            Clear();
            await LoadMoreItemsAsync(1);
        }
    }
}
