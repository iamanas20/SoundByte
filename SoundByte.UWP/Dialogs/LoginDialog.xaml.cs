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
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Helpers;
using SoundByte.Core.Services;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using QueryParameterCollection = Microsoft.Toolkit.Uwp.Helpers.QueryParameterCollection;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class LoginDialog 
    {
        private readonly ServiceType _loginService;
        private readonly string _stateVerification;
        private readonly bool _isRemoteConnect;
        private readonly string _loginCode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="isRemoteConnect"></param>
        /// <param name="loginCode"></param>
        public LoginDialog(ServiceType service, bool isRemoteConnect = false, string loginCode = "")
        {
            // Set the login service type
            _loginService = service;
            _stateVerification = new Random().Next(0, 100000000).ToString("D8");
            _isRemoteConnect = isRemoteConnect;
            _loginCode = loginCode;

            // Load the XAML page
            InitializeComponent();

            // Loading event handlers
            LoginWebView.NavigationStarting += (sender, args) => { LoadingSection.Visibility = Visibility.Visible; };
            LoginWebView.NavigationCompleted += (sender, args) => { LoadingSection.Visibility = Visibility.Collapsed; };

            // Handle new window requests, if a new window is requested, just navigate on the 
            // current page. 
            LoginWebView.NewWindowRequested += (view, eventArgs) =>
            {
                eventArgs.Handled = true;
                LoginWebView.Navigate(eventArgs.Uri);
            };

            // Set the callback
            var appCallback = Uri.EscapeUriString("http://localhost/soundbyte");

            // Create the URI
            string connectUri;

            switch (_loginService)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    connectUri =
                        $"https://soundcloud.com/connect?scope=non-expiring&client_id={AppKeysHelper.SoundCloudClientId}&response_type=code&display=popup&redirect_uri={appCallback}&state={_stateVerification}";
                    break;
                case ServiceType.Fanburst:
                    connectUri =
                        $"https://fanburst.com/oauth/authorize?client_id={AppKeysHelper.FanburstClientId}&response_type=code&redirect_uri={appCallback}&state={_stateVerification}&display=popup";
                    break;
                case ServiceType.YouTube:
                    connectUri =
                        $"https://accounts.google.com/o/oauth2/v2/auth?client_id={AppKeysHelper.YouTubeLoginClientId}&redirect_uri={appCallback}&response_type=code&state={_stateVerification}&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fyoutube";
                    break;
                case ServiceType.SoundByte:
                    connectUri =
                        $"https://soundbytemedia.com/connect/authorize?client_id={AppKeysHelper.SoundByteClientId}&response_type=code&redirect_uri={appCallback}&state={_stateVerification}&scope=api";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Show the web view and navigate to the connect URI
            LoginWebView.Visibility = Visibility.Visible;
            LoginWebView.Navigate(new Uri(connectUri));
        }

        private async void LoginWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs eventArgs)
        {
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
                    await NavigationService.Current.CallMessageDialogAsync(
                        "State Verfication Failed. This could be caused by another process intercepting the SoundByte login procedure. Signin has been canceled to protect your privacy.",
                        "Sign in Error");
                    App.Telemetry.TrackEvent("State Verfication Failed");

                    // Close
                    Hide();
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
                        Hide();
                        return;
                    }

                    // Display the error to the user
                    await NavigationService.Current.CallMessageDialogAsync(reason, "Sign in Error");
                    Hide();
                    return;
                }

                if (parser.FirstOrDefault(x => x.Key == "code").Value == null)
                {
                    await NavigationService.Current.CallMessageDialogAsync("No Code", "Sign in Error");
                    Hide();
                    return;
                }

                var code = parser.FirstOrDefault(x => x.Key == "code").Value;

                try
                {
                    var loginToken = await AuthorizationHelpers.GetAuthTokenAsync(_loginService, code);

                    if (!_isRemoteConnect)
                    {
                        // Connect the service
                        SoundByteService.Current.ConnectService(_loginService, loginToken);

                        // Close
                        Hide();
                    }
                    else
                    {
                        LoadingSection.Visibility = Visibility.Visible;

                        loginToken.LoginCode = _loginCode;
                        loginToken.ServiceType = _loginService;

                        var serviceResponse = await BackendService.Instance.LoginSendInfoAsync(loginToken);

                        if (string.IsNullOrEmpty(serviceResponse))
                        {
                            // Close
                            Hide();
                        }
                        else
                        {
                            await NavigationService.Current.CallMessageDialogAsync("Could not connect to your Xbox One. Please check that the codes match and try again.\n\n" + serviceResponse, 
                                "Xbox Connect Error");
                            // Close
                            Hide();
                        }
                    }
                }
                catch (SoundByteException ex)
                {
                    // Display the error to the user
                    await NavigationService.Current.CallMessageDialogAsync(ex.ErrorDescription, ex.ErrorTitle);

                    // Close
                    Hide();
                }
            }
        }
    }
}