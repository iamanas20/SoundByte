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

using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///     Gets comments for a supplied track
    /// </summary>
    public class CommentModel : BaseModel<BaseComment>
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
        ///     Loads stream items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        // ReSharper disable once RedundantAssignment
        protected override async Task<int> LoadMoreItemsAsync(int count)
        {
            _track = PlaybackService.Instance.CurrentTrack;

            if (_track == null)
                return 0;

            if (count <= 10)
                count = 10;

            if (count >= 50)
                count = 50;

            try
            {
                var trackComments = await _track.GetCommentsAsync(count, Token);

                // Get the comment offset
                Token = string.IsNullOrEmpty(trackComments.Token) ? "eol" : trackComments.Token;

                // Loop though all the comments on the UI thread
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { trackComments.Comments.ForEach(Add); });

                // Set the count variable
                count = trackComments.Comments.Count;
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