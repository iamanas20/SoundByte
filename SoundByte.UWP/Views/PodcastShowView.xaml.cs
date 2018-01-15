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

using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Items.Podcast;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    public sealed partial class PodcastShowView
    {
        public PodcastShowViewModel ViewModel { get; } = new PodcastShowViewModel();

        public PodcastShowView()
        {
            InitializeComponent();

            Unloaded += (s, e) =>
            {
                ViewModel.Dispose();
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init(e.Parameter as BasePodcast);
        }
    }
}