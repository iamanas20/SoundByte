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
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using SoundByte.Core.API.Endpoints;
using SoundByte.Core.Dialogs;
using SoundByte.Core.Services;
using SoundByte.UWP.Models;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using Playlist = SoundByte.Core.API.Endpoints.Playlist;
using SearchBox = SoundByte.UWP.UserControls.SearchBox;

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
        public SearchTrackModel SearchTracks { get; } = new SearchTrackModel();

        public FanburstSearchModel FanburstTracks { get; } = new FanburstSearchModel();

        // Model for the playlist searches
        public SearchPlaylistModel SearchPlaylists { get; } = new SearchPlaylistModel();

        // Model for the user searches
        public SearchUserModel SearchUsers { get; } = new SearchUserModel();

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
        }

        public void Search(object sender, SearchBoxQuerySubmittedEventArgs args)
        {
            App.NavigateTo(typeof(Search), args.QueryText);
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

            if (e.ClickedItem.GetType().Name == "Track")
            {
                var searchItem = e.ClickedItem as Track;

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

                        if (searchItem.ServiceType == SoundByteService.ServiceType.Fanburst)
                        {
                            var startPlayback =
                                await PlaybackService.Instance.StartMediaPlayback(FanburstTracks.ToList(),
                                    FanburstTracks.Token, false, searchItem);
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
                            App.NavigateTo(typeof(Views.Playlist), playlist);
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
                            App.NavigateTo(typeof(Views.Playlist), playlistR);
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
                App.NavigateTo(typeof(UserView), e.ClickedItem as User);
            }
            else if (e.ClickedItem.GetType().Name == "Playlist")
            {
                App.NavigateTo(typeof(Views.Playlist), e.ClickedItem as Playlist);
            }

            App.IsLoading = false;
        }

        #endregion
    }
}