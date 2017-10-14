using System;

using AppKit;
using Foundation;
using SoundByte.Core.Items.Track;
using System.Threading.Tasks;
using SoundByte.Core.Services;
using System.Linq;

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
            var results = await SoundCloudTrack.SearchAsync(sender.StringValue, 10, null, searchToken);

            System.Diagnostics.Debug.WriteLine(results.Tracks.FirstOrDefault()?.Title);

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
