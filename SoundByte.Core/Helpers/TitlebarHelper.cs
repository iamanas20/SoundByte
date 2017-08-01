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
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    ///     Helper methods for adjusting app titlebar / statusbar style
    /// </summary>
    public static class TitlebarHelper
    {
        /// <summary>
        ///     Refreshes the stored app accent color
        /// </summary>
        public static async void UpdateTitlebarStyle()
        {
            var textColor = Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.White : Colors.Black;

            if (DeviceHelper.IsDesktop)
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                // Update Title bar colors
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor =
                    new Color {R = 0, G = 0, B = 0, A = 20};
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor =
                    new Color {R = 0, G = 0, B = 0, A = 60};
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = textColor;
            }

            if (DeviceHelper.IsMobile)
                await StatusBar.GetForCurrentView().HideAsync();
        }
    }
}