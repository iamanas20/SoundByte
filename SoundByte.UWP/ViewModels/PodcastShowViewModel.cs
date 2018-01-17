using Windows.UI.Xaml.Controls;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources.Podcast;
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.ViewModels
{
    public class PodcastShowViewModel : BaseViewModel
    {
        #region Model
        public SoundByteCollection<PodcastItemsSource, BaseTrack> PodcastItems { get; } = new SoundByteCollection<PodcastItemsSource, BaseTrack>();
        #endregion

        #region Getters and Setters
        /// <summary>
        ///     Gets or sets the current podcast show item
        /// </summary>
        public BasePodcast PodcastShow
        {
            get => _podcastShow;
            set
            {
                if (value == _podcastShow) return;

                _podcastShow = value;
                UpdateProperty();
            }
        }
        private BasePodcast _podcastShow;
        #endregion

        public void Init(BasePodcast show)
        {
            PodcastShow = show;

            PodcastItems.Source.Show = show;
            PodcastItems.RefreshItems();
        }

        /// <summary>
        ///     Shuffles the tracks in the podcast
        /// </summary>
        public async void ShuffleItemsAsync()
        {
            await ShuffleTracksListAsync(PodcastItems);
        }

        public async void PlayPodcast(object sender, ItemClickEventArgs e)
        {
            await PlayAllTracksAsync(PodcastItems, e.ClickedItem as BaseTrack);
        }

        /// <summary>
        ///     Starts playing the playlist
        /// </summary>
        public async void NavigatePlay()
        {
            await PlayAllTracksAsync(PodcastItems);
        }
    }
}