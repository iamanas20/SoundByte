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
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.Fanburst;
using SoundByte.Core.Sources.SoundByte;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.YouTube;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views.Generic;
using SoundByte.UWP.Views.ImportViews;

namespace SoundByte.UWP.Views.CollectionViews
{
    public sealed partial class LikesView
    {
        public SoundByteCollection<SoundByteLikeSource, BaseTrack> SoundByteLikes { get; } =
            new SoundByteCollection<SoundByteLikeSource, BaseTrack>();

        public SoundByteCollection<SoundCloudLikeSource, BaseTrack> SoundCloudLikes { get; } =
            new SoundByteCollection<SoundCloudLikeSource, BaseTrack>();

        public SoundByteCollection<YouTubeLikeSource, BaseTrack> YouTubeLikes { get; } =
            new SoundByteCollection<YouTubeLikeSource, BaseTrack>();

        public SoundByteCollection<FanburstLikeSource, BaseTrack> FanburstLikes { get; } =
            new SoundByteCollection<FanburstLikeSource, BaseTrack>();

        public LikesView()
        {
            InitializeComponent();

            SoundCloudLikes.Source.User = SoundByteService.Current.GetConnectedUser(ServiceType.SoundCloud);
        }

        public async void PlaySoundCloudLikes(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(SoundCloudLikes, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();
        }


       

        public async void PlayShuffleSoundCloud()
        {
            await BaseViewModel.ShufflePlayAllTracksAsync(SoundCloudLikes);
        }

        public async void PlaySoundCloud()
        {
            await BaseViewModel.PlayAllTracksAsync(SoundCloudLikes);
        }

        public void ImportSoundCloud()
        {
            App.NavigateTo(typeof(WelcomeView));
        }


        #region Navigate More

        public void NavigateMoreSoundCloudLikes()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = SoundCloudLikes.Source,
                Title = "Likes"
                
            });
        }

        #endregion
    }
}