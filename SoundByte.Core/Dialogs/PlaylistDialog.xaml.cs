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
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Web.Http;
using SoundByte.API;
using SoundByte.API.Endpoints;
using SoundByte.API.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Core.Dialogs
{
    /// <summary>
    ///     Allows the user to add and remove items to and from
    ///     playlists.
    /// </summary>
    public sealed partial class PlaylistDialog
    {
        // Stop the check event when loading
        private bool _blockItemsLoading;

        public PlaylistDialog(BaseTrack trackItem)
        {
            // Do this before the xaml is loaded, to make sure
            // the object can be binded to.
            Track = trackItem;

            // Load the XAML page
            InitializeComponent();

            // Bind the open event handler
            Opened += LoadContent;
        }

        /// <summary>
        ///     The track that we want to add to a playlist
        /// </summary>
        public BaseTrack Track { get; }

        /// <summary>
        ///     A list of user playlists that we can add
        ///     this track to.
        /// </summary>
        private ObservableCollection<Playlist> Playlist { get; } = new ObservableCollection<Playlist>();

        public async void CreatePlaylist()
        {
            // Hide the current dialog
            Hide();

            // Create a text box for the playlist name
            var playlistTitle = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 5, 0, 5)
            };
            // Create a stack panel to hold all the contents
            var contentPanel = new StackPanel();
            // Add a text block for the title
            contentPanel.Children.Add(new TextBlock {Text = "Title:", Margin = new Thickness(0, 5, 0, 5)});
            // Add the text box for the title input
            contentPanel.Children.Add(playlistTitle);
            // Create the dialog box
            var dialog = new ContentDialog
            {
                Title = "Create New Playlist",
                Content = contentPanel,
                PrimaryButtonText = "Create",
                SecondaryButtonText = "Cancel",
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = true,
                Background = Application.Current.Resources["ShellBackground"] as SolidColorBrush
            };

            // Set the primary button click handler
            dialog.PrimaryButtonClick += async delegate
            {
                // Check that the playlist title is not null or empty
                if (!string.IsNullOrEmpty(playlistTitle.Text.Trim()))
                {
                    // Create the json string needed to create the playlist
                    var json = "{\"playlist\":{\"title\":\"" + playlistTitle.Text.Trim() + "\",\"tracks\":[{\"id\":\"" +
                               Track.Id + "\"}]}}";

                    try
                    {
                        // Get the response message
                        var response = await SoundByteService.Instance.PostAsync<Playlist>("/playlists",
                            new HttpStringContent(json, UnicodeEncoding.Utf8, "application/json"));

                        // Check that the creation was successful
                        if (response != null)
                        {
                            // Change the UI to display that the track is in the playlist
                            response.IsTrackInInternalSet = true;
                            // Add the playlist to the UI
                            Playlist.Insert(0, response);

                            dialog.Hide();
                            await ShowAsync();
                        }
                    }
                    catch (Exception)
                    {
                        // Exception is caused when creating item, just go back
                        dialog.Hide();
                        await ShowAsync();
                    }
                }
                else
                {
                    // Tell the user that they must enter a title
                    await new MessageDialog("Please enter the set title in order to continue.").ShowAsync();
                }
            };

            dialog.SecondaryButtonClick += async (sender, args) =>
            {
                // Hide the current dialog, and then show the old dialog
                dialog.Hide();
                await ShowAsync();
            };

            // Show the dialog
            await dialog.ShowAsync();
        }

        private async void LoadContent(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            if ((Track.ServiceType == ServiceType.Fanburst && !SoundByteService.Instance.IsFanBurstAccountConnected)
                || (Track.ServiceType == ServiceType.SoundCloud &&
                    !SoundByteService.Instance.IsSoundCloudAccountConnected))
            {
                Hide();
                await new MessageDialog("You must first login to add tracks to playlists.", "Login Required").ShowAsync();
                return;
            }

            if (Track.ServiceType == ServiceType.Fanburst)
            {
                Hide();
                await new MessageDialog("Adding Fanburst tracks to playlists is not currently supported.", "Not Supported").ShowAsync();
                return;
            }

            // We are loading content
            LoadingRing.Visibility = Visibility.Visible;

            _blockItemsLoading = true;

            // Get a list of the user playlists
            var userPlaylists =
                await SoundByteService.Instance.GetAsync<List<Playlist>>(ServiceType.SoundCloud, "/me/playlists");

            Playlist.Clear();

            // Loop though all the playlists
            foreach (var playlist in userPlaylists)
            {
                _blockItemsLoading = true;
                // Check if the track in in the playlist
                playlist.IsTrackInInternalSet = playlist.Tracks?.FirstOrDefault(x => x.Id == int.Parse(Track.Id)) != null;
                // Add the track to the UI
                Playlist.Add(playlist);
                _blockItemsLoading = false;
            }

            _blockItemsLoading = false;

            // We are done loading content
            LoadingRing.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     This method is called whenever the playlist items checkbox is unchecked
        ///     This method then removes the currently playing track from the playlist
        ///     and updates the UI.
        /// </summary>
        private async void RemoveTrackFromPlaylist(object sender, RoutedEventArgs e)
        {
            // Used to stop the playlist object running on first load
            if (_blockItemsLoading) return;

            // Show the loading ring to let the user know that we are doing something
            LoadingRing.Visibility = Visibility.Visible;

            // Get the playlist id
            var playlistId = int.Parse(((CheckBox) e.OriginalSource).Tag.ToString());
            // Check that the playlist id is not null

            try
            {
                // Get the playlist object from the internet
                var playlistObject = await SoundByteService.Instance.GetAsync<Playlist>("/playlists/" + playlistId);
                // Get the track within the object
                var trackObject = playlistObject.Tracks.FirstOrDefault(x => x.Id == int.Parse(Track.Id));

                // Check that the track exits
                if (trackObject != null)
                    playlistObject.Tracks.Remove(trackObject);

                // Start creating the json track string with the basic json
                var json = playlistObject.Tracks.Aggregate("{\"playlist\":{\"tracks\":[",
                    (current, track) => current + "{\"id\":\"" + track.Id + "\"},");

                // Loop through all the tracks adding the required json
                // Complete the json string
                json = json.TrimEnd(',') + "]}}";

                // Create the http request
                var response = await SoundByteService.Instance.PutAsync("/playlists/" + playlistId,
                    new HttpStringContent(json, UnicodeEncoding.Utf8, "application/json"));

                // Check that the remove was successful
                if (!response)
                {
                    _blockItemsLoading = true;
                    ((CheckBox) e.OriginalSource).IsChecked = true;
                    _blockItemsLoading = false;

                    // Alert the user that the request failed, also alert the reason
                    await new MessageDialog("An error occured while trying to remove the current sound from this set.")
                        .ShowAsync();
                }
            }
            catch (Exception)
            {
                _blockItemsLoading = true;
                ((CheckBox) e.OriginalSource).IsChecked = true;
                _blockItemsLoading = false;

                // Alert the user about an unknown error
                await new MessageDialog(
                        "An unknown error occured while removing the current sound from this set. Make sure that you are connected to the internet and try again.")
                    .ShowAsync();
            }
            finally
            {
                LoadingRing.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     This method opens a dialog box for the user to
        ///     create a playlist and add the current track to it.
        /// </summary>
        private async void AddTrackToPlaylist(object sender, RoutedEventArgs e)
        {
            // Used to stop the playlist object running on first load
            if (_blockItemsLoading) return;

            // Show the loading ring to let the user know that we are doing something
            LoadingRing.Visibility = Visibility.Visible;

            // Get the playlist id
            var playlistId = int.Parse(((CheckBox) e.OriginalSource).Tag.ToString());

            try
            {
                // Get the playlist object from the internet
                var playlistObject = await SoundByteService.Instance.GetAsync<Playlist>("/playlists/" + playlistId);

                // Start creating the json track string with the basic json
                var json = playlistObject.Tracks.Aggregate("{\"playlist\":{\"tracks\":[",
                    (current, track) => current + "{\"id\":\"" + track.Id + "\"},");

                // Complete the json string by adding the current track
                json += "{\"id\":\"" + Track.Id + "\"}]}}";
                // Create the http request
                var response =
                    await SoundByteService.Instance.PutAsync(
                        "/playlists/" + int.Parse(playlistObject.Id) + "/?secret-token=" + playlistObject.SecretToken,
                        new HttpStringContent(json, UnicodeEncoding.Utf8, "application/json"));

                // Check that the update was successful
                if (!response)
                {
                    _blockItemsLoading = true;
                    ((CheckBox) e.OriginalSource).IsChecked = false;
                    _blockItemsLoading = false;

                    // Alert the user that the request failed, also alert the reason
                    await new MessageDialog("An error occured while trying to add the current sound to this set.")
                        .ShowAsync();
                }
            }
            catch (Exception ex)
            {
                _blockItemsLoading = true;
                ((CheckBox) e.OriginalSource).IsChecked = false;
                _blockItemsLoading = false;

                // Alert the user about an unknown error
                await new MessageDialog(
                    "An unknown error occured while adding the current sound to this set. Make sure that you are connected to the internet and try again. Error:\n" +
                    ex).ShowAsync();
            }
            finally
            {
                LoadingRing.Visibility = Visibility.Collapsed;
            }
        }
    }
}