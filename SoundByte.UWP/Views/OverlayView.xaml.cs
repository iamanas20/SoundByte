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

using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    public sealed partial class OverlayView 
    {
        public PlaybackViewModel PlaybackViewModel { get; }

        public OverlayView()
        {
            InitializeComponent();

            PlaybackViewModel = new PlaybackViewModel(CoreWindow.GetForCurrentThread().Dispatcher);

            // Set the accent color
            TitlebarHelper.UpdateTitlebarStyle();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.Telemetry.TrackPage("Compact Overlay View");
        }
    }
}