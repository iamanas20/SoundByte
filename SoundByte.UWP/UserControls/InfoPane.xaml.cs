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

        private static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(InfoPane), null);

        private static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(InfoPane), null);

        private static readonly DependencyProperty IsErrorProperty =
            DependencyProperty.Register("IsError", typeof(bool), typeof(InfoPane), null);
        #endregion

        #region Getters and Setters

        /// <summary>
        ///     The title to show on the error control
        /// </summary>
        public string Header
        {
            get => GetValue(HeaderProperty) as string;
            set
            {
                SetValue(HeaderProperty, value);
                HeaderTextBlock.Text = value;
            }
        }

        /// <summary>
        ///     The text to show on the error control
        /// </summary>
        public string Text
        {
            get => GetValue(TextProperty) as string;
            set
            {
                SetValue(TextProperty, value);
                TextTextBlock.Text = value;
            }
        }

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set
            {
                SetValue(IsLoadingProperty, value);

                if (value)
                {
                    LoadingRing.Visibility = Visibility.Visible;
                    Visibility = Visibility.Visible;
                }
                else
                {
                    LoadingRing.Visibility = Visibility.Collapsed;

                    if (!IsError)
                        Visibility = Visibility.Collapsed;
                }
            }
        }

        public bool IsError
        {
            get => (bool)GetValue(IsErrorProperty);
            set
            {
                SetValue(IsErrorProperty, value);

                if (value)
                {
                    Visibility = Visibility.Visible;
                    LoadingRing.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (!IsLoading)
                        Visibility = Visibility.Collapsed;
                }
            }
        }



        #endregion

        #region Methods

        [Obsolete]
        public void ShowLoading()
        {
            IsLoading = true;
        }


        [Obsolete]
        public void ShowMessage(string header, string text, bool showButton = true)
        {
            IsError = true;
            // Update the needed variables
            Header = header;
            Text = text;

            // Logic to show or hide the buton
          //  CloseButton.Visibility = showButton ? Visibility.Visible : Visibility.Collapsed;
        }

        public void ClosePaneButtonClick()
        {
            IsError = false;
        }

        /// <summary>
        ///     Closes the pane
        /// </summary>
        [Obsolete]
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