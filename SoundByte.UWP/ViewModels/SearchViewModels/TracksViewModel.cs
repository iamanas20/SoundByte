using System;
using Windows.UI.Xaml.Controls;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources.Fanburst.Search;
using SoundByte.Core.Sources.SoundCloud.Search;
using SoundByte.Core.Sources.YouTube.Search;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views.Generic;

namespace SoundByte.UWP.ViewModels.SearchViewModels
{
    /// <summary>
    /// View model for searched tracks
    /// </summary>
    public class TracksViewModel : BaseViewModel
    {
        #region Sources
        /// <summary>
        /// Search for SoundCloud tracks
        /// </summary>
        public SoundByteCollection<SoundCloudSearchTrackSource, BaseTrack> SoundCloudTracks { get; } =
            new SoundByteCollection<SoundCloudSearchTrackSource, BaseTrack>();

        /// <summary>
        /// Search for Fanburst tracks
        /// </summary>
        public SoundByteCollection<FanburstSearchTrackSource, BaseTrack> FanburstTracks { get; } =
            new SoundByteCollection<FanburstSearchTrackSource, BaseTrack>();

        /// <summary>
        /// Search for YouTube videos/tracks
        /// </summary>
        public SoundByteCollection<YouTubeSearchTrackSource, BaseTrack> YouTubeTracks { get; } =
            new SoundByteCollection<YouTubeSearchTrackSource, BaseTrack>();
        #endregion

        #region Private Variables
        // The query string
        private string _searchQuery;
        #endregion

        #region Getters and Setters
        /// <summary>
        /// The current search query, setting this value will cause
        /// the sources to update.
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (value == _searchQuery) return;

                _searchQuery = value;
                UpdateProperty();

                // Update the models
                SoundCloudTracks.Source.SearchQuery = value;
                SoundCloudTracks.RefreshItems();

                FanburstTracks.Source.SearchQuery = value;
                FanburstTracks.RefreshItems();

                YouTubeTracks.Source.SearchQuery = value;
                YouTubeTracks.RefreshItems();
            }
        }
        #endregion

        #region Refresh Logic
        public void RefreshAll()
        {
            SoundCloudTracks.RefreshItems();
            FanburstTracks.RefreshItems();
            YouTubeTracks.RefreshItems();
        }
        #endregion

        #region Play All
        /// <summary>
        /// Play all SoundCloud tracks
        /// </summary>
        public async void PlayAllSoundCloud()
        {
            await PlayAllTracksAsync(SoundCloudTracks);
        }

        /// <summary>
        /// Play all YouTube tracks/videos
        /// </summary>
        public async void PlayAllYouTube()
        {
            await PlayAllTracksAsync(YouTubeTracks);
        }

        /// <summary>
        /// Play all Fanburst tracks
        /// </summary>
        public async void PlayFanburst()
        {
            await PlayAllTracksAsync(FanburstTracks);
        }
        #endregion

        #region Play Shuffle
        /// <summary>
        /// Shuffle play SoundCloud tracks
        /// </summary>
        public async void PlayShuffleSoundCloud()
        {
            await ShufflePlayAllTracksAsync(SoundCloudTracks);
        }

        /// <summary>
        /// Shuffle play Fanburst tracks
        /// </summary>
        public async void PlayShuffleFanburst()
        {
            await ShufflePlayAllTracksAsync(FanburstTracks);
        }

        /// <summary>
        /// Shuffle play YouTube tracks
        /// </summary>
        public async void PlayShuffleYouTube()
        {
            await ShufflePlayAllTracksAsync(YouTubeTracks);
        }
        #endregion

        #region Play Single
        /// <summary>
        /// This one method controlls all possible playback points for these
        /// three services.
        /// </summary>
        public async void NavigateTrack(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            // Get the base track item
            var baseTrack = (BaseTrack)e.ClickedItem;

            // Switch to the correct service
            switch (baseTrack.ServiceType)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    {
                        await PlayAllTracksAsync(SoundCloudTracks, baseTrack);
                    }
                    break;
                case ServiceType.YouTube:
                    {
                        await PlayAllTracksAsync(YouTubeTracks, baseTrack);
                    }
                    break;
                case ServiceType.Fanburst:
                    {
                        await PlayAllTracksAsync(FanburstTracks, baseTrack);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region View All
        /// <summary>
        /// Navigate to more SoundCloud tracks
        /// </summary>
        public void NavigateSoundCloudTracks()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = SoundCloudTracks.Source,
                Title = $"Results for \"{SearchQuery}\""
            });
        }

        /// <summary>
        /// Navigate to more Fanburst tracks
        /// </summary>
        public void NavigateFanburstTracks()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = FanburstTracks.Source,
                Title = $"Results for \"{SearchQuery}\""
            });
        }

        /// <summary>
        /// Navigate to more YouTube tracks
        /// </summary>
        public void NavigateYouTubeTracks()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = YouTubeTracks.Source,
                Title = $"Results for \"{SearchQuery}\""
            });
        }
        #endregion
    }
}