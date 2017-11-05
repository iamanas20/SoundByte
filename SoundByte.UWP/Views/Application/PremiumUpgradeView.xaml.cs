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
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views.Application
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PremiumUpgradeView
    {
        public PremiumUpgradeView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Instance.TrackPage("Premium View");

            PremuimStatus.Text = "Status: Loading...";
            PremuimStatus.Foreground = new SolidColorBrush(Colors.Orange);

            // We are loading
            await App.SetLoadingAsync(true);

            // Get all the products
            await MonitizeService.Instance.InitProductInfoAsync();

            try
            {
                if (await MonitizeService.Instance.IsPremium())
                {
                    PremuimStatus.Text = "Status: Premium Enabled";
                    PremuimStatus.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    PremuimStatus.Text = "Status: Premium Disabled";
                    PremuimStatus.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
            catch
            {
                PremuimStatus.Text = "Status: Premium Unknown";
                PremuimStatus.Foreground = new SolidColorBrush(Colors.OrangeRed);
            }




            if (MonitizeService.Instance.Products.Count > 0)
            {
               // LooseChangePrice.Text = MonitizeService.Instance.Products.Exists(t => t.Key.ToLower() == "9p3vls5wtft6")
             //       ? MonitizeService.Instance.Products.Find(t => t.Key.ToLower() == "9p3vls5wtft6").Value.Price.FormattedBasePrice
              //      : "Unknown";
             //   SmallCoffeePrice.Text = MonitizeService.Instance.Products.Exists(t => t.Key.ToLower() == "9msxrvnlnlj7")
              //      ? MonitizeService.Instance.Products.Find(t => t.Key.ToLower() == "9msxrvnlnlj7").Value.Price.FormattedBasePrice
              //      : "Unknown";
              //  RegularCoffeePrice.Text = MonitizeService.Instance.Products.Exists(t => t.Key.ToLower() == "9nrgs6r2grsz")
              //      ? MonitizeService.Instance.Products.Find(t => t.Key.ToLower() == "9nrgs6r2grsz").Value.Price.FormattedBasePrice
              //      : "Unknown";
              //  LargeCoffeePrice.Text = MonitizeService.Instance.Products.Exists(t => t.Key.ToLower() == "9pnsd6hskwpk")
             //       ? MonitizeService.Instance.Products.Find(t => t.Key.ToLower() == "9pnsd6hskwpk").Value.Price.FormattedBasePrice
               //     : "Unknown";
            }
            else
            {
                await new MessageDialog("Could not load donation options...").ShowAsync();
            }
          
            // We are not loading now
            await App.SetLoadingAsync(false);
        }
    }
}