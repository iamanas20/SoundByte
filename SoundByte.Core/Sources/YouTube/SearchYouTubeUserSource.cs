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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;

namespace SoundByte.Core.Sources.YouTube
{
    [UsedImplicitly]
    public class SearchYouTubeUserSource : ISource<BaseUser>
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public Task<SourceResponse<BaseUser>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            throw new NotImplementedException();
        }

       
    }
}
