using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Services.Store;
using Windows.UI.Popups;

namespace SoundByte.UWP.Services
{
    public partial class DonationService
    {
        // Store Context used to access the store.
        private readonly StoreContext _storeContext;

        public readonly List<KeyValuePair<string, StoreProduct>> Products = new List<KeyValuePair<string, StoreProduct>>();

        private DonationService()
        {
            _storeContext = StoreContext.GetDefault();
        }

        public async Task PurchaseDonation(string storeId)
        {
             App.Telemetry.TrackEvent("Donation Attempt",
                new Dictionary<string, string> { { "StoreID", storeId } });

            // Get the item
            var item = Products.FirstOrDefault(x => x.Key.ToLower() == storeId).Value;

            try
            {
                if (item != null)
                {
                    // Request to purchase the item
                    var result = await item.RequestPurchaseAsync();

                    // Check if the purchase was successful
                    if (result.Status == StorePurchaseStatus.Succeeded)
                    {
                        App.Telemetry.TrackEvent("Donation Successful",
                            new Dictionary<string, string> { { "StoreID", storeId } });

                        await new MessageDialog("Thank you for your donation!", "SoundByte").ShowAsync();
                    }
                    else
                    {
                        App.Telemetry.TrackEvent("Donation Failed",
                            new Dictionary<string, string> { { "StoreID", storeId }, { "Reason", result.ExtendedError?.Message } });

                        await new MessageDialog("Your account has not been charged:\n" + result.ExtendedError?.Message,
                            "SoundByte").ShowAsync();
                    }
                }
                else
                {
                    await new MessageDialog("Your account has not been charged:\n" + "Unknown Error",
                        "SoundByte").ShowAsync();
                }
            }
            catch (Exception e)
            {
                await new MessageDialog("Your account has not been charged:\n" + e.Message,
                    "SoundByte").ShowAsync();
            }
        }

        public async Task InitProductInfoAsync()
        {
            if (Products.Count > 0)
                return;

            // Specify the kinds of add-ons to retrieve.
            var filterList = new List<string> { "Durable", "Consumable", "UnmanagedConsumable" };

            // Specify the Store IDs of the products to retrieve.
            var storeIds = new[]
            {
                "9nrgs6r2grsz", // Regular Coffee
                "9p3vls5wtft6", // Loose Change
                "9msxrvnlnlj7", // Small Coffee
                "9pnsd6hskwpk" // Large Coffee
            };

            var results = await _storeContext.GetStoreProductsAsync(filterList, storeIds);
            Products.AddRange(results.Products);
        }
    }

    public partial class DonationService
    {
        private static readonly Lazy<DonationService> InstanceHolder =
            new Lazy<DonationService>(() => new DonationService());

        public static DonationService Current => InstanceHolder.Value;
    }
}
