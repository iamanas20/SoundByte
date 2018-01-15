﻿using Windows.UI.Xaml;
using SoundByte.UWP.Services;
using WinRTXamlToolkit.Tools;

namespace SoundByte.UWP.Views.Settings
{
    /// <summary>
    /// Blank page used for debugging.
    /// </summary>
    public sealed partial class SettingsAdvancedView
    {
        public SettingsAdvancedView()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await NavigationService.Current.CallDialogAsync(Command.Text);
        }

        private async void GetDialogList(object sender, RoutedEventArgs e)
        {
            var dialogs = NavigationService.Current.GetRegisteredDialogs();

            var dialogList = string.Empty;

            dialogs.ForEach(x => dialogList += "- " + x.Key + "\n");

            await NavigationService.Current.CallMessageDialogAsync(dialogList, "Registered Dialogs");
        }
    }
}
