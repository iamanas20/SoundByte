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
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundByte.API.Endpoints;
using SoundByte.API.Items.Track;
using SoundByte.Core.Services;
using SoundByte.UWP.Models;
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

        public HistoryModel HistoryModel { get; } = new HistoryModel();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Instance.TrackPage("History View");
        }

        public async void PlayShuffleItems()
        {
            await BaseViewModel.ShuffleTracksAsync(HistoryModel.ToList(), HistoryModel.Token);
        }

        public async void PlayAllItems()
        {
            App.IsLoading = true;

            var startPlayback =
                await PlaybackService.Instance.StartMediaPlayback(HistoryModel.ToList(), HistoryModel.Token);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();

            App.IsLoading = false;
        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            App.IsLoading = true;

            var startPlayback = await PlaybackService.Instance.StartMediaPlayback(HistoryModel.ToList(),
                HistoryModel.Token, false, (BaseTrack) e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();

            App.IsLoading = false;
        }
    }
}