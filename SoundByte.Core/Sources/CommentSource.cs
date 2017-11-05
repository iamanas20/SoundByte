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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.Track;

namespace SoundByte.Core.Sources
{
    [UsedImplicitly]
    public class CommentSource : ISource<BaseComment>
    {
        [CanBeNull]
        public BaseTrack Track { get; set; }

        public async Task<SourceResponse<BaseComment>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Call the SoundCloud API and get the items
            var comments = await Track.GetCommentsAsync(count, token).ConfigureAwait(false);

            // If there are no comments
            if (!comments.Comments.Any())
            {
                return new SourceResponse<BaseComment>(null, null, false, "No comments", "This track has no comments");
            }

            // Return the items
            return new SourceResponse<BaseComment>(comments.Comments, comments.Token;
        }
    }
}
