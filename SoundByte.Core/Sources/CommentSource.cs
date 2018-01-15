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
            if (Track == null)
            {
                return new SourceResponse<BaseComment>(null, null, false, "No comments", "This track has no comments");
            }

            // Call the SoundCloud API and get the items
            var comments = await Track.GetCommentsAsync(count, token).ConfigureAwait(false);

            // If there are no comments
            if (!comments.Comments.Any())
            {
                return new SourceResponse<BaseComment>(null, null, false, "No comments", "This track has no comments");
            }

            // Return the items
            return new SourceResponse<BaseComment>(comments.Comments, comments.Token);
        }
    }
}