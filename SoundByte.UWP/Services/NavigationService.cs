﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using WinRTXamlToolkit.Controls.Extensions;

namespace SoundByte.UWP.Services
{
    /// <summary>
    /// Todo: this is mainly used for debugging dialogs
    /// </summary>
    public class NavigationService
    {
        private static readonly Lazy<NavigationService> InstanceHolder =
            new Lazy<NavigationService>(() => new NavigationService());

        public static NavigationService Current => InstanceHolder.Value;

        private readonly Dictionary<string, Type> _registeredDialogs = new Dictionary<string, Type>();

        /// <summary>
        /// Registers a dialog with the navigation service. This allows the ability to call dialogs without
        /// strict naming. E.g using the call method: Dialog/CrashDialog will open the crash dialog.
        /// USED FOR DEBUGGING
        /// </summary>
        /// <param name="altName">An alternative name to call this dialog by</param>
        public void RegisterTypeAsDialog<T>(string altName = "")
        {
            if (string.IsNullOrEmpty(altName))
                altName = typeof(T).Name;

            _registeredDialogs.Add(altName, typeof(T));
        }

        public async Task CallMessageDialogAsync(string content, string title)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                await new MessageDialog(content, title).ShowQueuedAsync();
            });
        }

        public async Task CallMessageDialogAsync(string content)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                await new MessageDialog(content).ShowQueuedAsync();
            });
        }

        public async Task CallDialogAsync<T>(params object[] param)
        {
            var dialogType = _registeredDialogs.FirstOrDefault(x => x.Value == typeof(T)).Value;

            if (dialogType != null)
            {
                try
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                    {
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

                        var instance = Activator.CreateInstance(dialogType, param);
                        await ((ContentDialog)instance).ShowAsync();
                    });
                }
                catch (Exception)
                {
                    // Crashes if another dialog is open
                }
            }
        }

        public async Task CallDialogAsync(string name, object[] param = null)
        {
            var dialogType = _registeredDialogs.FirstOrDefault(x => x.Key == name).Value;

            if (dialogType != null)
            {
                try
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                    {
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

                        var instance = Activator.CreateInstance(dialogType, param);
                        await ((ContentDialog)instance).ShowAsync();
                    });
                }
                catch (Exception)
                {
                    // Crashes if another dialog is open
                }
            }
        }

        public Dictionary<string, Type> GetRegisteredDialogs()
        {
            return _registeredDialogs;
        }
    }
}
