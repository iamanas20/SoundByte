using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using SoundByte.Core.Items.Track;

namespace SoundByte.MacOS
{
    public partial class TrackItemViewController : AppKit.NSViewController
    {
        private BaseTrack _track;

        #region Constructors

        // Called when created from unmanaged code
        public TrackItemViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public TrackItemViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public TrackItemViewController() : base("TrackItemView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        [Export("Track")]
        public BaseTrack Track
        {
            get { return _track; }
            set
            {
                WillChangeValue("Track");
                _track = value;
                DidChangeValue("Track");
            }
        }

        //strongly typed view accessor
        public new TrackItemView View
        {
            get
            {
                return (TrackItemView)base.View;
            }
        }
    }
}
