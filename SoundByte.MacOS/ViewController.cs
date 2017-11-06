using System;
using AppKit;
using System.Linq;
using SoundByte.Core.Sources.SoundCloud;

namespace SoundByte.MacOS
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        System.Threading.CancellationTokenSource searchToken = new System.Threading.CancellationTokenSource();

        async partial void SearchBox(NSSearchField sender)
        {
            // Cancel pending search
            searchToken.Cancel();

            // Get search results
            var searchResults = await new SearchSoundCloudTrackSource
            {
                SearchQuery = sender.StringValue
            }.GetItemsAsync(10, null, searchToken);

            System.Diagnostics.Debug.WriteLine(searchResults.Items.FirstOrDefault()?.Title);
        }
    }
}
