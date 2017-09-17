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

using SoundByte.UWP.Views.Search;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class SearchDialog
    {
        public SearchDialog()
        {
            InitializeComponent();
        }

        public void Search()
        {
            // Navigate to search page
            App.NavigateTo(typeof(SearchView), SearchBox.Text);
           
            // Hide the popup
            Hide();
        }

        private void QuerySubmitted(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
        { 
            // Navigate to search page
            App.NavigateTo(typeof(SearchView), SearchBox.Text);

            // Hide the popup
            Hide();
        }
    }
}