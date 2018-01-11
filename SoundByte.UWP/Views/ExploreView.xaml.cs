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

using SoundByte.Core.Items.Track;
using SoundByte.UWP.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Services;
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

        public SoundByteCollection<FanburstPopularTracksSource, BaseTrack> FanburstTracks { get; } =
            new SoundByteCollection<FanburstPopularTracksSource, BaseTrack>();

        public SoundByteCollection<SoundCloudExploreSource, BaseTrack> SoundCloudTracks { get; } =
            new SoundByteCollection<SoundCloudExploreSource, BaseTrack>();

        #endregion

        public ExploreView() 
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // If we have a connected SoundByte account, don't show the banner
            SoundByteAccountBanner.Visibility = SoundByteService.Current.IsSoundByteAccountConnected 
                ? Visibility.Collapsed : Visibility.Visible;
        }

        #region SoundCloud
        public async void PlayChartItem(object sender, ItemClickEventArgs e)
        {
            await BaseViewModel.PlayAllTracksAsync(SoundCloudTracks, (BaseTrack) e.ClickedItem);
        }

        public void NavigateMoreCharts()
        {
            App.NavigateTo(typeof(SoundCloudExploreView));
        }

        public async void PlayShuffleSoundCloud()
        {
            await BaseViewModel.ShufflePlayAllTracksAsync(SoundCloudTracks);
        }

        public async void PlaySoundCloud()
        {
            await BaseViewModel.PlayAllTracksAsync(SoundCloudTracks);
        }
        #endregion

        #region YouTube
        public async void PlayYouTubeItem(object sender, ItemClickEventArgs e)
        {
            await BaseViewModel.PlayAllTracksAsync(YouTubeTracks, (BaseTrack)e.ClickedItem);
        }

        public void NavigateMoreYouTube()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = YouTubeTracks.Source,
                Title = "YouTube"         
            });
        }

        public async void PlayShuffleYouTube()
        {
            await BaseViewModel.ShufflePlayAllTracksAsync(YouTubeTracks);
        }

        public async void PlayYouTube()
        {
            await BaseViewModel.PlayAllTracksAsync(YouTubeTracks);
        }
        #endregion

        #region Fanburst
        public async void PlayFanburstItem(object sender, ItemClickEventArgs e)
        {
            await BaseViewModel.PlayAllTracksAsync(FanburstTracks, (BaseTrack)e.ClickedItem);
        }

        public void NavigateMoreFanburst()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = FanburstTracks.Source,
                Title = "Fanburst"
            });
        }

        public async void PlayShuffleFanburst()
        {
            await BaseViewModel.ShufflePlayAllTracksAsync(FanburstTracks);
        }

        public async void PlayFanburst()
        {
            await BaseViewModel.PlayAllTracksAsync(FanburstTracks);
        }
        #endregion

        private async void WhatsNewButtonClick(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync<WhatsNewDialog>();
        }

        private void SoundByteAccountLearnMoreClick(object sender, RoutedEventArgs e)
        {
            // Second pivot is the soundbyte account pivot
            App.NavigateTo(typeof(AccountManagerView), new AccountManagerView.AccountManagerArgs { PivotIndex = 1 });
        }
    }
}
