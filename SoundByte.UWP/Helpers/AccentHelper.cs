//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Helpers
{
    /// <summary>
    /// Helper methods for working with the apps accent color and
    /// selected theme
    /// </summary>
    public static class AccentHelper
    {
        /// <summary>
        /// Is the app currently using the default system theme
        /// </summary>
        public static bool IsDefaultTheme
        {
            get
            {
                switch (SettingsService.Current.ApplicationThemeType)
                {
                    case AppTheme.Default:
                        return true;
                    case AppTheme.Dark:
                        return false;
                    case AppTheme.Light:
                        return false;
                    default:
                        return true;
                }
            }
        }

        /// <summary>
        /// The apps currently picked theme color
        /// </summary>
        public static ApplicationTheme ThemeType
        {
            get
            {
                switch (SettingsService.Current.ApplicationThemeType)
                {
                    case AppTheme.Dark:
                        return ApplicationTheme.Dark;
                    case AppTheme.Light:
                        return ApplicationTheme.Light;
                    case AppTheme.Default:
                        return ApplicationTheme.Dark;
                    default:
                        return ApplicationTheme.Dark;
                }
            }
        }

        /// <summary>
        /// Refreshes the stored app accent color
        /// </summary>
        public static async void UpdateTitlebarStyle()
        {
            var textColor = Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.White : Colors.Black;

            if (App.IsDesktop)
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                // Update Title bar colors
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor = new Color { R = 0, G = 0, B = 0, A = 20 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor = new Color { R = 0, G = 0, B = 0, A = 60 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = textColor;
            }

            if (App.IsMobile)
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }
        }
    }
}