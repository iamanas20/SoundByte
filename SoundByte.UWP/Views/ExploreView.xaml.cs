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
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views.Application;
using SoundByte.UWP.Views.Generic;

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

        public SoundByteCollection<ExploreSoundCloudSource, BaseTrack> ExploreTracks { get; } =
            new SoundByteCollection<ExploreSoundCloudSource, BaseTrack>();

        #endregion

        public ExploreView() 
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        public async void PlayChartItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(ExploreTracks, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();
        }

        public async void PlayYouTubeItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(YouTubeTracks, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();
        }

        public async void PlayFanburstItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(FanburstTracks, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();
        }

        public void NavigateMoreCharts()
        {
            App.NavigateTo(typeof(SoundCloudExploreView));
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

        public void NavigateMoreFanburst()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = FanburstTracks.Source,
                Title = "Fanburst",
                Subtitle = "Trending"
            });
        }

        private void WhatsNewButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.NavigateTo(typeof(WhatsNewView));
        }
    }
}
