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
using Microsoft.EntityFrameworkCore;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources;
using SoundByte.UWP.DatabaseContexts;

namespace SoundByte.UWP.Sources
{
    [UsedImplicitly]
    public class HistorySource : ISource<BaseTrack>
    {
        private bool _firstTime = true;

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            using (var db = new HistoryContext())
            {
                if (_firstTime)
                    db.Database.Migrate();

                _firstTime = false;


                // Set the default token to zero
                if (string.IsNullOrEmpty(token))
                    token = "0";

                if (token == "eol")
                    token = "0";

                // Get items in date descending order, skip the token and take the count
                var tracks = await db.Tracks.Include(x => x.User).OrderByDescending(x => x.LastPlaybackDate).Skip(int.Parse(token)).Take(count).ToListAsync();

                // If there are no tracks
                if (!tracks.Any())
                {
                    return new SourceResponse<BaseTrack>(null, null, false, "No History", "Listen to some songs to get started");
                }

                // Set the new token;
                if (token != "eol")
                    token = (int.Parse(token) + count).ToString();

                return new SourceResponse<BaseTrack>(tracks, token);
            }
        }
    }
}
