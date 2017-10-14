using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace SoundByte.MacOS
{
    public partial class TrackItemView : AppKit.NSView
    {
        #region Constructors

        // Called when created from unmanaged code
        public TrackItemView(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public TrackItemView(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion
    }
}
