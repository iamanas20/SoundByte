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
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Services;

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

        private async Task RunYouTubeLogic(uint count)
        {
            // Get the comments
            var comments = await SoundByteService.Instance.GetAsync<YouTubeCommentHolder>(ServiceType.YouTube, "commentThreads", new Dictionary<string, string>
            {
                { "maxResults", "50"},
                { "part", "snippet"},
                { "videoId", _track.Id},
                { "pageToken", Token }
            });

            var offset = comments.NextPageToken;

            // Get the comment offset
            Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

            // Make sure that there are comments in the list
            if (comments.Items.Count > 0)
            {
                // Set the count variable
                count = (uint)comments.Items.Count;

                foreach (var comment in comments.Items)
                {
                    // Loop though all the comments on the UI thread
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        Add(comment.ToBaseComment());
                    });
                }   
            }
            else
            {
                // There are no items, so we added no items
                count = 0;

                // Reset the token
                Token = "eol";

                // No items tell the user
                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await new MessageDialog("Be the first to post a comment.", "No Comments").ShowAsync();
                });
            }
        }

        private async Task RunSoundCloudLogic(uint count)
        {
            // Get the comments
            var comments = await SoundByteService.Instance.GetAsync<CommentListHolder>(ServiceType.SoundCloud,
                string.Format("/tracks/{0}/comments", _track.Id), new Dictionary<string, string>
                {
                    {"limit", "50"},
                    {"cursor", Token},
                    {"linked_partitioning", "1"}
                });

            // Parse uri for offset
            var param = new QueryParameterCollection(comments.NextList);
            var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Get the comment offset
            Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

            // Make sure that there are comments in the list
            if (comments.Items.Count > 0)
            {
                // Set the count variable
                count = (uint)comments.Items.Count;

                // Loop though all the comments on the UI thread
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { comments.Items.ForEach(x => Add(x.ToBaseComment())); });
            }
            else
            {
                // There are no items, so we added no items
                count = 0;

                // Reset the token
                Token = "eol";

                // No items tell the user
                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await new MessageDialog("Be the first to post a comment.", "No Comments").ShowAsync();
                });
            }
        }


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

                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { App.IsLoading = true; });

                try
                {
                    switch (_track.ServiceType)
                    {
                        case ServiceType.SoundCloud:
                            await RunSoundCloudLogic(count);
                            break;
                        case ServiceType.YouTube:
                            await RunYouTubeLogic(count);
                            break;

                    }

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

        [JsonObject]
        public class YouTubeCommentHolder
        {
            [JsonProperty("nextPageToken")]
            public string NextPageToken { get; set; }

            [JsonProperty("items")]
            public List<YouTubeComment> Items { get; set; }
        }

        [JsonObject]
        public class CommentListHolder
        {
            /// <summary>
            ///     List of comments
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudComment> Items { get; set; }

            /// <summary>
            ///     Next items in the list
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}