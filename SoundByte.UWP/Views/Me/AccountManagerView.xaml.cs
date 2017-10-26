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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Items;
using SoundByte.Core.Services;
using SoundByte.UWP.Assets;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    ///     This page is used to login the user to SoundCloud so we can access their stream etc.
    /// </summary>
    public sealed partial class AccountManagerView
    {
        private readonly string _appCallback;
        private ServiceType _loginService;
        private string _stateVerification;
        private bool _isXboxConnect;

        public AccountManagerView()
        {
            // Load the XAML page
            InitializeComponent();

            LoginWebView.NavigationStarting += (sender, args) => { LoadingSection.Visibility = Visibility.Visible; };

            LoginWebView.NavigationCompleted += (sender, args) => { LoadingSection.Visibility = Visibility.Collapsed; };

            // On disconnect and connect events refresh the UI
            SoundByteV3Service.Current.OnServiceConnected += (type, token) => RefreshUi();
            SoundByteV3Service.Current.OnServiceDisconnected += type => RefreshUi();

            // Handle new window requests, if a new window is requested, just navigate on the 
            // current page. 
            LoginWebView.NewWindowRequested += (view, eventArgs) =>
            {
                eventArgs.Handled = true;
                LoginWebView.Navigate(eventArgs.Uri);
            };

            // Set the callback
            _appCallback = Uri.EscapeUriString("http://localhost/soundbyte");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Instance.TrackPage("Account View");

           
            MainView.Visibility = Visibility.Visible;
            ConnectAccountView.Visibility = Visibility.Collapsed;
            LoginWebView.Visibility = Visibility.Collapsed;

            if (DeviceHelper.IsXbox)
            {
                XboxConnectPanel.Visibility = Visibility.Collapsed;
                XboxConnectPanelHost.Visibility = Visibility.Visible;
            }

            RefreshUi();
        }

        private void RefreshUi()
        {
            if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud))
            {
                SoundCloudDisconnectAccount.Visibility = Visibility.Visible;
                SoundCloudViewProfile.Visibility = Visibility.Visible;
                SoundCloudConnectAccount.Visibility = Visibility.Collapsed;
            }
            else
            {
                SoundCloudDisconnectAccount.Visibility = Visibility.Collapsed;
                SoundCloudViewProfile.Visibility = Visibility.Collapsed;
                SoundCloudConnectAccount.Visibility = Visibility.Visible;
            }

            if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.Fanburst))
            {
                FanburstDisconnectAccount.Visibility = Visibility.Visible;
                FanburstViewProfile.Visibility = Visibility.Visible;
                FanburstConnectAccount.Visibility = Visibility.Collapsed;
            }
            else
            {
                FanburstDisconnectAccount.Visibility = Visibility.Collapsed;
                FanburstViewProfile.Visibility = Visibility.Collapsed;
                FanburstConnectAccount.Visibility = Visibility.Visible;
            }

            if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.YouTube))
            {
                YouTubeDisconnectAccount.Visibility = Visibility.Visible;
                YouTubeViewProfile.Visibility = Visibility.Visible;
                YouTubeConnectAccount.Visibility = Visibility.Collapsed;
            }
            else
            {
                YouTubeDisconnectAccount.Visibility = Visibility.Collapsed;
                YouTubeViewProfile.Visibility = Visibility.Collapsed;
                YouTubeConnectAccount.Visibility = Visibility.Visible;
            }

            // Update the UI depending if we are logged in or not
            if (SoundByteV3Service.Current.IsServiceConnected(ServiceType.SoundCloud) ||
                SoundByteV3Service.Current.IsServiceConnected(ServiceType.Fanburst))
                App.Shell.ShowLoginContent();
            else
                App.Shell.ShowLogoutContent();
        }

        private async void LoginWebView_OnNavigationStarting(WebView sender,
            WebViewNavigationStartingEventArgs eventArgs)
        {
            // We worry about localhost addresses are they are directed towards us.
            if (eventArgs.Uri.Host == "localhost")
            {
                // Cancel the navigation, (as localhost does not exist).
                eventArgs.Cancel = true;

                // Parse the URL for work
                // ReSharper disable once CollectionNeverUpdated.Local
                var parser = new QueryParameterCollection(eventArgs.Uri);

                // First we just check that the state equals (to make sure the url was not hijacked)
                var state = parser.FirstOrDefault(x => x.Key == "state").Value;

                // The state does not match
                if (string.IsNullOrEmpty(state) || state.TrimEnd('#') != _stateVerification)
                {
                    // Display the error to the user
                    await new MessageDialog(
                        "State Verfication Failed. This could be caused by another process intercepting the SoundByte login procedure. Signin has been canceled to protect your privacy.",
                        "Sign in Error").ShowAsync();
                    TelemetryService.Instance.TrackEvent("State Verfication Failed");
                    // Close
                    LoadingSection.Visibility = Visibility.Collapsed;
                    LoginWebView.Visibility = Visibility.Collapsed;
                    return;
                }

                // We have an error
                if (parser.FirstOrDefault(x => x.Key == "error").Value != null)
                {
                    var type = parser.FirstOrDefault(x => x.Key == "error").Value;
                    var reason = parser.FirstOrDefault(x => x.Key == "error_description").Value;

                    // The user denied the request
                    if (type == "access_denied")
                    {
                        LoadingSection.Visibility = Visibility.Collapsed;
                        LoginWebView.Visibility = Visibility.Collapsed;
                        return;
                    }

                    // Display the error to the user
                    await new MessageDialog(reason, "Sign in Error").ShowAsync();

                    // Close
                    LoadingSection.Visibility = Visibility.Collapsed;
                    LoginWebView.Visibility = Visibility.Collapsed;
                    return;
                }

                // Get the code from the url
                if (parser.FirstOrDefault(x => x.Key == "code").Value != null)
                {
                    var code = parser.FirstOrDefault(x => x.Key == "code").Value;

                    // Create a http client to get the token
                    using (var httpClient = new HttpClient())
                    {
                        // Set the user agent string
                        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SoundByte",
                            Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." +
                            Package.Current.Id.Version.Build));

                        // Get all the params
                        var parameters = new Dictionary<string, string>
                        {
                            {
                                "client_id",
                                _loginService == ServiceType.SoundCloud
                                    ? AppKeys.SoundCloudClientId
                                    : AppKeys.FanburstClientId
                            },
                            {
                                "client_secret",
                                _loginService == ServiceType.SoundCloud
                                    ? AppKeys.SoundCloudClientSecret
                                    : AppKeys.FanburstClientSecret
                            },
                            {"grant_type", "authorization_code"},
                            {"redirect_uri", _appCallback},
                            {"code", code}
                        };

                        var encodedContent = new FormUrlEncodedContent(parameters);

                        // Post to the soundcloud API
                        using (var postQuery =
                            await httpClient.PostAsync(
                                _loginService == ServiceType.SoundCloud
                                    ? "https://api.soundcloud.com/oauth2/token"
                                    : "https://fanburst.com/oauth/token", encodedContent))
                        {
                            // Check if the post was successful
                            if (postQuery.IsSuccessStatusCode)
                            {
                                // Get the stream
                                using (var stream = await postQuery.Content.ReadAsStreamAsync())
                                {
                                    // Read the stream
                                    using (var streamReader = new StreamReader(stream))
                                    {
                                        // Get the text from the stream
                                        using (var textReader = new JsonTextReader(streamReader))
                                        {
                                            // Used to get the data from JSON
                                            var serializer = new JsonSerializer
                                            {
                                                NullValueHandling = NullValueHandling.Ignore
                                            };

                                            // Get the class from the json
                                            var response = serializer.Deserialize<LoginToken>(textReader);

                                            if (_isXboxConnect)
                                            {
                                                LoadingSection.Visibility = Visibility.Visible;

                                                var serviceResponse = await BackendService.Instance.LoginSendInfoAsync(
                                                    new LoginToken
                                                    {
                                                        ServiceType = _loginService,
                                                        LoginCode = LoginCodeTextBox.Text,
                                                        AccessToken = response.AccessToken,
                                                        Scope = response.Scope
                                                    });

                                                if (string.IsNullOrEmpty(serviceResponse))
                                                {
                                                    LoadingSection.Visibility = Visibility.Collapsed;
                                                    LoginWebView.Visibility = Visibility.Collapsed;
                                                    RefreshUi();

                                                    TelemetryService.Instance.TrackEvent("Login Successful",
                                                        new Dictionary<string, string>
                                                        {
                                                            {"Service", "Xbox Connect"}
                                                        });

                                                    MainView.Visibility = Visibility.Visible;
                                                    ConnectAccountView.Visibility = Visibility.Collapsed;
                                                }
                                                else
                                                {
                                                    await new MessageDialog("Could not connect to your Xbox One. Please check that the codes match and try again.\n\n" + serviceResponse, "Xbox Connect Error").ShowAsync();
                                                    MainView.Visibility = Visibility.Visible;
                                                    ConnectAccountView.Visibility = Visibility.Collapsed;
                                                    LoadingSection.Visibility = Visibility.Collapsed;
                                                    LoginWebView.Visibility = Visibility.Collapsed;
                                                }                      
                                            }
                                            else
                                            {
                                                // Connect the service
                                                SoundByteV3Service.Current.ConnectService(_loginService, response);

                                                LoadingSection.Visibility = Visibility.Collapsed;
                                                LoginWebView.Visibility = Visibility.Collapsed;
                                            }                             
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Display the error to the user
                                await new MessageDialog("Token Error. Try again later.", "Sign in Error").ShowAsync();

                                // Close
                                LoadingSection.Visibility = Visibility.Collapsed;
                                LoginWebView.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
        }

        private async Task ConnectAccountAsync(ServiceType serviceType)
        {
            // Generate State (for security)
            _stateVerification = new Random().Next(0, 100000000).ToString("D8");
            _loginService = serviceType;

            // Create the URI
            var connectUri = string.Empty;

            switch (serviceType)
            {
                case ServiceType.SoundCloud:
                    connectUri =
                        $"https://soundcloud.com/connect?scope=non-expiring&client_id={AppKeys.SoundCloudClientId}&response_type=code&display=popup&redirect_uri={_appCallback}&state={_stateVerification}";
                    break;
                case ServiceType.Fanburst:
                    connectUri =
                        $"https://fanburst.com/oauth/authorize?client_id={AppKeys.FanburstClientId}&response_type=code&redirect_uri={_appCallback}&state={_stateVerification}";
                    break;
            }

            // Clear any webview cache
            await WebView.ClearTemporaryWebDataAsync();

            // Show the web view and navigate to the connect URI
            LoginWebView.Visibility = Visibility.Visible;
            LoginWebView.Navigate(new Uri(connectUri));
        }


       

        private void ConnectXboxOne(object sender, RoutedEventArgs e)
        {
            MainView.Visibility = Visibility.Collapsed;
            ConnectAccountView.Visibility = Visibility.Visible;
            LoginCodeTextBox.Text = string.Empty;
        }

        private async void XboxOneConnectRequest(object sender, RoutedEventArgs e)
        {
            _isXboxConnect = true;
            await ConnectAccountAsync(ServiceType.SoundCloud);         
        }

        private void NavigateToXboxConnect(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(XboxAccountView));
        }

        private void XboxConnectGoBack(object sender, RoutedEventArgs e)
        {
            MainView.Visibility = Visibility.Visible;
            ConnectAccountView.Visibility = Visibility.Collapsed;
        }




        #region Navigate Profile Methods
        private void NavigateSoundCloudProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteV3Service.Current.GetConnectedUser(ServiceType.SoundCloud));
        }

        private void NavigateFanburstProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteV3Service.Current.GetConnectedUser(ServiceType.Fanburst));
        }

        private void NavigateYouTubeProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(UserView), SoundByteV3Service.Current.GetConnectedUser(ServiceType.YouTube));
        }
        #endregion

        #region Connect Account Methods
        private async void ConnectSoundCloudAccount(object sender, RoutedEventArgs e)
        {
            // Disable Xbox Mode
            _isXboxConnect = false;

            // Connect Account
            await ConnectAccountAsync(ServiceType.SoundCloud);      
        }

        private async void ConnectFanburstAccount(object sender, RoutedEventArgs e)
        {
            // Disable Xbox Mode
            _isXboxConnect = false;

            // Connect Account
            await ConnectAccountAsync(ServiceType.Fanburst);
        }

        private async void ConnectYouTubeAccount(object sender, RoutedEventArgs e)
        {
            // Disable Xbox Mode
            _isXboxConnect = false;
            
            // Connect Account
            await ConnectAccountAsync(ServiceType.YouTube);
        }
        #endregion

        #region Disconnect Account Methods
        private void DisconnectSoundCloudAccount(object sender, RoutedEventArgs e)
        {
            SoundByteV3Service.Current.DisconnectService(ServiceType.SoundCloud);
            RefreshUi();
        }

        private void DisconnectFanburstAccount(object sender, RoutedEventArgs e)
        {
            SoundByteV3Service.Current.DisconnectService(ServiceType.Fanburst);
            RefreshUi();
        }

        private void DisconnectYouTubeAccount(object sender, RoutedEventArgs e)
        {
            SoundByteV3Service.Current.DisconnectService(ServiceType.YouTube);
            RefreshUi();
        }
        #endregion
    }
}