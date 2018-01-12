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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources;
using SoundByte.UWP.Services;
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.ViewModels
{
    /// <summary>
    ///     Base class for all view models to extend off of
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        ///     Dispose the model
        /// </summary>
        public virtual void Dispose()
        {
            // Collect memory if in background
         //   if (DeviceHelper.IsBackground)
         //   {
                GC.Collect();
         //   }
        }

        public static async Task ShuffleTracksListAsync(IEnumerable<BaseTrack> tracks)
        {
            var initPlaylistResponse = await PlaybackService.Instance.InitilizePlaylistAsync<DummyTrackSource>(tracks);

            if (initPlaylistResponse.Success == false)
            {
                await NavigationService.Current.CallMessageDialogAsync(initPlaylistResponse.Message,
                    "Could not build playlist.");
                return;
            }

            await PlaybackService.Instance.StartRandomTrackAsync();
        }

        /// <summary>
        ///     Helper method for PlayAllTracksAsync to play shuffled tracks. See 
        ///     PlayAllTracksAsync for more information
        /// </summary>
        /// <typeparam name="TSource">A base track source.</typeparam>
        /// <param name="model">SoundByte collection of a base track source.</param>
        public static async Task ShufflePlayAllTracksAsync<TSource>(SoundByteCollection<TSource, BaseTrack> model) where TSource : ISource<BaseTrack>
        {
            await PlayAllTracksAsync(model, null, true);
        }

        /// <summary>
        ///     Attempts to play a model of type BaseTrack. This method handles playback and loading 
        ///     feedback to the user. If an error occured, the user will see a message dialog.
        /// </summary>
        /// <typeparam name="TSource">A base track source.</typeparam>
        /// <param name="model">SoundByte collection of a base track source.</param>
        /// <param name="startingTrack">The track to start first (can be null)</param>
        /// <param name="shuffle">Should the playlist be shuffled.</param>
        public static async Task PlayAllTracksAsync<TSource>(SoundByteCollection<TSource, BaseTrack> model, BaseTrack startingTrack = null, bool shuffle = false) where TSource : ISource<BaseTrack>
        {
            if (model.Count == 0)
                return;

            model.IsLoading = true;

            // Attempt to create the playlist
            var result = await PlaybackService.Instance.InitilizePlaylistAsync(model.Source, model, model.Token);

            if (result.Success == false)
            {
                model.IsLoading = false;

                await NavigationService.Current.CallMessageDialogAsync(result.Message, "Could not build playlist.");

                return;
            }

            // Start playback
            if (shuffle)
            {
                await PlaybackService.Instance.StartRandomTrackAsync();
            }
            else
            {
                await PlaybackService.Instance.StartTrackAsync(startingTrack);
            }

            model.IsLoading = false;
        }


        #region Property Changed Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}