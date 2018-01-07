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

using SoundByte.UWP.ViewModels;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.Controls
{
    public sealed partial class NowPlayingBar
    {
        public PlaybackViewModel PlaybackViewModel { get; private set; }

        public NowPlayingBar()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                PlaybackViewModel = new PlaybackViewModel();
            };

            Unloaded += (sender, args) =>
            {
                PlaybackViewModel?.Dispose();
                PlaybackViewModel = null;
            };
        }

        private void NavigateTrack()
        {
            App.NavigateTo(typeof(NowPlayingView));
        }

        public async void ShowCompactView()
        {
            await App.SwitchToCompactView();
        } 
    }
}