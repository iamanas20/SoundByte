using System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views
{
    public sealed partial class DonateView
    {
        public DonateView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            App.Telemetry.TrackPage("Donate View");

            await App.SetLoadingAsync(true);

            await DonationService.Current.InitProductInfoAsync();

            if (DonationService.Current.Products.Count > 0)
            {
                LooseChangePrice.Content = DonationService.Current.Products.Exists(t => t.Key.ToLower() == "9p3vls5wtft6")
                    ? "Loose Change (" + DonationService.Current.Products.Find(t => t.Key.ToLower() == "9p3vls5wtft6").Value.Price.FormattedBasePrice + ")"
                    : "Loose Change ($--.--)?";

                SmallCoffeePrice.Content = DonationService.Current.Products.Exists(t => t.Key.ToLower() == "9msxrvnlnlj7")
                    ? "Small Coffee (" + DonationService.Current.Products.Find(t => t.Key.ToLower() == "9msxrvnlnlj7").Value.Price.FormattedBasePrice + ")"
                    : "Small Coffee ($--.--)?";

                RegularCoffeePrice.Content = DonationService.Current.Products.Exists(t => t.Key.ToLower() == "9nrgs6r2grsz")
                    ? "Regular Coffee (" + DonationService.Current.Products.Find(t => t.Key.ToLower() == "9nrgs6r2grsz").Value.Price.FormattedBasePrice + ")"
                    : "Regular Coffee ($--.--)?";

                LargeCoffeePrice.Content = DonationService.Current.Products.Exists(t => t.Key.ToLower() == "9pnsd6hskwpk")
                    ? "Large Coffee (" + DonationService.Current.Products.Find(t => t.Key.ToLower() == "9pnsd6hskwpk").Value.Price.FormattedBasePrice + ")"
                    : "Large Coffee ($--.--)?";
            }
            else
            {
                await new MessageDialog("Could not load donation options...").ShowAsync();
            }

            await App.SetLoadingAsync(false);
        }

        private async void DonateLooseChange(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await App.SetLoadingAsync(true);
            await DonationService.Current.PurchaseDonation("9p3vls5wtft6");
            await App.SetLoadingAsync(false);
        }

        private async void DonateSmall(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await App.SetLoadingAsync(true);
            await DonationService.Current.PurchaseDonation("9msxrvnlnlj7");
            await App.SetLoadingAsync(false);
        }

        private async void DonateRegular(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await App.SetLoadingAsync(true);
            await DonationService.Current.PurchaseDonation("9nrgs6r2grsz");
            await App.SetLoadingAsync(false);
        }

        private async void DonateLarge(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await App.SetLoadingAsync(true);
            await DonationService.Current.PurchaseDonation("9pnsd6hskwpk");
            await App.SetLoadingAsync(false);
        }
    }
}