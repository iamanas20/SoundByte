using Windows.UI.Xaml.Controls;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Sources.SoundByte;
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.Views.Collection
{
    public sealed partial class PodcastsView
    {
        public SoundByteCollection<SoundBytePodcastsSource, BasePodcast> SoundBytePodcasts { get; } = 
            new SoundByteCollection<SoundBytePodcastsSource, BasePodcast>();

        public PodcastsView()
        {
            InitializeComponent();
        }

        public void NavigatePodcastShow(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            App.NavigateTo(typeof(PodcastShowView), e.ClickedItem as BasePodcast);
        }
    }
}