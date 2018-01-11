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

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources.SoundByte;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views.Me
{
    public sealed partial class HistoryView
    {
        public SoundByteCollection<SoundByteHistorySource, BaseTrack> History { get; } = new SoundByteCollection<SoundByteHistorySource, BaseTrack>();

        public HistoryView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.Telemetry.TrackPage("History View");
        }

        public async void PlayShuffleItems()
        {
            await BaseViewModel.ShufflePlayAllTracksAsync(History);
        }

        public async void PlayAllItems()
        {
            await BaseViewModel.PlayAllTracksAsync(History);
        }

        public void ClearAll()
        {

        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            await BaseViewModel.PlayAllTracksAsync(History, (BaseTrack) e.ClickedItem);
        }
    }
}