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
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using SoundByte.Core.API.Holders;
using SoundByte.Core.Converters;
using SoundByte.Core.Helpers;
using SoundByte.Core.Services;

namespace SoundByte.Notifications
{
    public sealed class NotificationTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Enable Async Code
            var deferral = taskInstance.GetDeferral();

            // If we have no internet, do not perform the task
            if (!NetworkHelper.HasInternet)
                return;

            // If the user has not connected their soundcloud 
            // account. Do nothing.
            if (!SoundByteService.Current.IsSoundCloudAccountConnected)
                return;

            // If the user has disabled notifications, do not
            // do anything.
            if (!SettingsService.Current.IsNotificationsEnabled)
                return;

            // Get the time of the last check, we do this
            // so we know what tracks to get (that the user 
            // has not already seen).
            var lastCheckTime = SettingsService.Current.LatestViewedTrack;

            try
            {
                // Call the SoundCloud api and get notifications for this user
                var items = await SoundByteService.Current.GetAsync<NotificationListHolder>("/e1/me/stream",
                    new Dictionary<string, string>
                    {
                        {"limit", "20"}
                    });

                foreach (var notification in items.Notifications)
                {
                    // Get the current date and time of the uploaded item
                    var postedTime = notification.CreatedAt.ToLocalTime();

                    // Check if the notification happened before the last 
                    // notification check
                    if (lastCheckTime <= postedTime)
                    {
                        // Create the variables needed for the notification
                        var title = string.Empty;
                        var content = string.Empty;
                        var logo = string.Empty;
                        var arguments = string.Empty;

                        switch (notification.Type)
                        {
                            case "track-repost":
                                // Clean and set the notification title
                                title = TextHelper.CleanXmlString(notification.Track.Title);
                                // Clean and set the notification content
                                content = TextHelper.CleanXmlString(string.Format("{0} has reposted a sound by {1}.",
                                    notification.User.Username, notification.Track.User.Username));
                                // Set the logo
                                logo = ArtworkConverter.ConvertObjectToImage(notification.Track);
                                // Set the arguments
                                arguments = $"soundbyte://core/track?id={notification.Track.Id}";
                                break;
                            case "track":
                                // Clean and set the notification title
                                title = TextHelper.CleanXmlString(notification.Track.Title);
                                // Clean and set the notification content
                                content = TextHelper.CleanXmlString(string.Format("{0} has uploaded a new sound.",
                                    notification.User.Username));
                                // Set the logo
                                logo = ArtworkConverter.ConvertObjectToImage(notification.Track);
                                // Set the arguments
                                arguments = $"soundbyte://core/track?id={notification.Track.Id}";
                                break;
                            case "playlist-repost":
                                // Clean and set the notification title
                                title = TextHelper.CleanXmlString(notification.Playlist.Title);
                                // Clean and set the notification content
                                content = TextHelper.CleanXmlString(string.Format("{0} has reposted a set by {1}.",
                                    notification.User.Username, notification.Playlist.User.Username));
                                // Set the logo
                                logo = ArtworkConverter.ConvertObjectToImage(notification.Playlist);
                                // Set the arguments
                                arguments = $"soundbyte://core/playlist?id={notification.Playlist.Id}";
                                break;
                            case "playlist":
                                // Clean and set the notification title
                                title = TextHelper.CleanXmlString(notification.Playlist.Title);
                                // Clean and set the notification content
                                content = TextHelper.CleanXmlString(string.Format("{0} has created a new set.",
                                    notification.User.Username));
                                // Set the logo
                                logo = ArtworkConverter.ConvertObjectToImage(notification.Playlist);
                                // Set the arguments
                                arguments = $"soundbyte://core/playlist?id={notification.Playlist.Id}";
                                break;
                        }

                        // Create the visual part of the taost notification
                        var toastVisual =
                            $@"<visual><binding template='ToastGeneric'><text>{title}</text><text>{
                                    content
                                }</text><image src='{logo}' placement='appLogoOverride'/></binding></visual>";

                        // Create the toast XML string
                        var toastXmlString =
                            $@" <toast launch='{arguments}' displayTimestamp='{postedTime:yyyy-MM-ddTHH:mm:ss.ffzzz}'>{
                                    toastVisual
                                }<actions></actions></toast>";

                        // Create the XML document
                        var toastXml = new XmlDocument();
                        toastXml.LoadXml(toastXmlString);

                        // Create the toast notification
                        var toast = new ToastNotification(toastXml) { SuppressPopup = !SettingsService.Current.IsNotificationPopupEnabled };

                        // Show the taost notification
                        ToastNotificationManager.CreateToastNotifier().Show(toast);
                    }   
                }
            }
            catch (Exception e)
            {
                TelemetryService.Current.TrackException(e);
            }

            // Store the latest time so the notifications do not repeat
            SettingsService.Current.LatestViewedTrack = DateTime.UtcNow;

            // Finished
            deferral.Complete();
        }
    }
}