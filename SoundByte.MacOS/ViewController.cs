using System;
using AppKit;
using Foundation;
using SoundByte.Core.Items.Track;
using System.Linq;
using SoundByte.Core.Sources.SoundCloud;

namespace SoundByte.MacOS
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            

            // Do any additional setup after loading the view.
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


        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
