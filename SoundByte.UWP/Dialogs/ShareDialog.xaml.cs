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

using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class ShareDialog
    {
        public ShareDialog(BaseTrack trackItem)
        {
            // Do this before the xaml is loaded, to make sure
            // the object can be binded to.
            Track = trackItem;

            // Load the XAML page
            InitializeComponent();
        }

        public BaseTrack Track { get; }

        private void ShareWindows(object sender, RoutedEventArgs e)
        {
            // Create a share event
            void ShareEvent(DataTransferManager s, DataRequestedEventArgs a)
            {
                var dataPackage = a.Request.Data;
                dataPackage.Properties.Title = "SoundByte";
                dataPackage.Properties.Description = "Share this track with Windows 10.";
                dataPackage.SetText("Listen to " + Track.Title + " by " + Track.User.Username +
                                    " on #SoundByte #Windows10 " + Track.Link);
            }

            // Remove any old share events
            DataTransferManager.GetForCurrentView().DataRequested -= ShareEvent;
            // Add this new share event
            DataTransferManager.GetForCurrentView().DataRequested += ShareEvent;
            // Show the share dialog
            DataTransferManager.ShowShareUI();
            // Hide the popup
            Hide();
            // Track Event
            App.Telemetry.TrackEvent("Share Menu - Windows Share");
        }

        private void ShareLink(object sender, RoutedEventArgs e)
        {
            // Create a data package
            var data = new DataPackage {RequestedOperation = DataPackageOperation.Copy};
            // Set the link to the track on soundcloud
            data.SetText(Track.Link);
            // Set the clipboard content
            Clipboard.SetContent(data);
            // Hide the popup
            Hide();
            // Track Event
            App.Telemetry.TrackEvent("Share Menu - Copy General Link");
        }

        private void ShareSoundByte(object sender, RoutedEventArgs e)
        {
            // Create a data package
            var dataPackage = new DataPackage {RequestedOperation = DataPackageOperation.Copy};
            // Set the link to the track on soundcloud
            dataPackage.SetText($"soundbyte://core/track?id={Track.Id}&service={Track.ServiceType.ToString().ToLower()}");
            // Set the clipboard content
            Clipboard.SetContent(dataPackage);
            // Hide the popup
            Hide();
            // Track Event
            App.Telemetry.TrackEvent("Share Menu - Copy SoundByte Link");
        }
    }
}