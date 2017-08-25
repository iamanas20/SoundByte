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
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundByte.Core.Helpers;
using SoundByte.Core.Services;
using SoundByte.UWP.Views.Application;

namespace SoundByte.UWP.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public bool IsComboboxBlockingEnabled { get; set; }

        public async void ClearAppCache()
        {
            // Create a message dialog
            var dialog = new ContentDialog
            {
                Title = "Clear Application Cache?",
                Content = new TextBlock
                {
                    Text =
                        "Warning: Clearing Application cache will delete the following things:\n• Cached Images.\n• Jumplist Items.\n• Pinned Live Tiles.\n• Notifications.\n\n To Continue press 'Clear Cache', this may take a while.",
                    TextWrapping = TextWrapping.Wrap
                },
                PrimaryButtonText = "Clear Cache",
                SecondaryButtonText = "Cancel",
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = true
            };

            var response = await dialog.ShowAsync();

            if (response != ContentDialogResult.Primary)
                return;

            // Clear all jumplist items
            await JumplistHelper.RemoveAllAsync();
            // Clear all the live tiles
            await TileService.Instance.RemoveAllAsync();
            // Remove all cached images from the app
            var rootCacheFolder =
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("cache",
                    CreationCollisionOption.OpenIfExists);
            await rootCacheFolder.DeleteAsync();
            // Remove all toast notifications
            ToastNotificationManager.History.Clear();
        }

        public void NavigateDebugOptions()
        {
            App.NavigateTo(typeof(DebugView));
        }

        /// <summary>
        ///     Changes the language string and pompts the user to restart the app.
        /// </summary>
        public async void ChangeLangauge(object sender, SelectionChangedEventArgs e)
        {
            if (IsComboboxBlockingEnabled)
                return;

            // Get the langauge string
            var comboBoxItem = (ComboBoxItem) ((ComboBox) sender).SelectedItem;
            if (comboBoxItem != null)
            {
                var languageString = comboBoxItem.Tag as string;

                // If the langauge is the same, do nothing
                if (SettingsService.Instance.CurrentAppLanguage == languageString || IsComboboxBlockingEnabled ||
                    string.IsNullOrEmpty(SettingsService.Instance.CurrentAppLanguage))
                    return;

                // Set the current langauge
                SettingsService.Instance.CurrentAppLanguage = languageString;
            }
            // Get the resource loader
            var resources = ResourceLoader.GetForCurrentView();
            // Create the app restart dialog
            var restartAppDialog = new ContentDialog
            {
                Title = resources.GetString("LanguageRestart_Title"),
                Content = new TextBlock
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = resources.GetString("LanguageRestart_Content")
                },
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = resources.GetString("LanguageRestart_Button")
            };
            // Show the dialog and get the respose
            var response = await restartAppDialog.ShowAsync();
            // Restart the app if the user canceled or clicked the button
            if (response == ContentDialogResult.Primary || response == ContentDialogResult.None ||
                response == ContentDialogResult.Secondary)
                Application.Current.Exit();
        }
    }
}