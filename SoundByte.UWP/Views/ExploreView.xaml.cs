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
using SoundByte.UWP.Models;
using SoundByte.UWP.Services;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Views.Search;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExploreView : Page
    {
        public ChartModel ExploreTracks { get; } = new ChartModel();

        public YouTubeTrendingMusicModel YouTubeTracks { get; } = new YouTubeTrendingMusicModel { ModelHeader = "Trending ", ModelType = "YouTube" };

        public FanburstTrendingModel FanburstTracks { get; } = new FanburstTrendingModel { ModelHeader = "Trending ", ModelType = "Fanburst" };

        public ExploreView() 
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
            ExploreTracks.Kind = "top";
            ExploreTracks.Genre = "all-music";
        }

        public async void PlayChartItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(ExploreTracks, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();
        }

        public async void PlayYouTubeItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(YouTubeTracks, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();
        }

        public async void PlayFanburstItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(FanburstTracks, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();
        }

        public void NavigateMoreCharts()
        {
            App.NavigateTo(typeof(SoundCloudExploreView));
        }

        public void NavigateMoreYouTube()
        {
            App.NavigateTo(typeof(SearchTrackView), YouTubeTracks);
        }

        public void NavigateMoreFanburst()
        {
            App.NavigateTo(typeof(SearchTrackView), FanburstTracks);
        }

    }
}
