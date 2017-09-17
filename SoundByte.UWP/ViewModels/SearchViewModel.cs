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
using SoundByte.UWP.Services;
using SoundByte.UWP.Models;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        #region Private Variables
        // The query string
        private string _searchQuery;
        #endregion

        #region Models
        public SearchTrackModel SearchTracks { get; }
        public FanburstSearchModel FanburstTracks { get; }
        public SearchPlaylistModel SearchPlaylists { get; }
        public SearchUserModel SearchUsers { get; }
        public YouTubeSearchModel YouTubeTracks { get; }

        public SearchViewModel()
        {
            SearchTracks = new SearchTrackModel();
            FanburstTracks = new FanburstSearchModel();
            SearchPlaylists = new SearchPlaylistModel();
            SearchUsers = new SearchUserModel();
            YouTubeTracks = new YouTubeSearchModel();
        }
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
                }

                // Update the models
                SearchTracks.Query = value;
                SearchPlaylists.Query = value;
                SearchUsers.Query = value;
                FanburstTracks.Query = value;
                YouTubeTracks.Query = value;
            }
        }
        #endregion

        #region Method Bindings
        public void NavigateSoundCloudPlaylist(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

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
                        var startPlayback =
                            await PlaybackService.Instance.StartModelMediaPlaybackAsync(FanburstTracks, false, searchItem);
                        if (!startPlayback.success)
                            await new MessageDialog(startPlayback.message, "Error playing searched track.")
                                .ShowAsync();
                    }
                    break;
                case ServiceType.YouTube:
                    {
                        var startPlayback =
                            await PlaybackService.Instance.StartModelMediaPlaybackAsync(YouTubeTracks, false, searchItem);
                        if (!startPlayback.success)
                            await new MessageDialog(startPlayback.message, "Error playing searched track.")
                                .ShowAsync();
                    }
                    break;
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    {
                        var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(SearchTracks, false, searchItem);
                        if (!startPlayback.success)
                            await new MessageDialog(startPlayback.message, "Error playing searched track.")
                                .ShowAsync();
                    }
                    break;
                case ServiceType.ITunesPodcast:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
    }
}