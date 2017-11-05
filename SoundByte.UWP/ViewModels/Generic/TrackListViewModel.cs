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
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.ViewModels.Generic
{
    public class TrackListViewModel : BaseViewModel
    {
        #region Bindings
        /// <summary>
        /// The title to be displayed on the search page.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (value == _title)
                    return;

                _title = value;
                UpdateProperty();
            }
        }
        private string _title;

        /// <summary>
        /// Subtitle to be displayed on the search page.
        /// </summary>
        public string SubTitle
        {
            get => _subTitle;
            set
            {
                if (value == _subTitle)
                    return;

                _subTitle = value;
                UpdateProperty();
            }
        }
        private string _subTitle;


        /// <summary>
        /// The track model to show on this page
        /// </summary>
        public SoundByteCollection<ISource<BaseTrack>, BaseTrack> Model
        {
            get => _model;
            set
            {
                if (value == _model)
                    return;

                _model = value;
                UpdateProperty();
            }
        }
        private SoundByteCollection<ISource<BaseTrack>, BaseTrack> _model;
        #endregion

        #region Methods
        /// <summary>
        /// Setup the view model for use
        /// </summary>
        /// <param name="model">The track modek to use in this view</param>
        public void Init(SoundByteCollection<ISource<BaseTrack>, BaseTrack> model)
        {
            Model = model;
            Title = model.ModelHeader;
            SubTitle = model.ModelType;
        }
        #endregion

        #region Method Bindings
        public async void PlayShuffleItems()
        {
            await ShuffleTracksAsync(Model);
        }

        public async void PlayAllItems()
        {
            // We are loading
            Model.IsLoading = true;

            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(Model);

            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Playback Error").ShowAsync();

            // We are not loading
            Model.IsLoading = false;
        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            // We are loading
            Model.IsLoading = true;

            var startPlayback =
                await PlaybackService.Instance.StartModelMediaPlaybackAsync(Model, false, (BaseTrack)e.ClickedItem);

            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Playback Error").ShowAsync();

            // We are not loading
            Model.IsLoading = false;
        }
        #endregion
    }
}
