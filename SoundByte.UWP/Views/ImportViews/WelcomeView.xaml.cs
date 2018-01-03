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

namespace SoundByte.UWP.Views.ImportViews
{
    public sealed partial class WelcomeView
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void NavigateAppModeView(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.NavigateTo(typeof(ImportModeView));
        }
    }
}
