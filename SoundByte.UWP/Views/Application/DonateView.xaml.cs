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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Services;

namespace SoundByte.UWP.Views.Application
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DonateView
    {
        public DonateView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Instance.TrackPage("Donate View");

            // We are loading
            App.IsLoading = true;

            // Get all the products
            await MonitizeService.Instance.InitProductInfoAsync();

            if (MonitizeService.Instance.Products.Count > 0)
            {
                LooseChangePrice.Text = MonitizeService.Instance.Products.Exists(t => t.Key.ToLower() == "9p3vls5wtft6")
                    ? MonitizeService.Instance.Products.Find(t => t.Key.ToLower() == "9p3vls5wtft6").Value.Price.FormattedBasePrice
                    : "Unknown";
                SmallCoffeePrice.Text = MonitizeService.Instance.Products.Exists(t => t.Key.ToLower() == "9msxrvnlnlj7")
                    ? MonitizeService.Instance.Products.Find(t => t.Key.ToLower() == "9msxrvnlnlj7").Value.Price.FormattedBasePrice
                    : "Unknown";
                RegularCoffeePrice.Text = MonitizeService.Instance.Products.Exists(t => t.Key.ToLower() == "9nrgs6r2grsz")
                    ? MonitizeService.Instance.Products.Find(t => t.Key.ToLower() == "9nrgs6r2grsz").Value.Price.FormattedBasePrice
                    : "Unknown";
                LargeCoffeePrice.Text = MonitizeService.Instance.Products.Exists(t => t.Key.ToLower() == "9pnsd6hskwpk")
                    ? MonitizeService.Instance.Products.Find(t => t.Key.ToLower() == "9pnsd6hskwpk").Value.Price.FormattedBasePrice
                    : "Unknown";
            }
            else
            {
                await new MessageDialog("Could not load donation options...").ShowAsync();
            }
          
            // We are not loading now
            App.IsLoading = false;
        }

        private async void DonateLooseChange(object sender, RoutedEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            await MonitizeService.Instance.PurchaseDonation("9p3vls5wtft6");

            // We are not loading
            App.IsLoading = false;
        }

        private async void DonateSmall(object sender, RoutedEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            await MonitizeService.Instance.PurchaseDonation("9msxrvnlnlj7");

            // We are not loading
            App.IsLoading = false;
        }

        private async void DonateRegular(object sender, RoutedEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            await MonitizeService.Instance.PurchaseDonation("9nrgs6r2grsz");

            // We are not loading
            App.IsLoading = false;
        }

        private async void DonateLarge(object sender, RoutedEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            await MonitizeService.Instance.PurchaseDonation("9pnsd6hskwpk");

            // We are not loading
            App.IsLoading = false;
        }
    }
}