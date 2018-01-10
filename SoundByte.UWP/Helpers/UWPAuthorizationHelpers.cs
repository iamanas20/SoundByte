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
using System.Threading.Tasks;
using Windows.UI.Popups;
using SoundByte.Core.Helpers;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Helpers
{

    // ReSharper disable once InconsistentNaming
    public static class UWPAuthorizationHelpers
    {
        public static async Task<bool> OnlineAppInitAsync(bool updateKeys)
        {
            try
            {
                // Call the init method now and request new app keys
                var returnInfo = await AuthorizationHelpers.OnlineAppInitAsync("windows", "10.0.0.", SettingsService.Instance.AppId, updateKeys);

                if (!returnInfo.Successful)
                {
                    await new MessageDialog("SoundByte cannot load. The following error was returned from the SoundByte server: " + returnInfo.ErrorTitle + "\n\nPlease restart the app and try again. If this error continues, contact us on Twitter @SoundByteUWP or Facebook fb.com/SoundByteUWP.", "Critical Error").ShowAsync();
                    // Don't run anything, app will not work.
                    return false;
                }

                if (updateKeys)
                {
                    if (returnInfo.AppKeys == null)
                    {
                        await new MessageDialog("SoundByte cannot load. The following error was returned from the SoundByte server: App Keys not provided.\n\nPlease restart the app and try again. If this error continues, contact us on Twitter @SoundByteUWP or Facebook fb.com/SoundByteUWP.", "Critical Error").ShowAsync();
                        // Don't run anything, app will not work.
                        return false;
                    }

                    // We have keys! Time to update the app
                    var appKeys = returnInfo.AppKeys;

                    AppKeysHelper.SoundCloudClientId = appKeys.SoundCloudClientId;
                    AppKeysHelper.SoundCloudPlaybackIds = appKeys.SoundCloudPlaybackIds;
                    AppKeysHelper.YouTubeLoginClientId = appKeys.YouTubeLoginClientId;
                    AppKeysHelper.YouTubeClientId = appKeys.YouTubeClientId;
                    AppKeysHelper.FanburstClientId = appKeys.FanburstClientId;
                    AppKeysHelper.LastFmClientId = appKeys.LastFmClientId;
                    AppKeysHelper.GoogleAnalyticsTrackerId = appKeys.GoogleAnalyticsTrackerId;
                    AppKeysHelper.AppCenterClientId = appKeys.AppCenterClientId;
                    AppKeysHelper.HockeyAppClientId = appKeys.HockeyAppClientId;
                    AppKeysHelper.SoundByteClientId = appKeys.SoundByteClientId;
                }

                // Update the app ID (this should be the same as before / update to new value)
                SettingsService.Instance.AppId = returnInfo.AppId;

                return true;
            }
            catch (Exception e)
            {
                await new MessageDialog("SoundByte cannot load. The following error was returned from the SoundByte server: " + e.Message + "\n\nPlease restart the app and try again. If this error continues, contact us on Twitter @SoundByteUWP or Facebook fb.com/SoundByteUWP.", "Critical Error").ShowAsync();
                // Don't run anything, app will not work.
                return false;
            }
        }
    }
}
