using Windows.UI.Xaml;

namespace SoundByte.UWP.Controls
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
                    Loading.IsActive = true;
                    Header = "";
                    Text = "Loading...";
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Loading.IsActive = false;

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
                    Loading.IsActive = false;
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
        public void ClosePaneButtonClick()
        {
            IsError = false;
        }
        #endregion
    }
}