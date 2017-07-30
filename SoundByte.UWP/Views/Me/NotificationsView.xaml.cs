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

using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    /// Displays the users notifications
    /// </summary>
    public sealed partial class NotificationsView
    {
        // The view model
        public ViewModels.NotificationsViewModel ViewModel = new ViewModels.NotificationsViewModel();

        /// <summary>
        /// Load the page and init the xaml
        /// </summary>
        public NotificationsView()
        {
            // Setup the XAML
            InitializeComponent();
            // This page must be cached
            NavigationCacheMode = NavigationCacheMode.Enabled;
            // Set the data context
            DataContext = ViewModel;
            
            // Page has been unloaded from UI
          //  Unloaded += (s, e) =>
          //  {
           //     ViewModel.Dispose();
          //      ViewModel = null;
       //     };

            // Create the view model on load
       //     Loaded += (s, e) => ViewModel = new ViewModels.NotificationsViewModel();
        }

        /// <summary>
        /// Called when the user navigates to the page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the last frame
            SettingsService.Current.LastFrame = typeof(NotificationsView).FullName;
            // Track event
            TelemetryService.Current.TrackPage("Notifications Page");
        }
    }
}
