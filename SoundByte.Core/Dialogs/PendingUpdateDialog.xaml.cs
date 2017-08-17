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
using Windows.Services.Store;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp;
using SoundByte.Core.Services;

namespace SoundByte.Core.Dialogs
{
    public sealed partial class PendingUpdateDialog
    {
        public PendingUpdateDialog()
        {
            InitializeComponent();
        }

        public void DeferUpdate()
        {
            TelemetryService.Instance.TrackEvent("Defer Update");
            Hide();
        }

        private async void UpdateNow(object sender, RoutedEventArgs e)
        {
            ProgressRing.Visibility = Visibility.Visible;

            // Setup the UI
            UpdateBar.Visibility = Visibility.Visible;
            UpdateButton.IsEnabled = false;
            CloseButton.IsEnabled = false;

            // Get a list of updates
            var updates = await StoreContext.GetDefault().GetAppAndOptionalStorePackageUpdatesAsync();

            // Download and install the updates.
            var downloadOperation = StoreContext.GetDefault()
                .RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

            // The Progress async method is called one time for each step in the download
            // and installation process for each package in this request.
            downloadOperation.Progress = async (asyncInfo, progress) =>
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    UpdateBar.Value = progress.PackageDownloadProgress;
                });
            };

            await downloadOperation.AsTask();

            ProgressRing.Visibility = Visibility.Collapsed;
            Hide();
        }
    }
}