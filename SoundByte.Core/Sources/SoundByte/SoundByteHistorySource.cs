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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.SoundByte;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Sources.SoundByte
{
    [UsedImplicitly]
    public class SoundByteHistorySource : ISource<BaseTrack>
    {
        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // If the user has not connected their SoundByte account.
            if (!SoundByteService.Current.IsSoundByteAccountConnected)
            {
                return await Task.Run(() =>
                    new SourceResponse<BaseTrack>(null, null, false,
                        Resources.Resources.Sources_SoundByte_NoAccount_Title,
                        Resources.Resources.Sources_SoundByte_History_NoAccount_Description));
            }

            var history = await SoundByteService.Current.GetAsync<HistoryOutputModel>(ServiceType.SoundByte, "history", new Dictionary<string, string>
            {
                { "PageNumber", token },
                { "PageSize", "30" }
            }, cancellationToken).ConfigureAwait(false);

            if (!history.Items.Any())
            {
                return new SourceResponse<BaseTrack>(null, null, false, "No History", "Start listening to songs to build up your history.");
            }

            var nextPage = history.Links.FirstOrDefault(x => x.Rel == "next_page");

            if (nextPage == null)
                return new SourceResponse<BaseTrack>(history.Items, "eol");

            var param = new QueryParameterCollection(nextPage.Href);
            var nextToken = param.FirstOrDefault(x => x.Key == "PageNumber").Value;

            return new SourceResponse<BaseTrack>(history.Items, nextToken);
        }


        public class HistoryOutputModel
        {
            public PagingHeader Paging { get; set; }
            public List<LinkInfo> Links { get; set; }
            public List<BaseTrack> Items { get; set; }
        }
    }
}
