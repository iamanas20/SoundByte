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

using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using SoundByte.UWP.Services;
using WinRTXamlToolkit.Tools;
using System.Linq;
using SoundByte.Core.Items.Podcast;
using WelcomeView = SoundByte.UWP.Views.ImportViews.WelcomeView;

namespace SoundByte.UWP.Views.Application
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

            await new MessageDialog(dialogList, "Registered Dialogs").ShowAsync();
        }

        private async void GetItunes(object sender, RoutedEventArgs e)
        {
            var items = await PodcastShow.SearchAsync("WAN Show");
            var wanShow = items.FirstOrDefault();

            var wanShowItems = await wanShow.GetEpisodesAsync();

            var itemsList = string.Empty;

            wanShowItems.ForEach(x => itemsList += "- " + x.Title + "\n");

            await new MessageDialog(itemsList, "WAN Show Episodes").ShowAsync();
        }

        private async void GetWebClient(object sender, RoutedEventArgs e)
        {
            using (var client = new Windows.Web.Http.HttpClient())
            {
                var content = await client.GetAsync(new Uri("https://raw.githubusercontent.com/DominicMaas/SoundByte/master/README.md"));
                content.EnsureSuccessStatusCode();

                var result = await content.Content.ReadAsStringAsync();
                await new MessageDialog(result).ShowAsync();
            }
        }

        private async void GetNetClient(object sender, RoutedEventArgs e)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var content = await client.GetAsync(new Uri("https://raw.githubusercontent.com/DominicMaas/SoundByte/master/README.md"));
                content.EnsureSuccessStatusCode();

                var result = await content.Content.ReadAsStringAsync();
                await new MessageDialog(result).ShowAsync();
            }
        }

        private void NavigateSetupLogic(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(WelcomeView));
        }

        private void NavigateXboxView(object sender, RoutedEventArgs e)
        {

        }
    }
}
