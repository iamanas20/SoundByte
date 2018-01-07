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

using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Sources;
using SoundByte.Core.Sources.SoundCloud;

namespace SoundByte.UWP.ViewModels
{
    /// <summary>
    ///     The view model for the HomeView page
    /// </summary>
    public class SoundCloudStreamViewModel : BaseViewModel
    {
        public SoundByteCollection<SoundCloudStreamSource, GroupedItem> StreamItems { get; } =
            new SoundByteCollection<SoundCloudStreamSource, GroupedItem>();


        public async void PlayAllStreamTracks()
        {
            // We are loading
            StreamItems.IsLoading = true;

            // Get a list of items
            var trackList = StreamItems.Where(t => t.Type == ItemType.Track && t.Track != null)
                .Select(t => t.Track);

            var startPlayback = await PlaybackService.Instance.InitilizePlaylistAsync<DummyTrackSource>(trackList);

            if (!startPlayback.Success)
            {
                await new MessageDialog(startPlayback.Message, "Error playing stream.").ShowAsync();
                StreamItems.IsLoading = false;
                return;
            }

            await PlaybackService.Instance.StartTrackAsync();

            // We are not loading
            StreamItems.IsLoading = false;
        }

        public async void PlayShuffleStreamTracks()
        {
            // Get a list of items
            var trackList = StreamItems.Where(t => t.Type == ItemType.Track && t.Track != null)
                .Select(t => t.Track);

            // Shuffle and play the items
            await ShuffleTracksListAsync(trackList);
        }

        public async void NavigateStream(object sender, ItemClickEventArgs e)
        {
            // Get a list of items
            var trackList = StreamItems.Where(t => t.Type == ItemType.Track && t.Track != null)
                .Select(t => t.Track);

            // Get the clicked item
            var streamItem = (GroupedItem) e.ClickedItem;

            if (streamItem == null)
                return;

            // We are loading
            StreamItems.IsLoading = true;

            switch (streamItem.Type)
            {
                case ItemType.Track:
                    if (streamItem.Track != null)
                    {
                        var startPlayback = await PlaybackService.Instance.InitilizePlaylistAsync<DummyTrackSource>(trackList);

                        if (!startPlayback.Success)
                        {
                            await new MessageDialog(startPlayback.Message, "Error playing stream.").ShowAsync();
                            StreamItems.IsLoading = false;
                            return;
                        }

                        await PlaybackService.Instance.StartTrackAsync(streamItem.Track);
                    }
                    break;
                case ItemType.Playlist:
                    if (streamItem.Playlist != null)
                    {
                       // var gridView = App.CurrentFrame.FindName("StreamListView") as GridView;
                       // gridView?.PrepareConnectedAnimation("PlaylistImage", streamItem.Playlist, "ImagePanel");

                        App.NavigateTo(typeof(PlaylistView), streamItem.Playlist);
                    }

                    break;
            }

            // We are not loading
            StreamItems.IsLoading = false;
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