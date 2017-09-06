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
using System.Globalization;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     This class handles all the settings within the app
    /// </summary>
    public class SettingsService
    {
        #region Static Class Setup

        private static readonly Lazy<SettingsService> InstanceHolder =
            new Lazy<SettingsService>(() => new SettingsService());

        public static SettingsService Instance => InstanceHolder.Value;
        #endregion

        #region Constant Keys

        private const string SettingsSyncKey = "SoundByte_SettingsSyncEnabled";
        private const string ThemeTypeKey = "SoundByte_ThemeType";
        private const string CurrentTrackKey = "SoundByte_TrackID";
        private const string LastViewedTrackKey = "SoundByte_LastViewedTack";
        private const string AppStoredVersionKey = "SoundByte_AppStoredVersionKey";
        private const string NotificationPopupKey = "SoundByte_NotificationPopupEnabled";
        private const string NotificationKey = "SoundByte_NotificationsEnabled";
        private const string ArtworkQualityKey = "SoundByte_ArtworkQualityColor";
        private const string LanguageKey = "SoundByte_DefaultLanguage";

        private const string DebugKey = "SoundByte.DebugModeEnabled";
        private const string TileStyleKey = "SoundByte.Tile.Style";
        private const string MenuCollapsedKey = "SoundByte.Desktop.MenuPosition";
        private const string AppInBackgroundKey = "SoundByte.Core.AppBackground";
        private const string LastVolumeSliderKey = "SoundByte.Playback.Volume";

        #endregion

        #region Getter and Setters

        /// <summary>
        ///     Is the app currently using the default system theme
        /// </summary>
        public bool IsDefaultTheme
        {
            get
            {
                switch (ApplicationThemeType)
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
        ///     The apps currently picked theme color
        /// </summary>
        public ApplicationTheme ThemeType
        {
            get
            {
                switch (ApplicationThemeType)
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
        ///     How many items at once are we allowed to load
        ///     (less for mobile, more for PC)
        /// </summary>
        public static int TrackLimitor => ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar") ? 40 : 60;

        /// <summary>
        ///     Should the app use high quality images
        /// </summary>
        public bool IsHighQualityArtwork
        {
            get
            {
                var boolVal = ReadSettingsValue(ArtworkQualityKey) as bool?;

                return !boolVal.HasValue || boolVal.Value;
            }
            set => SaveSettingsValue(ArtworkQualityKey, value, true);
        }

        /// <summary>
        /// Should the app display debug notifications
        /// </summary>
        public bool IsDebugModeEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(DebugKey) as bool?;
                return boolVal.HasValue && boolVal.Value;
            }
            set => SaveSettingsValue(DebugKey, value);
        }

        public bool IsAppBackground
        {
            get
            {
                var boolVal = ReadSettingsValue(AppInBackgroundKey) as bool?;
                return boolVal.HasValue && boolVal.Value;
            }
            set => SaveSettingsValue(AppInBackgroundKey, value);
        }

        /// <summary>
        /// Hamburger menu position
        /// </summary>
        public bool IsMenuOpen
        {
            get
            {
                var boolVal = ReadSettingsValue(MenuCollapsedKey) as bool?;

                return !boolVal.HasValue || boolVal.Value;
            }

            set => SaveSettingsValue(MenuCollapsedKey, value);
        }

        /// <summary>
        /// Should the blur background be shown on the tile
        /// </summary>
        public bool TileBackgroundStyleEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(TileStyleKey) as bool?;

                return !boolVal.HasValue || boolVal.Value;
            }
            set => SaveSettingsValue(TileStyleKey, value, true);
        }

        /// <summary>
        ///     Are notifications enabled for the app
        /// </summary>
        public bool IsNotificationsEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(NotificationKey) as bool?;

                return !boolVal.HasValue || boolVal.Value;
            }
            set => SaveSettingsValue(NotificationKey, value, true);
        }

        /// <summary>
        ///     Should the app popup notifications to the user
        /// </summary>
        public bool IsNotificationPopupEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(NotificationPopupKey) as bool?;

                return boolVal.HasValue && boolVal.Value;
            }
            set => SaveSettingsValue(NotificationPopupKey, value);
        }

        /// <summary>
        ///     The last stored app version
        /// </summary>
        public string AppStoredVersion
        {
            get => ReadSettingsValue(AppStoredVersionKey, true) as string;
            set => SaveSettingsValue(AppStoredVersionKey, value);
        }

        /// <summary>
        ///     The user saved language for the app
        /// </summary>
        public string CurrentAppLanguage
        {
            get => ReadSettingsValue(LanguageKey, true) as string;
            set => SaveSettingsValue(LanguageKey, value);
        }

        /// <summary>
        ///     The latest viewed track in the user stream
        /// </summary>
        public DateTime LatestViewedTrack
        {
            get
            {
                var stringVal = ReadSettingsValue(LastViewedTrackKey, true) as string;

                if (string.IsNullOrEmpty(stringVal))
                    return DateTime.UtcNow;

                try
                {
                    return DateTime.Parse(stringVal);
                }
                catch (FormatException)
                {
                    return DateTime.UtcNow;
                }
            }
            set => SaveSettingsValue(LastViewedTrackKey, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Gets the application theme type
        /// </summary>
        public AppTheme ApplicationThemeType
        {
            get
            {
                var stringVal = ReadSettingsValue(ThemeTypeKey) as string;

                if (string.IsNullOrEmpty(stringVal))
                    return AppTheme.Default;

                try
                {
                    var enumVal = (AppTheme) Enum.Parse(typeof(AppTheme), stringVal);
                    return enumVal;
                }
                catch
                {
                    return AppTheme.Default;
                }
            }
            set => SaveSettingsValue(ThemeTypeKey, value.ToString(), true);
        }

        /// <summary>
        ///     The currently playing track in the background task
        /// </summary>
        public int? CurrentPlayingTrack
        {
            get => ReadSettingsValue(CurrentTrackKey, true) as int?;
            set
            {
                if (value == -1)
                {
                    SaveSettingsValue(CurrentTrackKey, null);
                    return;
                }
                SaveSettingsValue(CurrentTrackKey, value);
            }
        }

        public double PlaybackVolume
        {
            get
            {
                var value = ReadSettingsValue(LastVolumeSliderKey, true) as double?;

                return value ?? 1.0;
            }

            set => SaveSettingsValue(LastVolumeSliderKey, value);
        }


        /// <summary>
        ///     Gets if settings syncing is enabled or not
        /// </summary>
        public bool IsSyncSettingsEnabled
        {
            get => ReadBoolSetting(ReadSettingsValue(SettingsSyncKey, true) as bool?, true);
            set => SaveSettingsValue(SettingsSyncKey, value);
        }

        #endregion

        #region Settings Helpers

        /// <summary>
        ///     Used to Return bool values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static bool ReadBoolSetting(bool? value, bool defaultValue)
        {
            return value ?? defaultValue;
        }

        /// <summary>
        ///     Reads a settings value. This method will check the roaming data to see if anything is saved first
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <param name="forceLocal"></param>
        /// <returns>Saved object</returns>
        private object ReadSettingsValue(string key, bool forceLocal = false)
        {
            // Check if the force local flag is enabled
            if (forceLocal)
                return GetLocalValue(key);

            // Check if sync is enabled
            if (!IsSyncSettingsEnabled) return GetLocalValue(key);
            // Get remote value
            var remoteValue = GetRemoteValue(key);
            // Return the remote value if it exists
            return remoteValue ?? GetLocalValue(key);
        }


        /// <summary>
        ///     Gets a remote value
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Object or null</returns>
        private static object GetRemoteValue(string key)
        {
            return ApplicationData.Current.RoamingSettings.Values.ContainsKey(key)
                ? ApplicationData.Current.RoamingSettings.Values[key]
                : null;
        }

        /// <summary>
        ///     Gets a local value
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Object or null</returns>
        private static object GetLocalValue(string key)
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(key)
                ? ApplicationData.Current.LocalSettings.Values[key]
                : null;
        }

        /// <summary>
        ///     Save a key value pair in settings. Create if it doesn't exist
        /// </summary>
        /// <param name="key">Used to find the value at a later state</param>
        /// <param name="value">what to save</param>
        /// <param name="canSync">should this value save online? (If user has enabled syncing)</param>
        private void SaveSettingsValue(string key, object value, bool canSync = false)
        {
            // Check if this value supports remote syncing
            if (canSync)
                if (IsSyncSettingsEnabled)
                    if (!ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
                    {
                        // Create new value
                        ApplicationData.Current.RoamingSettings.Values.Add(key, value);
                        return;
                    }
                    else
                    {
                        // Edit existing value
                        ApplicationData.Current.RoamingSettings.Values[key] = value;
                        return;
                    }

            // Store the value locally
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            else
                ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        #endregion
    }

    /// <summary>
    ///     The possible states for the app theme
    /// </summary>
    public enum AppTheme
    {
        Default,
        Light,
        Dark
    }
}