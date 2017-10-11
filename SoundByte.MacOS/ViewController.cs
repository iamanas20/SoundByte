using System;

using AppKit;
using Foundation;

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

        partial void SearchBox(NSSearchField sender)
        {

            var text = sender.StringValue;
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
