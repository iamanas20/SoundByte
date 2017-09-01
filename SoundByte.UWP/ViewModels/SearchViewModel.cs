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
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using SoundByte.API;
using SoundByte.API.Endpoints;
using SoundByte.API.Items.Track;
using SoundByte.API.Items.User;
using SoundByte.Core.Dialogs;
using SoundByte.Core.Services;
using SoundByte.UWP.Models;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using Playlist = SoundByte.API.Endpoints.Playlist;

namespace SoundByte.UWP.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        #region Private Variables

        // Args for filtering
        private string _filterArgs;

        // The query string
        private string _searchQuery;

        #endregion

        #region Models

        // Model for the track searches
        public SearchTrackModel SearchTracks { get; private set; } = new SearchTrackModel();

        public FanburstSearchModel FanburstTracks { get; private set; } = new FanburstSearchModel();

        // Model for the playlist searches
        public SearchPlaylistModel SearchPlaylists { get; private set; } = new SearchPlaylistModel();

        // Model for the user searches
        public SearchUserModel SearchUsers { get; private set; } = new SearchUserModel();

        public YouTubeSearchModel YouTubeTracks { get; private set; } = new YouTubeSearchModel();

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
                SearchTracks.RefreshItems();

                SearchPlaylists.Query = value;
                SearchPlaylists.RefreshItems();

                SearchUsers.Query = value;
                SearchUsers.RefreshItems();

                FanburstTracks.Query = value;
                FanburstTracks.RefreshItems();

                YouTubeTracks.Query = value;
                YouTubeTracks.RefreshItems();
            }
        }

        /// <summary>
        ///     Args for filtering
        /// </summary>
        public string FilterArgs
        {
            get => _filterArgs;
            set
            {
                if (value != _filterArgs)
                {
                    _filterArgs = value;
                    UpdateProperty();
                }

                // Update the models
                SearchTracks.Filter = value;
                SearchTracks.RefreshItems();
            }
        }

        #endregion

        #region Method Bindings

        public void RefreshAll()
        {
            // Update the models
            SearchTracks.RefreshItems();
            SearchPlaylists.RefreshItems();
            SearchUsers.RefreshItems();
            FanburstTracks.RefreshItems();
            YouTubeTracks.RefreshItems();
        }

        public void Search(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            SearchQuery = args.QueryText;

            InputPane.GetForCurrentView().TryHide();
        }

        public async void ShowFilterMenu()
        {
            var filterDialog = new FilterDialog();
            filterDialog.FilterApplied += (sender, args) =>
            {
                FilterArgs = (args as FilterDialog.FilterAppliedArgs)?.FilterArgs;
            };

            await filterDialog.ShowAsync();
        }

        public async void NavigateItem(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            // Show the loading ring
            App.IsLoading = true;

            if (e.ClickedItem.GetType().Name == "BaseTrack")
            {
                var searchItem = e.ClickedItem as BaseTrack;

                // Get the resource loader
                var resources = ResourceLoader.GetForCurrentView();
                // Create the error dialog
                var uiUpdateError = new ContentDialog
                {
                    Title = resources.GetString("PlaylistNavigateError_Title"),
                    Content = resources.GetString("PlaylistNavigateError_Content"),
                    IsPrimaryButtonEnabled = true,
                    PrimaryButtonText = resources.GetString("PlaylistNavigateError_Button")
                };

                switch (searchItem?.Kind)
                {
                    case "track":
                    case "track-repost":
                        // Play this item

                        if (searchItem.ServiceType == ServiceType.Fanburst)
                        {
                            var startPlayback =
                                await PlaybackService.Instance.StartMediaPlayback(FanburstTracks.ToList(),
                                    FanburstTracks.Token, false, searchItem);
                            if (!startPlayback.success)
                                await new MessageDialog(startPlayback.message, "Error playing searched track.")
                                    .ShowAsync();
                        }
                        else if (searchItem.ServiceType == ServiceType.YouTube)
                        {
                            var startPlayback =
                                await PlaybackService.Instance.StartMediaPlayback(YouTubeTracks.ToList(), 
                                    YouTubeTracks.Token, false, searchItem);
                            if (!startPlayback.success)
                                await new MessageDialog(startPlayback.message, "Error playing searched track.")
                                    .ShowAsync();
                        }
                        else
                        {
                            var startPlayback = await PlaybackService.Instance.StartMediaPlayback(SearchTracks.ToList(),
                                SearchTracks.Token, false, searchItem);
                            if (!startPlayback.success)
                                await new MessageDialog(startPlayback.message, "Error playing searched track.")
                                    .ShowAsync();
                        }

                        break;
                    case "playlist":
                        try
                        {
                            var playlist =
                                await SoundByteService.Instance.GetAsync<Playlist>("/playlist/" + searchItem.Id);
                            App.NavigateTo(typeof(PlaylistView), playlist);
                        }
                        catch (Exception)
                        {
                            await uiUpdateError.ShowAsync();
                        }
                        break;
                    case "playlist-repost":
                        try
                        {
                            var playlistR =
                                await SoundByteService.Instance.GetAsync<Playlist>("/playlist/" + searchItem.Id);
                            App.NavigateTo(typeof(PlaylistView), playlistR);
                        }
                        catch (Exception)
                        {
                            await uiUpdateError.ShowAsync();
                        }

                        break;
                }
            }
            else if (e.ClickedItem.GetType().Name == "User")
            {
                App.NavigateTo(typeof(UserView), e.ClickedItem as BaseUser);
            }
            else if (e.ClickedItem.GetType().Name == "Playlist")
            {
                App.NavigateTo(typeof(Views.PlaylistView), e.ClickedItem as Playlist);
            }

            App.IsLoading = false;
        }

        #endregion
    }
}