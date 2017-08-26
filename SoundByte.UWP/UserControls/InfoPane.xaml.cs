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

using Windows.UI.Xaml;

namespace SoundByte.UWP.UserControls
{
    /// <summary>
    ///     This control is used to show friendly messages
    ///     within the app
    /// </summary>
    public sealed partial class InfoPane
    {
        #region Page Setup

        /// <summary>
        ///     Load the XAML part of the user control
        /// </summary>
        public InfoPane()
        {
            InitializeComponent();
        }

        #endregion

        #region Binding Variables

        private static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InfoPane), null);

        private static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register("Glyph", typeof(string), typeof(InfoPane), null);

        private static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(InfoPane), null);

        #endregion

        #region Getters and Setters

        /// <summary>
        ///     The title to show on the error control
        /// </summary>
        public string Header
        {
            get => GetValue(HeaderProperty) as string;
            private set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        ///     The icon to show on the control
        /// </summary>
        public string Glyph
        {
            get => GetValue(GlyphProperty) as string;
            private set => SetValue(GlyphProperty, value);
        }

        /// <summary>
        ///     The text to show on the error control
        /// </summary>
        public string Text
        {
            get => GetValue(TextProperty) as string;
            private set => SetValue(TextProperty, value);
        }

        #endregion

        #region Methods

        public void ShowLoading()
        {
            ShowMessage("Loading...", "Please Wait", "", false);

            GlyphTextBlock.Visibility = Visibility.Collapsed;
            LoadingRing.Visibility = Visibility.Visible;
        }


        /// <summary>
        ///     Shows a message on the screen
        /// </summary>
        /// <param name="header">The title of the message</param>
        /// <param name="text">The text of the message</param>
        /// <param name="glyph">The picture to show</param>
        /// <param name="showButton">Should we display the close button</param>
        public void ShowMessage(string header, string text, string glyph, bool showButton = true)
        {
            // Update the needed variables
            Header = header;
            Text = text;
            Glyph = glyph;

            // Logic to show or hide the buton
            CloseButton.Visibility = showButton ? Visibility.Visible : Visibility.Collapsed;

            // Hide glyph if not provided
            GlyphTextBlock.Visibility = string.IsNullOrEmpty(glyph) ? Visibility.Collapsed : Visibility.Visible;

            // Hide loading ring
            LoadingRing.Visibility = Visibility.Collapsed;

            // Show the control
            Visibility = Visibility.Visible;
            Opacity = 1;
        }

        /// <summary>
        ///     Closes the pane
        /// </summary>
        public void ClosePane()
        {
            if (LoadingRing.Visibility == Visibility.Visible)
            {
                // Hide the pane
                Visibility = Visibility.Collapsed;
                Opacity = 0;
            }
        }

        #endregion
    }
}