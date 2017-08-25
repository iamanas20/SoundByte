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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

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

        public async Task CallDialogAsync<T>(object[] param = null)
        {
            var dialogType = _registeredDialogs.FirstOrDefault(x => x.Value == typeof(T)).Value;

            if (dialogType != null)
            {
                var instance = Activator.CreateInstance(dialogType, param);
                await ((ContentDialog)instance).ShowAsync();
            }
        }

        public async Task CallDialogAsync(string name, object[] param = null)
        {
            var dialogType = _registeredDialogs.FirstOrDefault(x => x.Key == name).Value;

            if (dialogType != null)
            {
                var instance = Activator.CreateInstance(dialogType, param);
                await ((ContentDialog) instance).ShowAsync();
            }
        }
    }
}
