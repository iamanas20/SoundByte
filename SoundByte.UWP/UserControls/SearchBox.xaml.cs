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

using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.UserControls
{
    public sealed partial class SearchBox
    {
        public SearchBox()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The text currently stored in the
        ///     search box
        /// </summary>
        public string Text
        {
            get => AutoSearchBox.Text;
            set => AutoSearchBox.Text = value;
        }

        /// <summary>
        ///     Event handler for binding to the submitted
        ///     event.
        /// </summary>
        public event RoutedEventHandler SearchSubmitted;

        /// <summary>
        ///     Called when the query is submitted from the text box
        /// </summary>
        private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            SearchBoxSubmitted();
        }

        /// <summary>
        ///     Called when the text changes on the text box
        ///     This method detects the enter key and then
        ///     performs a search.
        /// </summary>
        private void TextAdded(object sender, KeyRoutedEventArgs e)
        {
            // Check if enter key
            if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.GamepadMenu)
                SearchBoxSubmitted();
        }

        private void SearchBoxSubmitted()
        {
            // Create the search arguments
            var args = new SearchEventArgs {Keyword = AutoSearchBox.Text};
            // Call the event handler
            SearchSubmitted?.Invoke(this, args);
        }

        private void NavigateSearch(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(SearchView));
        }

        /// <summary>
        ///     Holds the keyword that came from
        ///     the search box
        /// </summary>
        public class SearchEventArgs : RoutedEventArgs
        {
            /// <summary>
            ///     The Keyword that the user
            ///     search for in the searched
            ///     box
            /// </summary>
            public string Keyword { get; set; }
        }
    }
}