using Windows.UI.Xaml.Controls;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources;
using SoundByte.UWP.Helpers;

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
        /// <param name="data">The track modek to use in this view</param>
        public void Init(TrackViewModelHolder data)
        {
            Model = new SoundByteCollection<ISource<BaseTrack>, BaseTrack>(data.Track);
            Title = data.Title;
        }
        #endregion

        public class TrackViewModelHolder
        {
            public TrackViewModelHolder()
            { }

            public TrackViewModelHolder(ISource<BaseTrack> track, string title)
            {
                Track = track;
                Title = title;
            }

            public ISource<BaseTrack> Track { get; set; }
            public string Title { get; set; }
        }

        #region Method Bindings
        public async void PlayShuffleItems()
        {
            await ShufflePlayAllTracksAsync(Model);
        }

        public async void PlayAllItems()
        {
            await PlayAllTracksAsync(Model);
        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            await PlayAllTracksAsync(Model, (BaseTrack) e.ClickedItem);
        }
        #endregion
    }
}
