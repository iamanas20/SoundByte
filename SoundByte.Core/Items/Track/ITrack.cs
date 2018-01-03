/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
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
using System.Threading;

namespace SoundByte.Core.Items.Track
{
    /// <summary>
    /// Extend custom service track classes
    /// off of this interface.
    /// </summary>
    public interface ITrack
    {
        /// <summary>
        /// Convert the service specific track implementation to a
        /// universal implementation. Overide this method and provide
        /// the correct mapping between the service specific and universal
        /// classes.
        /// </summary>
        /// <returns>A base track item.</returns>
        BaseTrack ToBaseTrack();

        /// <summary>
        /// Gets a list of base comments for this track.
        /// </summary>
        /// <param name="count">The amount of comments to get.</param>
        /// <param name="token">Position in the comments (depends on service)</param>
        /// <param name="cancellationTokenSource"></param>
        /// <returns>A list of base comments and the next token</returns>
        Task<BaseTrack.CommentResponse> GetCommentsAsync(int count, string token, CancellationTokenSource cancellationTokenSource = null);

        /// <summary>
        /// Likes a track.
        /// </summary>
        /// <returns>True is the track is liked, false if not.</returns>
        Task<bool> LikeAsync();

        /// <summary>
        /// Unlikes a track.
        /// </summary>
        /// <returns>True if the track is not liked, false if it is.</returns>
        Task<bool> UnlikeAsync();
    }
}
