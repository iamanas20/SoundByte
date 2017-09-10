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
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Security.Credentials;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items;
using SoundByte.Core.Services;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using SoundByte.UWP.Views.CoreApp;
using UICompositionAnimations.Lights;
using WinRTXamlToolkit.Tools;

namespace SoundByte.UWP
{
    sealed partial class App
    {
        #region App Setup

        /// <summary>
        ///     This is the main class for this app. This function is the first function
        ///     called and it setups the app analytic (If in release mode), components,
        ///     requested theme and event handlers.
        /// </summary>
        public App()
        {
            // Init XAML Resources
            InitializeComponent();

            // We want to use the controler if on xbox
            if (DeviceHelper.IsXbox)
                RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;

            // Check that we are not using the default theme,
            // if not change the requested theme to the users
            // picked theme.
            if (!SettingsService.Instance.IsDefaultTheme)
                RequestedTheme = SettingsService.Instance.ThemeType;

            // Registor the dialogs
            NavigationService.Current.RegisterTypeAsDialog<CrashDialog>();
            NavigationService.Current.RegisterTypeAsDialog<FilterDialog>();
            NavigationService.Current.RegisterTypeAsDialog<PendingUpdateDialog>();
            NavigationService.Current.RegisterTypeAsDialog<PinTileDialog>();
            NavigationService.Current.RegisterTypeAsDialog<PlaylistDialog>();
            NavigationService.Current.RegisterTypeAsDialog<ShareDialog>();

            // Init Keys
            ApiKeyService.Init();

            // Init service
            InitV3Service();

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
            SoundByteV3Service.Current.OnServiceConnected += (type, token) =>
            {
                var vault = new PasswordVault();

                // Add the password to the vault so we can access it when restarting the app
                switch (type)
                {
                    case ServiceType.SoundCloud:
                    case ServiceType.SoundCloudV2:
                        vault.Add(new PasswordCredential("SoundByte.SoundCloud", "Token", token.AccessToken));
                        vault.Add(new PasswordCredential("SoundByte.SoundCloud", "Scope", token.Scope));
                        break;
                    case ServiceType.Fanburst:
                        vault.Add(new PasswordCredential("SoundByte.FanBurst", "Token", token.AccessToken));
                        break;
                    case ServiceType.YouTube:
                        vault.Add(new PasswordCredential("SoundByte.YouTube", "Token", token.AccessToken));
                        break;
                }

                // Track the connect event
                TelemetryService.Instance.TrackEvent("Service Connected",
                    new Dictionary<string, string>
                    {
                        {"Service", type.ToString()}
                    });

                // Navigate home if we connected SoundCloud, else navigate to explore
                NavigateTo(type == ServiceType.SoundCloud ? typeof(HomeView) : typeof(ExploreView));
            };

            // Run this code when a service is disconencted from SoundByte
            SoundByteV3Service.Current.OnServiceDisconnected += type =>
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
                }

                // Track the disconnect event
                TelemetryService.Instance.TrackEvent("Service Disconnected",
                    new Dictionary<string, string>
                    {
                        {"Service", type.ToString()}
                    });

                // Navigate to the explore view
                NavigateTo(typeof(ExploreView));
            };
        }

        private void InitV3Service()
        {
            // Get the password vault
            var vault = new PasswordVault();

            LoginToken soundCloudToken = null;
            LoginToken fanburstToken = null;
            LoginToken youTubeToken = null;

            try
            {
                var soundCloudResource = vault.FindAllByResource("SoundByte.SoundCloud");
                if (soundCloudResource != null)
                    soundCloudToken = new LoginToken { AccessToken = vault.Retrieve("SoundByte.SoundCloud", "Token").Password };
            }
            catch
            {
                // ignored
            }

            try
            {
                var fanburstResource = vault.FindAllByResource("SoundByte.FanBurst");
                if (fanburstResource != null)
                    fanburstToken = new LoginToken { AccessToken = vault.Retrieve("SoundByte.FanBurst", "Token").Password };
            }
            catch
            {
                // ignored
            }

            try
            {
                var youTubeResource = vault.FindAllByResource("SoundByte.YouTube");
                if (youTubeResource != null)
                    youTubeToken = new LoginToken { AccessToken = vault.Retrieve("SoundByte.YouTube", "Token").Password };
            }
            catch
            {
                // ignored
            }

            var secretList = new List<ServiceSecret>
            {
                new ServiceSecret
                {
                    Service = ServiceType.SoundCloud,
                    ClientId = ApiKeyService.SoundCloudClientId,
                    ClientSecret = ApiKeyService.SoundCloudClientSecret,
                    UserToken = soundCloudToken
                },
                new ServiceSecret
                {
                    Service = ServiceType.SoundCloudV2,
                    ClientId = ApiKeyService.SoundCloudClientId,
                    ClientSecret = ApiKeyService.SoundCloudClientSecret,
                    UserToken = soundCloudToken
                },
                new ServiceSecret
                {
                    Service = ServiceType.Fanburst,
                    ClientId = ApiKeyService.FanburstClientId,
                    ClientSecret = ApiKeyService.FanburstClientSecret,
                    UserToken = fanburstToken
                },
                new ServiceSecret
                {
                    Service = ServiceType.YouTube,
                    ClientId = ApiKeyService.YouTubeClientId,
                    UserToken = youTubeToken
                }
            };

            SoundByteV3Service.Current.Init(secretList);
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

        #region Key Events

        private void CoreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.F11:
                    // Send hit
                    TelemetryService.Instance.TrackEvent("Toggle FullScreen");
                    // Toggle between fullscreen or not
                    if (!DeviceHelper.IsDeviceFullScreen)
                        ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                    else
                        ApplicationView.GetForCurrentView().ExitFullScreenMode();
                    break;
                case VirtualKey.GamepadView:
                    // Send hit
                    TelemetryService.Instance.TrackEvent("Xbox Playing Page");
                    // Navigate to the current playing track
                    NavigateTo(typeof(NowPlayingView));
                    break;
                case VirtualKey.GamepadY:
                    // Send hit
                    TelemetryService.Instance.TrackEvent("Xbox Search Page");
                    // Navigate to the search page
                    NavigateTo(typeof(SearchView));
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
            // Get the main shell
            var shell = Window.Current.Content as MainShell;

            // If the shell is null, we need to set it up.
            if (shell == null)
            {
                shell = new MainShell(parameters);

                // Hook the key pressed event for the global app
                Window.Current.CoreWindow.KeyUp += CoreWindowOnKeyUp;
            }
            else
            {
                await shell.HandleProtocolAsync(parameters);
            }

            // Set the root shell as the window content
            Window.Current.Content = shell;

            try
            {
                // Setup the lights
                LightsSourceHelper.Initialize(() => new PointerPositionSpotLight());
            }
            catch
            {
                // Temp fix around light helper already existing, proper fix later
            }

            LightsSourceHelper.SetIsLightsContainer(Window.Current.Content, true);

            // If on xbox display the screen to the full width and height
            if (DeviceHelper.IsXbox)
                ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            // Activate the window
            Window.Current.Activate();
        }

        #region Static App Helpers

        /// <summary>
        ///     Navigate to a certain page using the main shells
        ///     rootfrom navigate method
        /// </summary>
        public static async void NavigateTo(Type page, object param = null)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                (Window.Current.Content as MainShell)?.RootFrame.Navigate(page, param);
            });
        }

        /// <summary>
        ///     Stops the back event from being called, allowing for manual overiding
        /// </summary>
        public static bool OverrideBackEvent { get; set; }

        public static Page CurrentFrame => (Window.Current?.Content as MainShell)?.RootFrame.Content as Page;

        public static MainShell Shell => Window.Current?.Content as MainShell;

        /// <summary>
        ///     Is anything currently loading
        /// </summary>
        public static bool IsLoading
        {
            set
            {
                if ((Window.Current?.Content as MainShell)?.FindName("LoadingRing") is ProgressBar loadingRing)
                    loadingRing.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
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
            TelemetryService.Instance.TrackEvent("Leave Background");

            // Restore view content if it was previously unloaded
            if (Window.Current != null && Window.Current.Content == null)
            {
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
                TelemetryService.Instance.TrackEvent("Enter Background");

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
            TelemetryService.Instance.TrackEvent("Reducing Memory Usage", new Dictionary<string, string>
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
            if (level != AppMemoryUsageLevel.OverLimit && level != AppMemoryUsageLevel.High)
                await ReduceMemoryUsageAsync();

            // Send hit
            TelemetryService.Instance.TrackEvent("Memory Usage Increader", new Dictionary<string, string>
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
                    var shell = Window.Current.Content as MainShell;
                    if (shell == null) return;

                    shell.RootFrame.Navigate(typeof(BlankPage));
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

            // Handle all the activation protocols that could occure
            switch (e.Kind)
            {
                // We were launched using the protocol
                case ActivationKind.Protocol:
                    var protoArgs = e as ProtocolActivatedEventArgs;
                    if (protoArgs != null) path = protoArgs.Uri.ToString();
                    break;
                case ActivationKind.ToastNotification:
                    var toastArgs = e as IToastNotificationActivatedEventArgs;
                    if (toastArgs != null) path = toastArgs.Argument;
                    break;
                case ActivationKind.VoiceCommand:
                    var voiceArgs = e as VoiceCommandActivatedEventArgs;
                    if (voiceArgs != null) path = voiceArgs.Result.RulePath[0];
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

            // If this is just a prelaunch, don't 
            // actually set the content to the frame.
            if (e.PrelaunchActivated) return;

            // Create / Get the main shell
            await InitializeShellAsync(path);
        }
        #endregion
    }
}