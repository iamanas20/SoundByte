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
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.API.Endpoints;
using SoundByte.API.Exceptions;
using SoundByte.API.Holders;
using SoundByte.Core.Services;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///     Gets comments for a supplied track
    /// </summary>
    public class CommentModel : ObservableCollection<Comment>, ISupportIncrementalLoading
    {
        // The track we want to get comments for
        private Track _track;

        /// <summary>
        ///     Get comments for a track
        /// </summary>
        /// <param name="track"></param>
        public CommentModel(Track track)
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
            var comments = await SoundByteService.Instance.GetAsync<dynamic>(ServiceType.YouTube, "commentThreads", new Dictionary<string, string>
            {
                { "maxResults", "50"},
                { "part", "snippet"},
                { "videoId", _track.Id},
                { "pageToken", Token }
            });

            var offset = (string)comments.nextPageToken;

            // Get the comment offset
            Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

            // Make sure that there are comments in the list
            if (comments.items.Count > 0)
            {
                // Set the count variable
                count = (uint)comments.items.Count;

                foreach (var comment in comments.items)
                {
                    // Loop though all the comments on the UI thread
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        Add(new Comment
                        {
                            Id = comment.id,
                            Body = comment.snippet.topLevelComment.snippet.textDisplay,
                            CreatedAt = comment.snippet.topLevelComment.snippet.publishedAt,
                            Timestamp = "0",
                            Track = _track,
                            User = new User
                            {
                                Id = "",
                                ArtworkLink = comment.snippet.topLevelComment.snippet.authorProfileImageUrl,
                                Username = comment.snippet.topLevelComment.snippet.authorDisplayName
                            }            
                        });
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
            var comments = await SoundByteService.Instance.GetAsync<CommentListHolder>(
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
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { comments.Items.ForEach(Add); });
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
    }
}