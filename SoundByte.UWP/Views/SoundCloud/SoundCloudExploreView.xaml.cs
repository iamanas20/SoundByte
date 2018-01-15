using System.Linq;
using Windows.UI.Xaml.Controls;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views.SoundCloud
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SoundCloudExploreView
    {
        public SoundByteCollection<SoundCloudExploreSource, BaseTrack> ChartsModel { get; } =
            new SoundByteCollection<SoundCloudExploreSource, BaseTrack>();

        public SoundCloudExploreView()
        {
            InitializeComponent();
        }

        public async void PlayAllChartItems()
        {
            if (ChartsModel.FirstOrDefault() == null)
                return;

            await BaseViewModel.PlayAllTracksAsync(ChartsModel);
        }

        public async void PlayShuffleChartItems()
        {
            await BaseViewModel.ShufflePlayAllTracksAsync(ChartsModel);
        }

        public async void PlayChartItem(object sender, ItemClickEventArgs e)
        {
            await BaseViewModel.PlayAllTracksAsync(ChartsModel, (BaseTrack)e.ClickedItem);
        }

        /// <summary>
        ///     Combobox for trending selection and
        ///     type of song.
        /// </summary>
        public void OnComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            // Dislay the loading ring
            ChartsModel.IsLoading = true;

            // Get the combo box
            var comboBox = sender as ComboBox;

            // See which combo box we got
            switch (comboBox?.Name)
            {
                case "ExploreTypeCombo":
                    ChartsModel.Source.Kind = (comboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
                    break;
                case "ExploreGenreCombo":
                    ChartsModel.Source.Genre = (comboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
                    break;
            }

            ChartsModel.RefreshItems();

            // Hide loading ring
            ChartsModel.IsLoading = false;
        }
    }
}
