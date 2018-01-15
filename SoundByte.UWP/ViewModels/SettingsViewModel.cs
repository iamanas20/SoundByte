using System;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views.Settings;

namespace SoundByte.UWP.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public bool IsComboboxBlockingEnabled { get; set; }

        public async void ClearAppCache()
        {
            var dialog = new MessageDialog(
                "Warning: Clearing Application cache will delete the following things:\n• Cached Images.\n• Jumplist Items.\n• Pinned Live Tiles.\n• Local Playback History.\n\n To Continue press 'Clear Cache', this may take a while."
                , "Clear Application Cache?");
            dialog.Commands.Add(new UICommand("Clear Cache", null, 0));
            dialog.Commands.Add(new UICommand("Cancel", null, 1));

            var response = await dialog.ShowAsync();

            if ((int)response.Id == 1)
                return;

            // Clear all the live tiles
            await TileHelper.RemoveAllTilesAsync();
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
            App.NavigateTo(typeof(SettingsAdvancedView));
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