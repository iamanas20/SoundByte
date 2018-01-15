using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Android.App;
using Android.Content;
using Newtonsoft.Json;

namespace SoundByte.Android.Helpers
{
    /// <summary>
    ///     Methods for app keys
    /// </summary>
    public static class AppKeysHelper
    {
        /// <summary>
        /// Client ID used to access SoundCloud services
        /// </summary>
        public static string SoundCloudClientId
        {
            get => Load();
            set => Save(value);
        }

        /// <summary>
        /// A list of IDs used for SoundCloud Playback
        /// </summary>
        public static List<string> SoundCloudPlaybackIds
        {
            get => JsonConvert.DeserializeObject<List<string>>(Load());

            set => Save(JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// Client ID used to access Fanburst services
        /// </summary>
        public static string FanburstClientId
        {
            get => Load();
            set => Save(value);
        }

        /// <summary>
        /// Client ID used to access YouTube Resources.
        /// </summary>
        public static string YouTubeClientId
        {
            get => Load();
            set => Save(value);
        }

        /// <summary>
        /// Client ID used to login to a users YouTube account.
        /// </summary>
        public static string YouTubeLoginClientId
        {
            get => Load();
            set => Save(value);
        }

        /// <summary>
        /// Used for the last FM scrobbler
        /// </summary>
        public static string LastFmClientId
        {
            get => Load();
            set => Save(value);
        }

        /// <summary>
        /// Used for crash reporting
        /// </summary>
        public static string HockeyAppClientId
        {
            get => Load();
            set => Save(value);
        }

        /// <summary>
        /// Used for Google Analytics
        /// </summary>
        public static string GoogleAnalyticsTrackerId
        {
            get => Load();
            set => Save(value);
        }

        /// <summary>
        /// Client ID used for MS app center
        /// </summary>
        public static string AppCenterClientId
        {
            get => Load();
            set => Save(value);
        }

        public static string SoundByteClientId
        {
            get => Load();
            set => Save(value);
        }

        /// <summary>
        /// This method checks to see if all app keys are valid. If an app key is missing
        /// (such as new install, re-install, app update) this method will return false,
        /// letting the app know that we need fresh/new api keys.
        /// </summary>
        public static bool KeysValid => !(string.IsNullOrEmpty(SoundCloudClientId) ||
                                          string.IsNullOrEmpty(FanburstClientId) ||
                                          string.IsNullOrEmpty(YouTubeClientId) ||
                                          string.IsNullOrEmpty(YouTubeLoginClientId) ||
                                          string.IsNullOrEmpty(LastFmClientId) ||
                                          string.IsNullOrEmpty(HockeyAppClientId) ||
                                          string.IsNullOrEmpty(GoogleAnalyticsTrackerId) ||
                                          string.IsNullOrEmpty(AppCenterClientId) ||
                                          string.IsNullOrEmpty(SoundByteClientId));

        private static void Save(string value, [CallerMemberName]string name = "")
        {
            // Open the settings file to edit
            var settings = Application.Context.GetSharedPreferences("SoundByte.Android.Keys", FileCreationMode.Private);
            var settingsEditor = settings.Edit();

            // Save the setting
            settingsEditor.PutString("SBKEY_" + name.ToUpper(), value);
            settingsEditor.Commit();
        }

        private static string Load([CallerMemberName]string name = "")
        {
            var settings = Application.Context.GetSharedPreferences("SoundByte.Android.Keys", FileCreationMode.Private);
            return settings.GetString("SBKEY_" + name.ToUpper(), null);
        }
    }
}