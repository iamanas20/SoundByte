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
using SoundByte.UWP.Models;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views.Search
{
    public sealed partial class SearchTrackView 
    {
        public BaseTrackModel Model { get; set; }

        public SearchTrackView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Model?.Clear();
            Model = (BaseTrackModel) e.Parameter;        
            TextHeader.Text = Model.ModelHeader;
            MobileTitle.Text = Model.ModelHeader;
            ModelType.Text = Model.ModelType;
        }

        public async void PlayShuffleItems()
        {
            await BaseViewModel.ShuffleTracksAsync(Model);
        }

        public async void PlayAllItems()
        {
            // We are loading
            App.IsLoading = true;

            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(Model);

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing likes.").ShowAsync();

            // We are not loading
            App.IsLoading = false;
        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            var startPlayback =
                await PlaybackService.Instance.StartModelMediaPlaybackAsync(Model, false, (BaseTrack)e.ClickedItem);

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing likes.").ShowAsync();

            // We are not loading
            App.IsLoading = false;
        }
    }
}
