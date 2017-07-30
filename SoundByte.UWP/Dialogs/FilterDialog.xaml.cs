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
using Windows.UI.Xaml.Controls;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class FilterDialog
    {
        /// <summary>
        /// Event handler for binding to the submitted
        /// event.
        /// </summary>
        public event RoutedEventHandler FilterApplied;

        /// <summary>
        /// Holds the filter string
        /// </summary>
        public class FilterAppliedArgs : RoutedEventArgs
        {
            /// <summary>
            /// A string list of the filter
            /// </summary>
            public string FilterArgs { get; set; }
        }

        public FilterDialog()
        {
            InitializeComponent();
        }

        public async void Apply()
        {
            var filterArgs = string.Empty;

            // Check that they user has entred something in tags list
            if (!string.IsNullOrEmpty(SearchTags.Text))
            {
                try
                {
                    var tags = SearchTags.Text.Split(',');
                    filterArgs += "&tags=" + String.Join(",", tags);
                }
                catch (Exception)
                {
                    await new MessageDialog("Error: You must seperate each tag with a comma").ShowAsync();
                    return;
                }
            }

            // Get the base content
            var selectedLicense = ((ComboBoxItem)SearchLicense.SelectedItem)?.Content?.ToString();

            if (!string.IsNullOrEmpty(selectedLicense) && selectedLicense.ToLower() != "any")
            {
                selectedLicense = selectedLicense.ToLower().Replace(' ', '-');
                filterArgs += "&license=" + System.Net.WebUtility.UrlEncode(selectedLicense);
            }


            var selectedType = ((ComboBoxItem)SearchType?.SelectedItem)?.Content?.ToString();

            if (!string.IsNullOrEmpty(selectedType) && selectedType.ToLower() != "any")
            {
                selectedType = selectedType.ToLower();
                filterArgs += "&types=" + System.Net.WebUtility.UrlEncode(selectedType);
            }

            if (!string.IsNullOrEmpty(SearchBpm.Text))
            {
                if (int.Parse(SearchBpm.Text) >= 10)
                {
                    filterArgs += "bpm[from]=" + (int.Parse(SearchBpm.Text) - 10) + "&bpm[to]=" + (int.Parse(SearchBpm.Text) + 10);
                }
                else
                {
                    filterArgs += "bpm[from]=0&bpm[to]=" + (int.Parse(SearchBpm.Text) + 10);
                }
            }

            // Create the arguments
            FilterAppliedArgs args = new FilterAppliedArgs { FilterArgs = filterArgs };
            // Call the event handler
            FilterApplied?.Invoke(this, args);
            // Hide the popup
            Hide();
        }
    }
}
