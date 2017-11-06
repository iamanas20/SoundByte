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
using Windows.ApplicationModel;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Items;
using SoundByte.Core.Services;
using SoundByte.UWP.Assets;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class LoginDialog 
    {
        private readonly string _appCallback;
        private ServiceType _loginService;
        private string _stateVerification;
        private bool _isRemoteConnect;
        private string _loginCode;

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
            _appCallback = Uri.EscapeUriString("http://localhost/soundbyte");

            // Create the URI
            string connectUri;

            switch (_loginService)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    connectUri =
                        $"https://soundcloud.com/connect?scope=non-expiring&client_id={AppKeys.SoundCloudClientId}&response_type=code&display=popup&redirect_uri={_appCallback}&state={_stateVerification}";
                    break;
                case ServiceType.Fanburst:
                    connectUri =
                        $"https://fanburst.com/oauth/authorize?client_id={AppKeys.FanburstClientId}&response_type=code&redirect_uri={_appCallback}&state={_stateVerification}&display=popup";
                    break;
                case ServiceType.YouTube:
                    connectUri =
                        $"https://accounts.google.com/o/oauth2/v2/auth?client_id={AppKeys.YouTubeLoginClientId}&redirect_uri={_appCallback}&response_type=code&state={_stateVerification}&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fyoutube.readonly";
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
                    await new MessageDialog(
                        "State Verfication Failed. This could be caused by another process intercepting the SoundByte login procedure. Signin has been canceled to protect your privacy.",
                        "Sign in Error").ShowAsync();
                    TelemetryService.Instance.TrackEvent("State Verfication Failed");

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
                    await new MessageDialog(reason, "Sign in Error").ShowAsync();
                    Hide();
                    return;
                }

                if (parser.FirstOrDefault(x => x.Key == "code").Value == null)
                {
                    await new MessageDialog("No Code", "Sign in Error").ShowAsync();
                    Hide();
                    return;
                }

                var code = parser.FirstOrDefault(x => x.Key == "code").Value;

                // Create a http client to get the token
                using (var httpClient = new HttpClient())
                {
                    // Set the user agent string
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SoundByte",
                        Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." +
                        Package.Current.Id.Version.Build));

                    // Get the correct client ID
                    string clientId;
                    switch (_loginService)
                    {
                        case ServiceType.SoundCloud:
                        case ServiceType.SoundCloudV2:
                            clientId = AppKeys.SoundCloudClientId;
                            break;
                        case ServiceType.Fanburst:
                            clientId = AppKeys.FanburstClientId;
                            break;
                        case ServiceType.YouTube:
                            clientId = AppKeys.YouTubeLoginClientId;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // Get the correct client secret
                    string clientSecret;
                    switch (_loginService)
                    {
                        case ServiceType.SoundCloud:
                        case ServiceType.SoundCloudV2:
                            clientSecret = AppKeys.SoundCloudClientSecret;
                            break;
                        case ServiceType.Fanburst:
                            clientSecret = AppKeys.FanburstClientSecret;
                            break;
                        case ServiceType.YouTube:
                            clientSecret = "";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // Get all the params
                    var parameters = new Dictionary<string, string>
                    {
                        {"client_id", clientId },
                        { "client_secret", clientSecret },
                        { "grant_type", "authorization_code" },
                        { "redirect_uri", _appCallback },
                        { "code", code }
                    };

                    var encodedContent = new FormUrlEncodedContent(parameters);

                    string postUrl;
                    switch (_loginService)
                    {
                        case ServiceType.SoundCloud:
                        case ServiceType.SoundCloudV2:
                            postUrl = "https://api.soundcloud.com/oauth2/token";
                            break;
                        case ServiceType.Fanburst:
                            postUrl = "https://fanburst.com/oauth/token";
                            break;
                        case ServiceType.YouTube:
                            postUrl = "https://www.googleapis.com/oauth2/v4/token";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    try
                    {
                        // Post to the the respected API
                        using (var postQuery = await httpClient.PostAsync(postUrl, encodedContent))
                        {
                            postQuery.EnsureSuccessStatusCode();

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

                                        if (!_isRemoteConnect)
                                        {
                                            // Connect the service
                                            SoundByteV3Service.Current.ConnectService(_loginService, response);

                                            // Close
                                            Hide();
                                        }
                                        else
                                        {
                                            LoadingSection.Visibility = Visibility.Visible;

                                            var loginToken = response;
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
                                                await new MessageDialog("Could not connect to your Xbox One. Please check that the codes match and try again.\n\n" + serviceResponse, "Xbox Connect Error").ShowAsync();
                                                // Close
                                                Hide();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Display the error to the user
                        await new MessageDialog("Token Error. Try again later.\n" + ex.Message, "Sign in Error").ShowAsync();

                        // Close
                        Hide();
                    }
                }
            }
        }
    }
}
