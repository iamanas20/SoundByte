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
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Services;
using WinRTXamlToolkit.Tools;
using System.Linq;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///     Gets comments for a supplied track
    /// </summary>
    public class CommentModel : ObservableCollection<BaseComment>, ISupportIncrementalLoading
    {
        // The track we want to get comments for
        private BaseTrack _track;

        /// <summary>
        ///     Get comments for a track
        /// </summary>
        /// <param name="track"></param>
        public CommentModel(BaseTrack track)
        {
            _track = track;
        }

        /// <summary>
        ///     The position of the comments, will be 'eol'
        ///     if there are no new tracks
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        ///     Are there more items to load
        /// </summary>
        public bool HasMoreItems => Token != "eol";

        /// <summary>
        ///     Loads stream items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        // ReSharper disable once RedundantAssignment
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // Return a task that will get the items
            return Task.Run(async () =>
            {
                _track = PlaybackService.Instance.CurrentTrack;


                if (_track == null)
                    return new LoadMoreItemsResult {Count = 0};

                if (count <= 10)
                    count = 10;

                if (count >= 50)
                    count = 50;

                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { App.IsLoading = true; });

                try
                {
                    var trackComments = await _track.GetCommentsAsync(count, Token);

                    // Get the comment offset
                    Token = string.IsNullOrEmpty(trackComments.Token) ? "eol" : trackComments.Token;

                    // Loop though all the comments on the UI thread
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() => { trackComments.Comments.ForEach(Add); });

                    // Set the count variable
                    count = (uint)trackComments.Comments.Count();   
                }
                catch (SoundByteException ex)
                {
                    // Exception, most likely did not add any new items
                    count = 0;

                    // Reset the token
                    Token = "eol";

                    // Exception, display error to the user
                    await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                    {
                        await new MessageDialog(ex.ErrorDescription, ex.ErrorTitle).ShowAsync();
                    });
                }

                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { App.IsLoading = false; });

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