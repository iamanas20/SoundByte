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
using Windows.Services.Store;
using Windows.UI.Popups;

namespace SoundByte.Core.Services
{
    public class MonitizeService
    {
        // Private class instance
        private static readonly Lazy<MonitizeService> InstanceHolder =
            new Lazy<MonitizeService>(() => new MonitizeService());

        /// <summary>
        ///     Sets up the App Monetize class, when the app is run in debug mode,
        ///     it used the Store Proxy xml file. When built in release mode it
        ///     uses the stores license information file.
        /// </summary>
        public static MonitizeService Instance => InstanceHolder.Value;

        // Store Context used to access the store.
        private readonly StoreContext _storeContext;

        public readonly List<KeyValuePair<string, StoreProduct>> Products = new List<KeyValuePair<string, StoreProduct>>();

        private MonitizeService()
        {
            _storeContext = StoreContext.GetDefault();
        }

        public async Task<bool> PurchaseDonation(string storeId)
        {
            TelemetryService.Instance.TrackEvent("Donation Attempt",
                new Dictionary<string, string> {{"StoreID", storeId}});

            // Get the item
            var item = Products.FirstOrDefault(x => x.Key.ToLower() == storeId).Value;
            // Request to purchase the item
            var result = await item.RequestPurchaseAsync();

            // Check if the purchase was successful
            if (result.Status == StorePurchaseStatus.Succeeded)
            {
                TelemetryService.Instance.TrackEvent("Donation Successful",
                    new Dictionary<string, string> {{"StoreID", storeId}});

                await new MessageDialog("Thank you for your donation!", "SoundByte").ShowAsync();
            }
            else
            {
                TelemetryService.Instance.TrackEvent("Donation Failed",
                    new Dictionary<string, string> {{"StoreID", storeId}, {"Reason", result?.ExtendedError?.Message}});

                await new MessageDialog("Your account has not been charged:\n" + result?.ExtendedError?.Message,
                    "SoundByte").ShowAsync();
            }

            return true;
        }

        public async Task InitProductInfoAsync()
        {
            if (Products.Count > 0)
                return;

            // Specify the kinds of add-ons to retrieve.
            var filterList = new List<string> {"Durable", "Consumable", "UnmanagedConsumable"};

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
}