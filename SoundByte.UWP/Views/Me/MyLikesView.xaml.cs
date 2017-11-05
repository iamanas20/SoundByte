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
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    ///     Lets the user view their likes
    /// </summary>
    public sealed partial class MyLikesView
    {
        public MyLikesView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The likes model that contains the users liked tracks
        /// </summary>
        public SoundByteCollection<LikeSoundCloudSource, BaseTrack> LikesModel { get; } =
            new SoundByteCollection<LikeSoundCloudSource, BaseTrack>();


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Instance.TrackPage("User Likes View");

            LikesModel.Source.User = SoundByteV3Service.Current.GetConnectedUser(ServiceType.SoundCloud);
        }

        public async void PlayShuffleItems()
        {
            await BaseViewModel.ShuffleTracksAsync(LikesModel);
        }

        public async void PlayAllItems()
        {
            // We are loading
            LikesModel.IsLoading = true;

            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(LikesModel);

            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing likes.").ShowAsync();

            // We are not loading
            LikesModel.IsLoading = false;
        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            // We are loading
            LikesModel.IsLoading = true;

            var startPlayback =
                await PlaybackService.Instance.StartModelMediaPlaybackAsync(LikesModel, false, (BaseTrack) e.ClickedItem);

            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing likes.").ShowAsync();

            // We are not loading
            LikesModel.IsLoading = false;
        }
    }
}