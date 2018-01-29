﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Globalization;
using Windows.Services.Store;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;
using SoundByte.UWP.Views;
using SoundByte.UWP.Views.Me;
using SoundByte.UWP.Views.Settings;
using SoundByte.UWP.Views.SoundCloud;

namespace SoundByte.UWP
{
    public sealed partial class AppShell
    {
        public AppShell(string path)
        {
            // Init the XAML
            LoggingService.Log(LoggingService.LogType.Debug, "Loading Shell XAML");
            InitializeComponent();

            // Set the accent color
            TitlebarHelper.UpdateTitlebarStyle();

            LoggingService.Log(LoggingService.LogType.Debug, "Attaching Event Handlers");

            // When the page is loaded (after the following and xaml init)
            // we can perform the async work
            Loaded += async (sender, args) => await PerformAsyncWork(path);

            // Unload events
            Unloaded += (sender, args) => Dispose();

            var titleBar = CoreApplication.GetCurrentView().TitleBar;
            titleBar.LayoutMetricsChanged += (s, e) =>
            {
                AppTitle.Margin = new Thickness(CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset + 12, 8, 0, 0);
            };

            // This is a dirty to show the now playing
            // bar when a track is played. This method
            // updates the required layout for the now
            // playing bar.
            PlaybackService.Instance.OnTrackChange += InstanceOnOnCurrentTrackChanged;


            // Create a shell frame shadow for mobile and desktop
            if (DeviceHelper.IsDesktop)
            {
                ShellFrame.CreateElementShadow(new Vector3(4, 0, 0), 40, new Color { A = 82, R = 0, G = 0, B = 0 },
                    ShellFrameShadow);
            }

            // Events for Xbox
            if (DeviceHelper.IsXbox)
            {
                // Make xbox selection easy to see
                Application.Current.Resources["CircleButtonStyle"] =
                    Application.Current.Resources["XboxCircleButtonStyle"];
            }

            RootFrame.Focus(FocusState.Keyboard);
        }

        private async void InstanceOnOnCurrentTrackChanged(BaseTrack newTrack)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                if (!DeviceHelper.IsDesktop ||
                    RootFrame.CurrentSourcePageType == typeof(NowPlayingView))
                    HideNowPlayingBar();
                else
                    ShowNowPlayingBar();
            });
        }


        public void Dispose()
        {
            PlaybackService.Instance.OnTrackChange -= InstanceOnOnCurrentTrackChanged;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (App.OverrideBackEvent)
            {
                e.Handled = true;
            }
            else
            {
                if (RootFrame.SourcePageType == typeof(BlankView))
                {
                    RootFrame.BackStack.Clear();
                    RootFrame.Navigate(typeof(ExploreView));
                    e.Handled = true;
                }
                else
                {
                    if (RootFrame.CanGoBack)
                    {
                        RootFrame.GoBack();
                        e.Handled = true;
                    }
                    else
                    {
                        RootFrame.Navigate(typeof(ExploreView));
                        e.Handled = true;
                    }
                }
            }
        }

        private async Task PerformAsyncWork(string path)
        {
            LoggingService.Log(LoggingService.LogType.Debug, "Page loaded, performing async work");

            // Set the app language
            ApplicationLanguages.PrimaryLanguageOverride =
                string.IsNullOrEmpty(SettingsService.Instance.CurrentAppLanguage)
                    ? ApplicationLanguages.Languages[0]
                    : SettingsService.Instance.CurrentAppLanguage;

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            // Navigate to the first page
            await HandleProtocolAsync(path);

            // Run on the background thread
            await Task.Run(async () =>
            {
                // We have not yet run the online init
                if (!App.OnlineAppInitComplete)
                {
                    // Update the keys anyway to ensure we have the latest.
                    await UWPAuthorizationHelpers.OnlineAppInitAsync(true);
                }

                // Test Version and tell user app upgraded
                await HandleNewAppVersionAsync();

                // Clear the unread badge
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();

                // Get the store and check for app updates
                var updates = await StoreContext.GetDefault().GetAppAndOptionalStorePackageUpdatesAsync();

                // If we have updates navigate to the update page where we
                // ask the user if they would like to update or not (depending
                // if the update is mandatory or not).
                if (updates.Count > 0)
                {
                    await NavigationService.Current.CallDialogAsync<PendingUpdateDialog>();
                }

                // Load logged in user objects
                await SoundByteService.Current.InitUsersAsync();

                App.Telemetry.TrackEvent("Connected Accounts", new Dictionary<string, string>
                {
                    { "IsSoundByteConnected", SoundByteService.Current.IsServiceConnected(ServiceType.SoundByte).ToString() },
                    { "IsSoundCloudConnected", SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud).ToString() },
                    { "IsFanburstConnected", SoundByteService.Current.IsServiceConnected(ServiceType.Fanburst).ToString() },
                    { "IsYouTubeConnected", SoundByteService.Current.IsServiceConnected(ServiceType.YouTube).ToString() }
                });

                // Register notifications
                //  var engagementManager = StoreServicesEngagementManager.GetDefault();
                //   await engagementManager.RegisterNotificationChannelAsync();
                // TODO: NUGET FILE TOO LONG


                try
                {
                    // Install Cortana Voice Commands
                    var vcdStorageFile = await Package.Current.InstalledLocation.GetFileAsync(@"SoundByteCommands.xml");
                    await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
                }
                catch
                {
                    // Ignore
                }
            });
        }

        private async Task HandleNewAppVersionAsync()
        {
            var currentAppVersionString = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor +
                                          "." + Package.Current.Id.Version.Build;

            // Get stored app version (this will stay the same when app is updated)
            var storedAppVersionString = SettingsService.Instance.AppStoredVersion;

            // Save the new app version
            SettingsService.Instance.AppStoredVersion = currentAppVersionString;

            // If the stored version is null, set the temp to 0, and the version to the actual version
            if (!string.IsNullOrEmpty(storedAppVersionString))
            {
                // Convert the current app version
                var currentAppVersion = new Version(currentAppVersionString);
                // Convert the stored app version
                var storedAppVersion = new Version(storedAppVersionString);

                if (currentAppVersion <= storedAppVersion)
                    return;
            }

            // First run
            if (string.IsNullOrEmpty(storedAppVersionString))
            {
                //todo: show welcome dialog
                //  await NavigationService.Current.CallDialogAsync<WhatsNewDialog>();
            }
            else
            {
                await NavigationService.Current.CallDialogAsync<WhatsNewDialog>();
            }
        }

        #region Protocol
        public async Task HandleProtocolAsync(string path)
        {
            LoggingService.Log(LoggingService.LogType.Debug, "Performing protocol work using path of " + path);

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    if (path == "playUserLikes" || path == "shufflePlayUserLikes")
                    {
                        if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud))
                        {
                            // Navigate to the now playing screen
                            RootFrame.Navigate(typeof(NowPlayingView));

                            // Get and load the user liked items
                            var userLikes = new SoundByteCollection<SoundCloudLikeSource, BaseTrack>();
                            userLikes.Source.User = SoundByteService.Current.GetConnectedUser(ServiceType.SoundCloud);

                            // Loop through loading all the likes
                            while (userLikes.HasMoreItems)
                                await userLikes.LoadMoreItemsAsync(50);

                            // Play the list of items
                            await BaseViewModel.PlayAllTracksAsync(userLikes, null, path == "shufflePlayUserLikes");

                            return;
                        }
                    }

                    if (path == "playUserStream")
                    {
                        if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud))
                        {
                            // Navigate to the now playing screen
                            RootFrame.Navigate(typeof(NowPlayingView));

                            // Get and load the user liked items
                            var userStream = new SoundByteCollection<SoundCloudStreamSource, GroupedItem>();

                            // Counter so we don't get an insane amount of items
                            var i = 0;

                            // Grab all the users stream / 5 items
                            while (userStream.HasMoreItems && i <= 5)
                            {
                                i++;
                                await userStream.LoadMoreItemsAsync(50);
                            }

                            // Play the list of items
                            var result = await PlaybackService.Instance.InitilizePlaylistAsync<DummyTrackSource>(
                                userStream.Where(x => x.Track != null).Select(x => x.Track).ToList());

                            if (result.Success)
                            {
                                await PlaybackService.Instance.StartTrackAsync();
                            }

                            return;
                        }
                    }

                    var parser = DeepLinkParser.Create(path);

                    var section = parser.Root?.Split('/')[0]?.ToLower();

                    await App.SetLoadingAsync(true);
                    if (section == "core")
                    {
                        var page = parser.Root?.Split('/')[1]?.ToLower();

                        switch (page)
                        {
                            case "track":

                                BaseTrack track = null;

                                switch (parser["service"])
                                {
                                    case "soundcloud":
                                        track = (await SoundByteService.Current.GetAsync<SoundCloudTrack>(ServiceType.SoundCloud, $"/tracks/{parser["id"]}")).Response.ToBaseTrack();
                                        break;
                                    case "youtube":
                                        break;
                                    case "fanburst":
                                        track = (await SoundByteService.Current.GetAsync<FanburstTrack>(ServiceType.Fanburst, $"/videos/{parser["id"]}")).Response.ToBaseTrack();
                                        break;
                                }

                                if (track != null)
                                {
                                    var startPlayback =
                                        await PlaybackService.Instance.InitilizePlaylistAsync<DummyTrackSource>(new List<BaseTrack> { track });

                                    if (startPlayback.Success)
                                    {
                                        await PlaybackService.Instance.StartTrackAsync();
                                    }
                                    else
                                    {
                                        await NavigationService.Current.CallMessageDialogAsync(startPlayback.Message,
                                            "Error playing track.");
                                    }
                                }
                                break;
                            case "playlist":
                                var playlist =
                                    await SoundByteService.Current.GetAsync<SoundCloudPlaylist>(ServiceType.SoundCloud, $"/playlists/{parser["id"]}");
                                App.NavigateTo(typeof(PlaylistView), playlist.Response.ToBasePlaylist());
                                return;
                            case "user":
                                var user = await SoundByteService.Current.GetAsync<SoundCloudUser>(ServiceType.SoundCloud, $"/users/{parser["id"]}");
                                App.NavigateTo(typeof(UserView), user.Response.ToBaseUser());
                                return;
                            case "changelog":
                                await NavigationService.Current.CallDialogAsync<WhatsNewDialog>();
                                break;
                        }
                    }
                    else if (section == "rs" || section == "remote-subsystem")
                    {
                        try
                        {
                            await App.SetLoadingAsync(true);

                            parser.TryGetValue("d", out var data);
                            parser.TryGetValue("t", out var timespan);

                            var result = App.RoamingService.DecodeActivityParameters(data);

                            // Get the current track object
                            BaseTrack currentTrack = null;
                            var tracks = new List<BaseTrack>();

                            switch (result.CurrentTrack.Service)
                            {
                                case ServiceType.Fanburst:
                                    break;
                                case ServiceType.SoundCloud:
                                case ServiceType.SoundCloudV2:
                                    currentTrack = (await SoundByteService.Current.GetAsync<SoundCloudTrack>(ServiceType.SoundCloud, $"/tracks/{result.CurrentTrack.TrackId}")).Response.ToBaseTrack();
                                    break;
                                case ServiceType.YouTube:
                                    currentTrack = (await SoundByteService.Current.GetAsync<YouTubeVideoHolder>(ServiceType.YouTube, "videos", new Dictionary<string, string>
                                    {
                                        {"part", "snippet,contentDetails"},
                                        { "id", result.CurrentTrack.TrackId }
                                    })).Response.Tracks.FirstOrDefault()?.ToBaseTrack();
                                    break;
                                case ServiceType.ITunesPodcast:
                                    // TODO: THIS
                                    break;
                            }

                            //TODO: List has to be put back into wanted order.

                            var soundCloudIds = string.Join(',', result.Tracks.Where(x => x.Service == ServiceType.SoundCloud || x.Service == ServiceType.SoundCloudV2).Select(x => x.TrackId));
                            var fanburstIds = string.Join(',', result.Tracks.Where(x => x.Service == ServiceType.Fanburst).Select(x => x.TrackId));
                            var youTubeIds = string.Join(',', result.Tracks.Where(x => x.Service == ServiceType.YouTube).Select(x => x.TrackId));

                            // SoundCloud tracks
                            tracks.AddRange((await SoundByteService.Current.GetAsync<List<SoundCloudTrack>>(ServiceType.SoundCloud, $"/tracks?ids={soundCloudIds}")).Response.Select(x => x.ToBaseTrack()));

                            // YouTube Tracks
                            tracks.AddRange((await SoundByteService.Current.GetAsync<YouTubeVideoHolder>(ServiceType.YouTube, "videos", new Dictionary<string, string>
                            {
                                { "part", "snippet,contentDetails"},
                                { "id", youTubeIds }

                            })).Response.Tracks.Select(x => x.ToBaseTrack()));

                            var startPlayback = await PlaybackService.Instance.InitilizePlaylistAsync(result.Source, tracks);

                            if (startPlayback.Success)
                            {
                                TimeSpan? timeSpan = null;

                                if (!string.IsNullOrEmpty(timespan))
                                    timeSpan = TimeSpan.FromMilliseconds(double.Parse(timespan));

                                await PlaybackService.Instance.StartTrackAsync(currentTrack, timeSpan);
                            }
                            else
                            {
                                await NavigationService.Current.CallMessageDialogAsync(startPlayback.Message, "The remote protocol subsystem failed.");
                            }

                            await App.SetLoadingAsync(false);
                        }
                        catch (Exception e)
                        {
                            await App.SetLoadingAsync(false);
                            await NavigationService.Current.CallMessageDialogAsync(e.Message, "The remote protocol subsystem failed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await NavigationService.Current.CallMessageDialogAsync(
                        "The specified protocol is not correct. App will now launch as normal.\n\n" + ex.Message);
                }
                await App.SetLoadingAsync(false);
            }

            if (DeviceHelper.IsMobile)
            {
                RootFrame.Navigate(typeof(MobileView));
            }
            else
            {
                RootFrame.Navigate(typeof(ExploreView));
            }
        }
        #endregion

        private void ShellFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Update the side bar
            switch (((Frame)sender).SourcePageType.Name)
            {
                case nameof(SoundCloudStreamView):
                    NavView.SelectedItem = NavigationItemSoundCloudStream;
                    break;
                case nameof(ExploreView):
                    NavView.SelectedItem = NavigationItemExplore;
                    break;
                case nameof(DonateView):
                    NavView.SelectedItem = NavigationItemDonations;
                    break;
                case nameof(HistoryView):
                    NavView.SelectedItem = NavigationItemHistory;
                    break;
                case nameof(AccountManagerView):
                    NavView.SelectedItem = NavigationItemAccounts;
                    break;
                case nameof(SettingsView):
                    NavView.SelectedItem = NavigationItemSettings;
                    break;
                case nameof(CollectionView):
                    NavView.SelectedItem = NavigationItemCollection;
                    break;
                case nameof(DeviceView):
                    NavView.SelectedItem = NavigationItemDownloads;
                    break;
             //   default:
             //       NavView.SelectedItem = NavigationItemOther;
              //      break;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((Frame)sender).CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;

            // Update the UI depending if we are logged in or not
            if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud) ||
                SoundByteService.Current.IsServiceConnected(ServiceType.YouTube) ||
                SoundByteService.Current.IsServiceConnected(ServiceType.Fanburst))
                ShowLoginContent();
            else
                ShowLogoutContent();

            if (DeviceHelper.IsDesktop)
            {
                if (((Frame)sender).SourcePageType == typeof(NowPlayingView))
                {
                    NavView.IsPaneToggleButtonVisible = false;
                    NavView.CompactPaneLength = 0;
                    NavView.OpenPaneLength = 0;

                    HideNowPlayingBar();
                }
                else
                {
                    NavView.IsPaneToggleButtonVisible = true;
                    NavView.CompactPaneLength = 64;
                    NavView.OpenPaneLength = 320;


                    if (PlaybackService.Instance.GetCurrentTrack() == null)
                        HideNowPlayingBar();
                    else
                        ShowNowPlayingBar();
                }
            }

            RootFrame.Focus(FocusState.Programmatic);
            RootFrame.Focus(FocusState.Keyboard);
        }

        private void HideNowPlayingBar()
        {
            NowPlaying.Visibility = Visibility.Collapsed;
            NavView.Margin = new Thickness { Bottom = 0 };
        }

        private void ShowNowPlayingBar()
        {
            NowPlaying.Visibility = Visibility.Visible;
            NavView.Margin = new Thickness { Bottom = 80 };
        }

        // Login and Logout events. This is used to display what pages
        // are visiable to the user.
        public void ShowLoginContent()
        {
            NavigationItemCollection.Visibility = Visibility.Visible;
            // Only show this tab if the users soundcloud account is connected
            NavigationItemSoundCloudStream.Visibility = SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud) ? Visibility.Visible : Visibility.Collapsed;
        }

        public void ShowLogoutContent()
        {
            NavigationItemCollection.Visibility = Visibility.Collapsed;
            // Only show this tab if the users soundcloud account is connected
            NavigationItemSoundCloudStream.Visibility = SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud) ? Visibility.Visible : Visibility.Collapsed;
        }


        #region Getters and Setters
        /// <summary>
        ///     Get the root frame, if no root frame exists,
        ///     we wait 150ms and call the getter again.
        /// </summary>
        public Frame RootFrame
        {
            get
            {
                if (ShellFrame != null) return ShellFrame;

                Task.Delay(TimeSpan.FromMilliseconds(150));

                return RootFrame;
            }
            set => ShellFrame = value;
        }

        #endregion

        private void NavView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            PerformNavigation(args.InvokedItem);
        }

        private void NavView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = args.SelectedItem as NavigationViewItem;
            PerformNavigation(item?.Tag);
        }

        private void PerformNavigation(object item)
        {
            switch (item)
            {
                case "scstream":
                    RootFrame.Navigate(typeof(SoundCloudStreamView));
                    break;
                case "explore":
                    RootFrame.Navigate(typeof(ExploreView));
                    break;
                case "history":
                    RootFrame.Navigate(typeof(HistoryView));
                    break;
                case "donations":
                    RootFrame.Navigate(typeof(DonateView));
                    break;
                case "settings":
                    RootFrame.Navigate(typeof(SettingsView));
                    break;
                case "accounts":
                    RootFrame.Navigate(typeof(AccountManagerView));
                    break;
                case "mycollection":
                    RootFrame.Navigate(typeof(CollectionView));
                    break;
                case "mydevice":
                    RootFrame.Navigate(typeof(DeviceView));
                    break;
                case "now-playing":
                    App.NavigateTo(typeof(NowPlayingView));
                    break;
                case "addons":
                    break;

            }
        }

        private void SearchForItem(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            App.NavigateTo(typeof(SearchView), args.QueryText);
        }


    }
}