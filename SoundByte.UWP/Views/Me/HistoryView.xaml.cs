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
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    ///     View to view messages
    /// </summary>
    public sealed partial class HistoryView
    {
        public HistoryView()
        {
            InitializeComponent();
        }

        public SoundByteCollection<HistorySource, BaseTrack> HistoryModel { get; } =
            new SoundByteCollection<HistorySource, BaseTrack>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Instance.TrackPage("History View");
        }

        public async void PlayShuffleItems()
        {
            await BaseViewModel.ShuffleTracksAsync(HistoryModel);
        }

        public async void PlayAllItems()
        {
            HistoryModel.IsLoading = true;

            var startPlayback =
                await PlaybackService.Instance.StartModelMediaPlaybackAsync(HistoryModel);
           if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();

            HistoryModel.IsLoading = false;
        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            HistoryModel.IsLoading = true;

            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(HistoryModel, false, (BaseTrack) e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();

            HistoryModel.IsLoading = false;
        }
    }
}