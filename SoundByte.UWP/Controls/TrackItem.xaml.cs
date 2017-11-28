using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Items.Track;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SoundByte.UWP.Controls
{
    public sealed partial class TrackItem : UserControl
    {
        /// <summary>
        /// Identifies the Track dependency property.
        /// </summary>
        public static readonly DependencyProperty TrackProperty =
            DependencyProperty.Register(nameof(Track), typeof(BaseTrack), typeof(TrackItem), null);

        /// <summary>
        /// Gets or sets the rounding interval for the Value.
        /// </summary>
        public BaseTrack Track
        {
            get => (BaseTrack)GetValue(TrackProperty);
            set => SetValue(TrackProperty, value);
        }


        public TrackItem()
        {
            InitializeComponent();
        }

        private void Share(object sender, RoutedEventArgs e)
        {

        }

        private void AddToPlaylist(object sender, RoutedEventArgs e)
        {

        }
    }
}
