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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Helpers;
using SoundByte.Core.Sources;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///    Acts are a wrapper arround the new service
    /// </summary>
    [Obsolete]
    public class PlaybackService 
    {
        // WRAP ALL LOGIN INTO VIEW MODEL
        public PlaybackViewModel ViewModel { get; } = new PlaybackViewModel();

        public ObservableCollection<BaseTrack> Playlist
        {
            get => _playlist;
            private set
            {
                _playlist = value;
            }
        }
        private ObservableCollection<BaseTrack> _playlist = new ObservableCollection<BaseTrack>();

        #region Service Setup
        private static readonly Lazy<PlaybackService> InstanceHolder =
            new Lazy<PlaybackService>(() => new PlaybackService());

        public static PlaybackService Instance => InstanceHolder.Value;
        #endregion

        // WRAPPERS
        public void ToggleRepeat() => ViewModel.ToggleRepeat();
        public void ToggleShuffle() => ViewModel.ToggleShuffle();
        public void ToggleMute() => ViewModel.ToggleMute();  
        public void ChangePlaybackState() => ViewModel.ChangePlaybackState();
        public void SkipNext() => ViewModel.SkipNext();
        public void SkipPrevious() => ViewModel.SkipPrevious();

   
        public class PlaybackResponse
        {
            public PlaybackResponse(bool success, string messsage)
            {
                Success = success;
                Message = messsage;
            }

            public string Message { get; set; }
            public bool Success { get; set; }
        }

        [Obsolete("This method restricts future development")]
        public async Task<PlaybackResponse> StartPlaylistMediaPlaybackAsync(IEnumerable<BaseTrack> playlist,
            bool isShuffled = false, BaseTrack startingItem = null)
        {
            // Create a dummy base track items
            var dummyBaseTrackModel = new SoundByteCollection<DummyTrackSource, BaseTrack> { Token = "eol" };

            // Add all the playlist items to this dummy
            foreach (var track in playlist)
                dummyBaseTrackModel.Add(track);

            // Call the normal method
            return await StartModelMediaPlaybackAsync(dummyBaseTrackModel, isShuffled, startingItem);
        }

        public async Task<PlaybackResponse> StartModelMediaPlaybackAsync<TSource>(SoundByteCollection<TSource, BaseTrack> trackSource,
            bool isShuffled = false, BaseTrack startingItem = null) where TSource : ISource<BaseTrack>
        {
            // WRAP V2 SERVICE

            await App.SetLoadingAsync(true);

            var result = await PlaybackV2Service.Instance.InitilizePlaylistAsync<TSource>(trackSource, trackSource.Token);

            if (!result.Success)
                return new PlaybackResponse(false, result.Message);

            await PlaybackV2Service.Instance.StartTrackAsync(startingItem);

            await App.SetLoadingAsync(false);

            return new PlaybackResponse(true, string.Empty);
        }

        public  void PlayingSliderChange()
        {
            ViewModel.OnPlayingSliderChange();
        }
    }
}