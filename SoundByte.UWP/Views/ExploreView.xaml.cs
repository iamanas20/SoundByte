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
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Models;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExploreView 
    {
        /// <summary>
        ///     The likes model that contains or the users liked tracks
        /// </summary>
        public ChartModel ChartsModel { get; } = new ChartModel();

        public ExploreView()
        {
            InitializeComponent();
        }

        public async void PlayAllChartItems()
        {
            if (ChartsModel.FirstOrDefault() == null)
                return;

            var startPlayback =
                await PlaybackService.Instance.StartModelMediaPlaybackAsync(ChartsModel);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();
        }

        public async void PlayShuffleChartItems()
        {
            await BaseViewModel.ShuffleTracksAsync(ChartsModel);
        }

        public async void PlayChartItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(ChartsModel, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();
        }

        /// <summary>
        ///     Combobox for trending selection and
        ///     type of song.
        /// </summary>
        public void OnComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            // Dislay the loading ring
            App.IsLoading = true;

            // Get the combo box
            var comboBox = sender as ComboBox;

            // See which combo box we got
            switch (comboBox?.Name)
            {
                case "ExploreTypeCombo":
                    ChartsModel.Kind = (comboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
                    break;
                case "ExploreGenreCombo":
                    ChartsModel.Genre = (comboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
                    break;
            }

            ChartsModel.RefreshItems();

            // Hide loading ring
            App.IsLoading = false;
        }
    }
}
