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

using Windows.UI.Xaml.Controls;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.Fanburst;
using SoundByte.Core.Sources.SoundByte;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.YouTube;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views.Generic;

namespace SoundByte.UWP.Views.Collection
{
    public sealed partial class LikesView
    {
        public LikesView()
        {
            InitializeComponent();

            SoundCloudLikes.Source.User = SoundByteService.Current.GetConnectedUser(ServiceType.SoundCloud);
        }

        #region SoundByte
        /// <summary>
        ///     Collection and model for showing SoundByte likes.
        /// </summary>
        public SoundByteCollection<SoundByteLikeSource, BaseTrack> SoundByteLikes { get; } =
            new SoundByteCollection<SoundByteLikeSource, BaseTrack>();

        /// <summary>
        ///     Play all SoundByte likes starting with this one.
        /// </summary>
        public async void PlaySoundByteItem(object sender, ItemClickEventArgs e) => 
            await BaseViewModel.PlayAllTracksAsync(SoundByteLikes, (BaseTrack)e.ClickedItem);

        /// <summary>
        ///     Shuffle play all SoundByte likes
        /// </summary>
        public async void ShuffleSoundByteItems() =>
            await BaseViewModel.ShufflePlayAllTracksAsync(SoundByteLikes);

        /// <summary>
        ///     Play all SoundByte likes
        /// </summary>
        public async void PlayAllSoundByteItems() => 
            await BaseViewModel.PlayAllTracksAsync(SoundByteLikes);

        /// <summary>
        ///     Navigate to see all SoundByte likes.
        /// </summary>
        public void NavigateAllSoundByteItems() => App.NavigateTo(typeof(TrackListView),
            new TrackListViewModel.TrackViewModelHolder(SoundByteLikes.Source, "Likes"));
        #endregion

        #region SoundCloud
        /// <summary>
        ///     Collection and model for showing SoundCloud likes.
        /// </summary>
        public SoundByteCollection<SoundCloudLikeSource, BaseTrack> SoundCloudLikes { get; } =
            new SoundByteCollection<SoundCloudLikeSource, BaseTrack>();

        /// <summary>
        ///     Play all SoundCloud likes starting with this one.
        /// </summary>
        public async void PlaySoundCloudItem(object sender, ItemClickEventArgs e) =>
            await BaseViewModel.PlayAllTracksAsync(SoundCloudLikes, (BaseTrack)e.ClickedItem);

        /// <summary>
        ///     Shuffle play all SoundCloud likes
        /// </summary>
        public async void ShuffleSoundCloudItems() =>
            await BaseViewModel.ShufflePlayAllTracksAsync(SoundCloudLikes);

        /// <summary>
        ///     Play all SoundCloud likes
        /// </summary>
        public async void PlayAllSoundCloudItems() =>
            await BaseViewModel.PlayAllTracksAsync(SoundCloudLikes);

        /// <summary>
        ///     Navigate to see all SoundCloud likes.
        /// </summary>
        public void NavigateAllSoundCloudItems() => App.NavigateTo(typeof(TrackListView),
            new TrackListViewModel.TrackViewModelHolder(SoundCloudLikes.Source, "Likes"));
        #endregion

        #region YouTube
        /// <summary>
        ///     Collection and model for showing YouTube likes.
        /// </summary>
        public SoundByteCollection<YouTubeLikeSource, BaseTrack> YouTubeLikes { get; } =
            new SoundByteCollection<YouTubeLikeSource, BaseTrack>();

        /// <summary>
        ///     Play all YouTube likes starting with this one.
        /// </summary>
        public async void PlayYouTubeItem(object sender, ItemClickEventArgs e) =>
            await BaseViewModel.PlayAllTracksAsync(YouTubeLikes, (BaseTrack)e.ClickedItem);

        /// <summary>
        ///     Shuffle play all YouTube likes
        /// </summary>
        public async void ShuffleYouTubeItems() =>
            await BaseViewModel.ShufflePlayAllTracksAsync(YouTubeLikes);

        /// <summary>
        ///     Play all YouTube likes
        /// </summary>
        public async void PlayAllYouTubeItems() =>
            await BaseViewModel.PlayAllTracksAsync(YouTubeLikes);

        /// <summary>
        ///     Navigate to see all YouTube likes.
        /// </summary>
        public void NavigateAllYouTubeItems() => App.NavigateTo(typeof(TrackListView),
            new TrackListViewModel.TrackViewModelHolder(YouTubeLikes.Source, "Likes"));
        #endregion

        #region Fanburst
        /// <summary>
        ///     Collection and model for showing Fanburst likes.
        /// </summary>
        public SoundByteCollection<FanburstLikeSource, BaseTrack> FanburstLikes { get; } =
            new SoundByteCollection<FanburstLikeSource, BaseTrack>();
       
        /// <summary>
        ///     Play all Fanburst likes starting with this one.
        /// </summary>
        public async void PlayFanburstItem(object sender, ItemClickEventArgs e) =>
            await BaseViewModel.PlayAllTracksAsync(FanburstLikes, (BaseTrack)e.ClickedItem);

        /// <summary>
        ///     Shuffle play all Fanburst likes
        /// </summary>
        public async void ShuffleFanburstItems() =>
            await BaseViewModel.ShufflePlayAllTracksAsync(FanburstLikes);

        /// <summary>
        ///     Play all Fanburst likes
        /// </summary>
        public async void PlayAllFanburstItems() =>
            await BaseViewModel.PlayAllTracksAsync(FanburstLikes);
        
        /// <summary>
        ///     Navigate to see all Fanburst likes.
        /// </summary>
        public void NavigateAllFanburstItems() => App.NavigateTo(typeof(TrackListView),
            new TrackListViewModel.TrackViewModelHolder(FanburstLikes.Source, "Likes"));
        #endregion
    }
}