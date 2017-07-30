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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Overlay : INotifyPropertyChanged
    {
        public BaseViewModel ViewModel { get; } = new BaseViewModel();

        #region Property Changed Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        // The content on the play_pause button
        private string _playButtonContent = "\uE769";

        public string PlayButtonContent
        {
            get => _playButtonContent;
            set
            {
                if (_playButtonContent == value)
                    return;

                _playButtonContent = value;
                UpdateProperty();
            }
        }

        private readonly CoreDispatcher _dispatcher;

        public Overlay()
        {
            InitializeComponent();

            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            ViewModel.Service.PropertyChanged += Service_PropertyChanged;

            // Set the accent color
            TitlebarHelper.UpdateTitlebarStyle();

            BackgroundImage.Source = new BitmapImage(new Uri(ArtworkConverter.ConvertObjectToImage(ViewModel.Service.CurrentTrack)));
            TrackTitle.Text = ViewModel.Service.CurrentTrack.Title;
            TrackUser.Text = ViewModel.Service.CurrentTrack.User.Username;
            BackgroundImage.Blur(18).Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("Compact Overlay Page");
        }

        private async void Service_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentTrack":
                    await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        BackgroundImage.Source = new BitmapImage(new Uri(ArtworkConverter.ConvertObjectToImage(ViewModel.Service.CurrentTrack)));
                        TrackTitle.Text = ViewModel.Service.CurrentTrack.Title;
                        TrackUser.Text = ViewModel.Service.CurrentTrack.User.Username;

                    });
                    break;
                case "PlayButtonContent":
                    await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        PlayButtonContent = ViewModel.Service.PlayButtonContent;
                    });
                    break;
            }
        }
    }
}