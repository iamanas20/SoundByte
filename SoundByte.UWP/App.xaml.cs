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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using SoundByte.UWP.Views.Application;
using WinRTXamlToolkit.Tools;

namespace SoundByte.UWP
{
    sealed partial class App
    {
        public static bool OnlineAppInitComplete { get; set; }

        public static ITelemetryService Telemetry { get; } = new TelemetryService();

        private bool _isInit;

        #region App Setup

        /// <summary>
        ///     This is the main class for this app. This function is the first function
        ///     called and it setups the app analytic (If in release mode), components,
        ///     requested theme and event handlers.
        /// </summary>
        public App()
        {
            LoggingService.Log(LoggingService.LogType.Debug, "----- App Started -----");

            // Init XAML Resources
            InitializeComponent();

            LoggingService.Log(LoggingService.LogType.Debug, "Loaded XAML");

            // We want to use the controler if on xbox
            if (DeviceHelper.IsXbox)
                RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;

            // Check that we are not using the default theme,
            // if not change the requested theme to the users
            // picked theme.
            if (!SettingsService.Instance.IsDefaultTheme)
                RequestedTheme = SettingsService.Instance.ThemeType;

            // Register the dialogs
            NavigationService.Current.RegisterTypeAsDialog<CrashDialog>();
            NavigationService.Current.RegisterTypeAsDialog<PendingUpdateDialog>();
            NavigationService.Current.RegisterTypeAsDialog<PinTileDialog>();
            NavigationService.Current.RegisterTypeAsDialog<PlaylistDialog>();
            NavigationService.Current.RegisterTypeAsDialog<ShareDialog>();
            NavigationService.Current.RegisterTypeAsDialog<LoginDialog>();
            NavigationService.Current.RegisterTypeAsDialog<WhatsNewDialog>();

            // Handle App Crashes
            CrashHelper.HandleAppCrashes(Current);

            // Enter and Leaving background handlers
            EnteredBackground += AppEnteredBackground;
            LeavingBackground += AppLeavingBackground;

            // During the transition from foreground to background the
            // memory limit allowed for the application changes. The application
            // has a short time to respond by bringing its memory usage
            // under the new limit.
            MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;

            // After an application is backgrounded it is expected to stay
            // under a memory target to maintain priority to keep running.
            // Subscribe to the event that informs the app of this change.
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;

            // Run this code when a service is connected to SoundByte
            SoundByteService.Current.OnServiceConnected += async (type, token) =>
            {
                var vault = new PasswordVault();

                // Add the password to the vault so we can access it when restarting the app
                string vaultName;
                 switch (type)
                {
                    case ServiceType.SoundCloud:
                    case ServiceType.SoundCloudV2:
                        vaultName = "SoundByte.SoundCloud";
                        break;
                    case ServiceType.Fanburst:
                        vaultName = "SoundByte.FanBurst";
                        break;
                    case ServiceType.YouTube:
                        vaultName = "SoundByte.YouTube";
                        break;
                    case ServiceType.SoundByte:
                        vaultName = "SoundByte.SoundByte";
                        break;
                    default:
                        vaultName = string.Empty;
                        break;
                }

                if (string.IsNullOrEmpty(vaultName))
                    return;

                vault.Add(new PasswordCredential(vaultName, "Token", token.AccessToken));
                vault.Add(new PasswordCredential(vaultName, "RefreshToken", string.IsNullOrEmpty(token.RefreshToken) ? "n/a" : token.RefreshToken));
                vault.Add(new PasswordCredential(vaultName, "ExpireTime", string.IsNullOrEmpty(token.ExpireTime) ? "n/a" : token.ExpireTime));

                // Track the connect event
                Telemetry.TrackEvent("Service Connected",
                    new Dictionary<string, string>
                    {
                        {"Service", type.ToString()}
                    });

                try
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        // Update the UI depending if we are logged in or not
                        if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud) ||
                            SoundByteService.Current.IsServiceConnected(ServiceType.YouTube) ||
                            SoundByteService.Current.IsServiceConnected(ServiceType.Fanburst))
                            Shell.ShowLoginContent();
                        else
                            Shell.ShowLogoutContent();
                    });
                }
                catch
                {
                    // Ignore
                }           
            };

            // Run this code when a service is disconencted from SoundByte
            SoundByteService.Current.OnServiceDisconnected += type =>
            {
                // Get the password vault
                var vault = new PasswordVault();

                // Delte the vault depending on the service type
                switch (type)
                {
                    case ServiceType.SoundCloud:
                    case ServiceType.SoundCloudV2:
                        vault.FindAllByResource("SoundByte.SoundCloud").ForEach(x => vault.Remove(x));
                        break;
                    case ServiceType.Fanburst:
                        vault.FindAllByResource("SoundByte.FanBurst").ForEach(x => vault.Remove(x));
                        break;
                    case ServiceType.YouTube:
                        vault.FindAllByResource("SoundByte.YouTube").ForEach(x => vault.Remove(x));
                        break;
                    case ServiceType.SoundByte:
                        vault.FindAllByResource("SoundByte.SoundByte").ForEach(x => vault.Remove(x));
                        break;
                }

                // Track the disconnect event
                Telemetry.TrackEvent("Service Disconnected",
                    new Dictionary<string, string>
                    {
                        {"Service", type.ToString()}
                    });

                // Navigate to the explore view
                NavigateTo(typeof(ExploreView));

                // Update the UI depending if we are logged in or not
                if (SoundByteService.Current.IsServiceConnected(ServiceType.SoundCloud) ||
                    SoundByteService.Current.IsServiceConnected(ServiceType.YouTube) ||
                    SoundByteService.Current.IsServiceConnected(ServiceType.Fanburst))
                    Shell.ShowLoginContent();
                else
                    Shell.ShowLogoutContent();
            };
        }

        private LoginToken GetLoginTokenFromVault(string vaultName, ServiceType service)
        {
            // Get the password vault
            var vault = new PasswordVault();

            LoginToken loginToken;

            try
            {
                loginToken = new LoginToken
                {
                    AccessToken = vault.Retrieve(vaultName, "Token")?.Password,
                    ServiceType = service
                };
            }
            catch
            {
                return null;
            }

            try
            {
                loginToken.RefreshToken = vault.Retrieve(vaultName, "RefreshToken")?.Password;
                loginToken.ExpireTime = vault.Retrieve(vaultName, "ExpireTime")?.Password;
            }
            catch
            {
                // Ignore. In version 17.10, refresh and expire times were not used,
                // so the above will cause an exception when updaating to the latest version.
                // Normally the crash would indicate that the user is not logged in, but in fact
                // they are. So we just ignore this.
            }

            return loginToken;
        }

        private void InitV3Service()
        {
            var soundCloudToken = GetLoginTokenFromVault("SoundByte.SoundCloud", ServiceType.SoundCloud);
            var fanburstToken = GetLoginTokenFromVault("SoundByte.FanBurst", ServiceType.Fanburst);
            var youTubeToken = GetLoginTokenFromVault("SoundByte.YouTube", ServiceType.YouTube);
            var soundByteToken = GetLoginTokenFromVault("SoundByte.SoundByte", ServiceType.SoundByte);


            var secretList = new List<ServiceInfo>
            {
                new ServiceInfo
                {
                    Service = ServiceType.SoundCloud,
                    ClientIds = AppKeysHelper.SoundCloudPlaybackIds,
                    ClientId = AppKeysHelper.SoundCloudClientId,
                    UserToken = soundCloudToken
                },
                new ServiceInfo
                {
                    Service = ServiceType.SoundCloudV2,
                    ClientIds = AppKeysHelper.SoundCloudPlaybackIds,
                    ClientId = AppKeysHelper.SoundCloudClientId,
                    UserToken = soundCloudToken
                },
                new ServiceInfo
                {
                    Service = ServiceType.Fanburst,
                    ClientId = AppKeysHelper.FanburstClientId,
                    UserToken = fanburstToken
                },
                new ServiceInfo
                {
                    Service = ServiceType.YouTube,
                    ClientId = AppKeysHelper.YouTubeClientId,
                    UserToken = youTubeToken
                },
                new ServiceInfo
                {
                    Service = ServiceType.ITunesPodcast
                },
                new ServiceInfo
                {
                    Service = ServiceType.Local
                },
                new ServiceInfo
                {
                    Service = ServiceType.SoundByte,
                    ClientId = AppKeysHelper.SoundByteClientId,
                    UserToken = soundByteToken
                }
            };

            SoundByteService.Current.Init(secretList);
            LoggingService.Log(LoggingService.LogType.Debug, "SoundByte V3 Service Started");
        }

        #endregion

        #region View Events

        /// <summary>
        ///     Open the compact overlay view
        /// </summary>
        /// <returns></returns>
        public static async Task SwitchToCompactView()
        {
            var compactView = CoreApplication.CreateNewView();
            var compactViewId = -1;

            // Create a new window within the view
            await compactView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Create a new frame and navigate it to the overlay view
                var overlayFrame = new Frame();
                overlayFrame.Navigate(typeof(OverlayView));

                // Set the window content and activate it
                Window.Current.Content = overlayFrame;
                Window.Current.Activate();

                // Get the Id back
                compactViewId = ApplicationView.GetForCurrentView().Id;
            });

            // Make the overlay small
            var compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Size(350, 150);

            // Display as compact overlay
            await ApplicationViewSwitcher.TryShowAsViewModeAsync(compactViewId, ApplicationViewMode.CompactOverlay,
                compactOptions);
        }

        #endregion

        private bool _isCtrlKeyPressed;

        #region Key Events
        private async void CoreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Control) _isCtrlKeyPressed = true;

            switch (args.VirtualKey)
            {
                case VirtualKey.F11:
                    // Send hit
                    Telemetry.TrackEvent("Toggle FullScreen");
                    // Toggle between fullscreen or not
                    if (!DeviceHelper.IsDeviceFullScreen)
                        ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                    else
                        ApplicationView.GetForCurrentView().ExitFullScreenMode();
                    break;
                case VirtualKey.GamepadY:
                    // Send hit
                    Telemetry.TrackEvent("Xbox Playing Page");
                    // Navigate to the current playing track
                    NavigateTo(typeof(NowPlayingView));
                    break;
                case VirtualKey.F12: // Simulates xbox memory cleanup process
                    if (_isCtrlKeyPressed)
                    {
                        DeviceHelper.IsBackground = true;
                        await ReduceMemoryUsageAsync();
                    }
                    break;
                case VirtualKey.Back:


                    if (_isCtrlKeyPressed)
                        CurrentFrame.Frame.GoBack();
                    break;
            }
        }

        #endregion

        /// <summary>
        ///     Creates the window and performs any protocol logic needed
        /// </summary>
        /// <param name="parameters">Param string, (soundbyte://core/user?id=454345)</param>
        /// <returns></returns>
        private async Task InitializeShellAsync(string parameters = null)
        {
            LoggingService.Log(LoggingService.LogType.Debug, "Initialize Main App Shell...");

            _isInit = true;

            // Live tile helpers
            TileHelper.Init();

            // Before we init the v3 service, we must check to see if we have the required API keys
            if (!AppKeysHelper.KeysValid)
            {
                LoggingService.Log(LoggingService.LogType.Info, "App keys are not valid. Requesting new keys.");

                // If this fails getting the keys, we have an issue and must close the app
                if (!await UWPAuthorizationHelpers.OnlineAppInitAsync(true))
                    return;

                OnlineAppInitComplete = true;
            }

            // Init service
            InitV3Service();

            // Init the telemetry service
            await Telemetry.InitAsync(AppKeysHelper.GoogleAnalyticsTrackerId, AppKeysHelper.HockeyAppClientId,
                AppKeysHelper.AppCenterClientId);

            // Get the main shell
            var shell = Window.Current.Content as AppShell;

            // If the shell is null, we need to set it up.
            if (shell == null)
            {
                LoggingService.Log(LoggingService.LogType.Debug, "Shell does not exist, creating new shell");
                shell = new AppShell(parameters);

                // Hook the key pressed event for the global app
                Window.Current.CoreWindow.KeyDown += CoreWindowOnKeyDown;
                Window.Current.CoreWindow.KeyUp += (s, e) =>
                {
                    if (e.VirtualKey == VirtualKey.Control) _isCtrlKeyPressed = false;
                };
            }
            else
            {
                LoggingService.Log(LoggingService.LogType.Debug, "Shell exists, running protocol logic");
                await shell.HandleProtocolAsync(parameters);
            }

            // Set the root shell as the window content
            Window.Current.Content = shell;

            // If on xbox display the screen to the full width and height
            if (DeviceHelper.IsXbox)
                ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            // Activate the window
            LoggingService.Log(LoggingService.LogType.Debug, "Activiating Window");
            Window.Current.Activate();
        }

        #region Static App Helpers

        /// <summary>
        ///     Navigate to a certain page using the main shells
        ///     rootfrom navigate method
        /// </summary>
        public static async void NavigateTo(Type page, object param = null)
        {
            LoggingService.Log(LoggingService.LogType.Debug, "Navigating Page: " + page.Name);

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                (Window.Current.Content as AppShell)?.RootFrame.Navigate(page, param);
            });
        }

        /// <summary>
        ///     Stops the back event from being called, allowing for manual overiding
        /// </summary>
        public static bool OverrideBackEvent { get; set; }

        public static Page CurrentFrame => (Window.Current?.Content as AppShell)?.RootFrame.Content as Page;

        public static AppShell Shell => Window.Current?.Content as AppShell;

        /// <summary>
        /// Updates the UI to either show a loading ring or not
        /// </summary>
        /// <param name="isLoading">Is the app loading</param>
        /// <returns>A async task</returns>
        public static async Task SetLoadingAsync(bool isLoading)
        {
            // Don't run in background
            if (DeviceHelper.IsBackground)
                return;

            try
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    if ((Window.Current?.Content as AppShell)?.FindName("LoadingRing") is ProgressBar loadingRing)
                        loadingRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
                });
            }
            catch (Exception ex)
            {
                // This can crash if the UI thread does not exist.
                // 99.9% of the time, the background switch will prevent
                // this from happening though.
                LoggingService.Log(LoggingService.LogType.Warning, "Exception when setting loading status: " + ex.Message);
            }
        }

        #endregion

        #region Background Handlers

        /// <summary>
        ///     The application is leaving the background.
        /// </summary>
        private async void AppLeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            // Mark the transition out of the background state
            DeviceHelper.IsBackground = false;

            // Send hit
            Telemetry.TrackEvent("Leave Background");

            // Restore view content if it was previously unloaded
            if (Window.Current != null && Window.Current.Content == null && !_isInit)
            {
                LoggingService.Log(LoggingService.LogType.Debug, "App Enter Foreground");
                await InitializeShellAsync();
            }
        }

        /// <summary>
        ///     The application entered the background.
        /// </summary>
        private void AppEnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();

            try
            {
                // Send hit
                Telemetry.TrackEvent("Enter Background");

                // Update the variable
                DeviceHelper.IsBackground = true;
            }
            finally
            {
                deferral.Complete();
            }
        }

        #endregion

        #region Memory Handlers
        /// <summary>
        ///     Raised when the memory limit for the app is changing, such as when the app
        ///     enters the background.
        /// </summary>
        /// <remarks>
        ///     If the app is using more than the new limit, it must reduce memory within 2 seconds
        ///     on some platforms in order to avoid being suspended or terminated.
        ///     While some platforms will allow the application
        ///     to continue running over the limit, reducing usage in the time
        ///     allotted will enable the best experience across the broadest range of devices.
        /// </remarks>
        private async void MemoryManager_AppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            // If app memory usage is over the limit, reduce usage within 2 seconds
            // so that the system does not suspend the app
            if (MemoryManager.AppMemoryUsage >= e.NewLimit)
                await ReduceMemoryUsageAsync();

            // Send hit
            Telemetry.TrackEvent("Reducing Memory Usage", new Dictionary<string, string>
            {
                {"NewLimit", e.NewLimit / 1024 / 1024 + "M"},
                {"OldLimit", e.OldLimit / 1024 / 1024 + "M"},
                {"CurrentUsage", MemoryManager.AppMemoryUsage / 1024 / 1024 + "M"}
            });
        }

        /// <summary>
        ///     Handle system notifications that the app has increased its
        ///     memory usage level compared to its current target.
        /// </summary>
        /// <remarks>
        ///     The app may have increased its usage or the app may have moved
        ///     to the background and the system lowered the target for the app
        ///     In either case, if the application wants to maintain its priority
        ///     to avoid being suspended before other apps, it may need to reduce
        ///     its memory usage.
        ///     This is not a replacement for handling AppMemoryUsageLimitChanging
        ///     which is critical to ensure the app immediately gets below the new
        ///     limit. However, once the app is allowed to continue running and
        ///     policy is applied, some apps may wish to continue monitoring
        ///     usage to ensure they remain below the limit.
        /// </remarks>
        private async void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {
            // Obtain the current usage level
            var level = MemoryManager.AppMemoryUsageLevel;

            // Check the usage level to determine whether reducing memory is necessary.
            // Memory usage may have been fine when initially entering the background but
            // the app may have increased its memory usage since then and will need to trim back.
            if (level == AppMemoryUsageLevel.OverLimit || level == AppMemoryUsageLevel.High)
                await ReduceMemoryUsageAsync();

            // Send hit
            Telemetry.TrackEvent("Memory Usage Increader", new Dictionary<string, string>
            {
                {"CurrentUsage", MemoryManager.AppMemoryUsage / 1024 / 1024 + "M"}
            });
        }

        /// <summary>
        ///     Reduces application memory usage.
        /// </summary>
        /// <remarks>
        ///     When the app enters the background, receives a memory limit changing
        ///     event, or receives a memory usage increased event, it can
        ///     can optionally unload cached data or even its view content in
        ///     order to reduce memory usage and the chance of being suspended.
        ///     This must be called from multiple event handlers because an application may already
        ///     be in a high memory usage state when entering the background, or it
        ///     may be in a low memory usage state with no need to unload resources yet
        ///     and only enter a higher state later.
        /// </remarks>
        private async Task ReduceMemoryUsageAsync()
        {
            try
            {
                // Run on UI thread
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    // Additionally, if the application is currently
                    // in background mode and still has a view with content
                    // then the view can be released to save memory and
                    // can be recreated again later when leaving the background.
                    if (!DeviceHelper.IsBackground || Window.Current == null || Window.Current.Content == null) return;

                    // Get the main shell and only continue if it exists
                    var shell = Window.Current.Content as AppShell;
                    if (shell == null) return;

                    shell.RootFrame.Navigate(typeof(BlankView));
                    shell.Dispose();

                    // Clear the page cache
                    var cacheSize = shell.RootFrame.CacheSize;
                    shell.RootFrame.CacheSize = 0;
                    shell.RootFrame.CacheSize = cacheSize;

                    // Clear backstack
                    shell.RootFrame.BackStack.Clear();

                    // Clear refrences
                    VisualTreeHelper.DisconnectChildrenRecursive(shell.RootFrame);
                    VisualTreeHelper.DisconnectChildrenRecursive(shell);

                    shell.RootFrame = null;

                    // Clear the view content. Note that views should rely on
                    // events like Page.Unloaded to further release resources.
                    // Release event handlers in views since references can
                    // prevent objects from being collected.
                    Window.Current.Content = null;

                    GC.Collect();
                });
            }
            catch
            {
                // This will crash if no main view is active
            }

            // Run the GC to collect released resources on background thread.
            GC.Collect();
        }

        #endregion

        #region Launch / Activate Events

        /// <summary>
        ///     Called when the app is activated.
        /// </summary>
        protected override async void OnActivated(IActivatedEventArgs e)
        {
            var path = string.Empty;

            LoggingService.Log(LoggingService.LogType.Debug, "App Activate Requested");

            // Handle all the activation protocols that could occure
            switch (e.Kind)
            {
                // We were launched using the protocol
                case ActivationKind.Protocol:
                    if (e is ProtocolActivatedEventArgs protoArgs)
                        path = protoArgs.Uri.ToString();
                    break;
                case ActivationKind.ToastNotification:
                    if (e is IToastNotificationActivatedEventArgs toastArgs)
                        path = toastArgs.Argument;

                    // Track app launched through dev center
                  //  var engagementManager = StoreServicesEngagementManager.GetDefault();
                  //  engagementManager.ParseArgumentsAndTrackAppLaunch(path);
                  // TODO: NUGET FILE TOO LONG

                    break;
                case ActivationKind.VoiceCommand:
                    if (e is VoiceCommandActivatedEventArgs voiceArgs)
                        path = voiceArgs.Result.RulePath[0];
                    break;
            }

            await InitializeShellAsync(path);
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var path = string.Empty;

            // Handle all the activation protocols that could occure
            if (!string.IsNullOrEmpty(e.TileId))
                path = e.Arguments;

            LoggingService.Log(LoggingService.LogType.Debug, "App Launch Requested");

            // If this is just a prelaunch, don't 
            // actually set the content to the frame.
            if (e.PrelaunchActivated) return;

            // Create / Get the main shell
            await InitializeShellAsync(path);
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            // Load the shell
            LoggingService.Log(LoggingService.LogType.Debug, "App File Activation");
            await InitializeShellAsync();

            // Start playing content

            // Convert to base tracks
            var tracks = new List<BaseTrack>();

            var random = new Random();

            foreach (var item in args.Files)
            {
                var file = item as StorageFile;

                if (file == null)
                    continue;

               // var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 512);

                var properties = await file.Properties.GetMusicPropertiesAsync();

                var track = new BaseTrack
                {
                    ServiceType = ServiceType.Local,
                    AudioStreamUrl = file.Path,
                    Title = GetNullString(properties.Title, file.DisplayName),
                    User = new BaseUser
                    {
                        ServiceType = ServiceType.Local,
                        Username = GetNullString(properties.Artist, "Unknown Artist")
                    },
                    Genre = string.Join(',', properties.Genre),
                    Duration = properties.Duration,
                    TrackId = random.Next(0, 999999999).ToString()
                };

                track.CustomProperties.Add("File", file);

                tracks.Add(track);
            }

            var result = await PlaybackService.Instance.InitilizePlaylistAsync<DummyTrackSource>(tracks);

            if (result.Success)
            {
                await PlaybackService.Instance.StartTrackAsync();
                return;
            }

            await NavigationService.Current.CallMessageDialogAsync(result.Message);
        }

        private string GetNullString(string text, string backupText)
        {
            return string.IsNullOrEmpty(text) 
                ? backupText 
                : text;
        }
        #endregion
        }
}