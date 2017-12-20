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

using System;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Services;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Sources.Fanburst;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.YouTube;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views.Generic;
using SoundByte.UWP.Views.Me;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExploreView
    {
        #region Sources

        public SoundByteCollection<ExploreYouTubeTrendingSource, BaseTrack> YouTubeTracks { get; } =
            new SoundByteCollection<ExploreYouTubeTrendingSource, BaseTrack>();

        public SoundByteCollection<ExploreFanburstPopularSource, BaseTrack> FanburstTracks { get; } =
            new SoundByteCollection<ExploreFanburstPopularSource, BaseTrack>();

        public SoundByteCollection<ExploreSoundCloudSource, BaseTrack> SoundCloudTracks { get; } =
            new SoundByteCollection<ExploreSoundCloudSource, BaseTrack>();

        #endregion

        public ExploreView() 
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        #region SoundCloud
        public async void PlayChartItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(SoundCloudTracks, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();
        }

        public void NavigateMoreCharts()
        {
            App.NavigateTo(typeof(SoundCloudExploreView));
        }

        public async void PlayShuffleSoundCloud()
        {
            await BaseViewModel.ShuffleTracksAsync(SoundCloudTracks);
        }

        public async void PlaySoundCloud()
        {
            await BaseViewModel.PlayAllItemsAsync(SoundCloudTracks);
        }
        #endregion

        #region YouTube
        public async void PlayYouTubeItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(YouTubeTracks, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();
        }

        public void NavigateMoreYouTube()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = YouTubeTracks.Source,
                Title = "YouTube",
                Subtitle = "Trending"
            });
        }

        public async void PlayShuffleYouTube()
        {
            await BaseViewModel.ShuffleTracksAsync(YouTubeTracks);
        }

        public async void PlayYouTube()
        {
            await BaseViewModel.PlayAllItemsAsync(YouTubeTracks);
        }
        #endregion

        #region Fanburst
        public async void PlayFanburstItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(FanburstTracks, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();
        }

        public void NavigateMoreFanburst()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = FanburstTracks.Source,
                Title = "Fanburst",
                Subtitle = "Trending"
            });
        }

        public async void PlayShuffleFanburst()
        {
            await BaseViewModel.ShuffleTracksAsync(FanburstTracks);
        }

        public async void PlayFanburst()
        {
            await BaseViewModel.PlayAllItemsAsync(FanburstTracks);
        }
        #endregion

        private async void WhatsNewButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync<WhatsNewDialog>();
        }

        private void SoundByteAccountLearnMoreClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Second pivot is the soundbyte account pivot
            App.NavigateTo(typeof(AccountManagerView), new AccountManagerView.AccountManagerArgs { PivotIndex = 1 });
        }
    }
}
