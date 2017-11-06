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
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using SoundByte.Core;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Sources.Fanburst;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.YouTube;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using SoundByte.UWP.Views.Generic;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels.Generic;

namespace SoundByte.UWP.ViewModels.Search
{
    public class SearchViewModel : BaseViewModel
    {
        #region Private Variables
        // The query string
        private string _searchQuery;
        #endregion

        #region Sources

        public SoundByteCollection<SearchSoundCloudTrackSource, BaseTrack> SearchTracks { get; } =
            new SoundByteCollection<SearchSoundCloudTrackSource, BaseTrack>();

        public SoundByteCollection<SearchFanburstTrackSource, BaseTrack> FanburstTracks { get; } =
            new SoundByteCollection<SearchFanburstTrackSource, BaseTrack>();

        public SoundByteCollection<SearchSoundCloudPlaylistSource, BasePlaylist> SearchPlaylists { get; } =
            new SoundByteCollection<SearchSoundCloudPlaylistSource, BasePlaylist>();

        public SoundByteCollection<SearchSoundCloudUserSource, BaseUser> SearchUsers { get; } =
            new SoundByteCollection<SearchSoundCloudUserSource, BaseUser>();

        public SoundByteCollection<SearchYouTubeTrackSource, BaseTrack> YouTubeTracks { get; } =
            new SoundByteCollection<SearchYouTubeTrackSource, BaseTrack>();
        #endregion

        #region Getters and Setters
        /// <summary>
        ///     The current pivot item that the user is viewing
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (value != _searchQuery)
                {
                    _searchQuery = value;
                    UpdateProperty();

                    // Update the models
                    SearchTracks.Source.SearchQuery = value;
                    SearchTracks.RefreshItems();

                    SearchPlaylists.Source.SearchQuery = value;
                    SearchPlaylists.RefreshItems();

                    SearchUsers.Source.SearchQuery = value;
                    SearchUsers.RefreshItems();

                    FanburstTracks.Source.SearchQuery = value;
                    FanburstTracks.RefreshItems();

                    YouTubeTracks.Source.SearchQuery = value;
                    YouTubeTracks.RefreshItems();
                }
            }
        }
        #endregion

        #region Method Bindings
        public void NavigateSoundCloudPlaylist(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

           // var gridView = App.CurrentFrame.FindName("PlaylistsView") as GridView;
           // gridView?.PrepareConnectedAnimation("PlaylistImage", e.ClickedItem as BasePlaylist, "ImagePanel");

            App.NavigateTo(typeof(PlaylistView), e.ClickedItem as BasePlaylist);
        }

        public void NavigateSoundCloudUser(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            App.NavigateTo(typeof(UserView), e.ClickedItem as BaseUser);
        }

        public async void NavigateTrack(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            var searchItem = (BaseTrack)e.ClickedItem;

            switch (searchItem.ServiceType)
            {
                case ServiceType.Fanburst:
                    {
                        // Load some more items
                        await FanburstTracks.LoadMoreItemsAsync(50);

                        var startPlayback =
                            await PlaybackService.Instance.StartModelMediaPlaybackAsync(FanburstTracks, false, searchItem);
                        if (!startPlayback.Success)
                            await new MessageDialog(startPlayback.Message, "Error playing searched track.")
                                .ShowAsync();
                    }
                    break;
                case ServiceType.YouTube:
                    {
                        // Load some more items
                        await YouTubeTracks.LoadMoreItemsAsync(50);

                        var startPlayback =
                            await PlaybackService.Instance.StartModelMediaPlaybackAsync(YouTubeTracks, false, searchItem);
                        if (!startPlayback.Success)
                            await new MessageDialog(startPlayback.Message, "Error playing searched track.")
                                .ShowAsync();
                    }
                    break;
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                {
                        // Load some more items
                        await SearchTracks.LoadMoreItemsAsync(50);

                        // Start media playback
                        var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(SearchTracks, false, searchItem);
                        if (!startPlayback.Success)
                            await new MessageDialog(startPlayback.Message, "Error playing searched track.")
                                .ShowAsync();
                    }
                    break;
                case ServiceType.ITunesPodcast:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void NavigateSoundCloudTracks()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = SearchTracks.Source,
                Title = "Search",
                Subtitle = "SoundCloud Tracks"
            });
        }

        public void NavigateSoundCloudPlaylists()
        {
            App.NavigateTo(typeof(PlaylistListView), new PlaylistListViewModel.PlaylistViewModelHolder
            {
                Playlist = SearchPlaylists.Source,
                Title = "Search",
                Subtitle = "SoundCloud Playlists"

            });
        }

        public void NavigateSoundCloudUsers()
        {
            App.NavigateTo(typeof(UserListView), new UserListViewModel.UserViewModelHolder
            {
                User = SearchUsers.Source,
                Title = "Search",
                Subtitle = "SoundCloud Users"
            });
        }

        public void NavigateYouTubeTracks()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = YouTubeTracks.Source,
                Title = "Search",
                Subtitle = "Youtube Videos"
            });
        }

        public void NavigateFanburstTracks()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = FanburstTracks.Source,
                Title = "Search",
                Subtitle = "Fanburst Tracks"
            });
        }

        public async void NavigatePodcasts()
        {
            await new MessageDialog("This feature will come in a future update.", "Coming Soon..").ShowAsync();
        }
        #endregion
    }
}