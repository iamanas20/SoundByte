using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels.Generic;
using SoundByte.UWP.Views.Generic;

namespace SoundByte.UWP.Views.MyCollection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LikesView
    {
        public SoundByteCollection<LikeSoundCloudSource, BaseTrack> SoundCloudLikes { get; } =
            new SoundByteCollection<LikeSoundCloudSource, BaseTrack>();


        public LikesView()
        {
            InitializeComponent();

            SoundCloudLikes.Source.User = SoundByteV3Service.Current.GetConnectedUser(ServiceType.SoundCloud);

        }


        public async void PlaySoundCloudLikes(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Instance.StartModelMediaPlaybackAsync(SoundCloudLikes, false, (BaseTrack)e.ClickedItem);
            if (!startPlayback.Success)
                await new MessageDialog(startPlayback.Message, "Error playing track.").ShowAsync();
        }


        public void NavigateMoreSoundCloudLikes()
        {
            App.NavigateTo(typeof(TrackListView), new TrackListViewModel.TrackViewModelHolder
            {
                Track = SoundCloudLikes.Source,
                Title = "SoundCloud",
                Subtitle = "Likes"
            });
        }
    }
}
