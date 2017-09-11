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
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.UWP.Services;
using SoundByte.UWP.Models;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    ///     Lets the user view their likes
    /// </summary>
    public sealed partial class LikesView
    {
        public LikesView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The likes model that contains or the users liked tracks
        /// </summary>
        private LikeModel LikesModel { get; } = new LikeModel(SoundByteV3Service.Current.GetConnectedUser(ServiceType.SoundCloud));

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Instance.TrackPage("User Likes View");
        }

        public async void PlayShuffleItems()
        {
            await BaseViewModel.ShuffleTracksAsync(LikesModel.ToList(), LikesModel.Token);
        }

        public async void PlayAllItems()
        {
            // We are loading
            App.IsLoading = true;

            var startPlayback = await PlaybackService.Instance.StartMediaPlayback(LikesModel.ToList(), LikesModel.Token);

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
                await PlaybackService.Instance.StartMediaPlayback(LikesModel.ToList(), LikesModel.Token, false,
                    (BaseTrack) e.ClickedItem);

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing likes.").ShowAsync();

            // We are not loading
            App.IsLoading = false;
        }
    }
}