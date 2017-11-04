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

using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources
{
    /// <summary>
    /// This interface represents an incremental data source. This data layer connected
    /// into the UI layer for each platform. 
    /// </summary>
    public interface ISource<TSource>
    {
        /// <summary>
        /// This method returns a list of items depending on the count and token
        /// </summary>
        /// <param name="count">A amount of items to get from the API</param>
        /// <param name="token">A Token place holder</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a collection of <typeparamref name="TSource"/>.</returns>
        Task<SourceResponse<TSource>> GetItemsAsync(int count, string token, CancellationTokenSource cancellationToken = default(CancellationTokenSource));
    }
}
