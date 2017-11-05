using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SoundByte.Core.Items.Track;

namespace SoundByte.Core.Sources
{
    public class HistorySource : ISource<BaseTrack>
    {
        public static void AddToHistory(BaseTrack track)
        {
            
        }

        public async Task<SourceResponse<BaseTrack>> GetItemsAsync(int count, string token,
            CancellationTokenSource cancellationToken = default(CancellationTokenSource))
        {
            // Set the default token to zero
            if (string.IsNullOrEmpty(token))
                token = "0";

            if (token == "eol")
                token = "0";

            return null;

            // using (var db = new LiteDatabase(@".\SBHistory.db"))
            // {
            //  var tracks = db.GetCollection<BaseTrack>().FindAll().OrderByDescending(x => x.LastPlaybackDate).Skip(int.Parse(token)).Take(count).ToList();

            // If there are no tracks
            //   if (!tracks.Any())
            //  {
            //      return new SourceResponse<BaseTrack>(null, null, false, "No History", "Listen to some songs to get started");
            //  }

            // Set the new token;
            //   if (token != "eol")
            //     token = (int.Parse(token) + count).ToString();

            //  return new SourceResponse<BaseTrack>(tracks, token);
            // }
        }
    }
}
