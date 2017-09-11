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
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Models;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.ViewModels
{
    /// <summary>
    ///     The view model for the HomeView page
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        // Model for stream items
        public StreamModel StreamItems { get; } = new StreamModel();

       

        /// <summary>
        ///     Refreshes the models depending on what
        ///     page is being viewed
        /// </summary>
        public void RefreshStreamItems()
        {
            // As this process can take a while
            // we need to enable the loading ring
            App.IsLoading = true;

            StreamItems.RefreshItems();

            // Now that we are complete, we need to hide
            // the loading ring.
            App.IsLoading = false;
        }

        public async void PlayAllStreamTracks()
        {
            // We are loading
            App.IsLoading = true;

            // Get a list of items
            var trackList = StreamItems.Where(t => t.Type == "track" || t.Type == "track-repost" && t.Type != null)
                .Select(t => t.Track).ToList();

            var baseTrackList = new List<BaseTrack>();
            trackList.ToList().ForEach(x => baseTrackList.Add(x.ToBaseTrack()));

            var startPlayback = await PlaybackService.Instance.StartPlaylistMediaPlaybackAsync(baseTrackList);

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing stream.").ShowAsync();

            // We are not loading
            App.IsLoading = false;
        }

       

        public async void PlayShuffleStreamTracks()
        {
            // Get a list of items
            var trackList = StreamItems.Where(t => t.Type == "track" || t.Type == "track-repost" && t.Type != null)
                .Select(t => t.Track).ToList();

            var baseTrackList = new List<BaseTrack>();
            trackList.ToList().ForEach(x => baseTrackList.Add(x.ToBaseTrack()));

            // Shuffle and play the items
            await ShuffleTracksAsync(baseTrackList);
        }

      

        public async void NavigateStream(object sender, ItemClickEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            // Get a list of items
            var trackList = StreamItems.Where(t => t.Type == "track" || t.Type == "track-repost" && t.Type != null)
                .Select(t => t.Track).ToList();

            var baseTrackList = new List<BaseTrack>();
            trackList.ToList().ForEach(x => baseTrackList.Add(x.ToBaseTrack()));

            // Get the clicked item
            var streamItem = (StreamModel.StreamItem) e.ClickedItem;

            switch (streamItem.Type)
            {
                case "track":
                case "track-repost":
                    if (streamItem.Track != null)
                    {
                        var startPlayback = await PlaybackService.Instance.StartPlaylistMediaPlaybackAsync(baseTrackList, false, streamItem.Track.ToBaseTrack());



                        if (!startPlayback.success)
                            await new MessageDialog(startPlayback.message, "Error playing stream.").ShowAsync();
                    }
                    break;
                case "playlist":
                case "playlist-repost":
                    App.NavigateTo(typeof(PlaylistView), streamItem.Playlist.ToBasePlaylist());
                    break;
            }

            // We are not loading
            App.IsLoading = false;
        }

        public override void Dispose()
        {
            // Only clean if we are in the background
            if (!DeviceHelper.IsBackground)
                return;

            GC.Collect();
        }
    }
}