/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using SoundByte.UWP.Services;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.UserControls
{
    public sealed partial class NowPlayingBar
    {
        public NowPlayingBar()
        {
            InitializeComponent();
        }

        public PlaybackService Service => PlaybackService.Instance;

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