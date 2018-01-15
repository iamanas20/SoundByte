namespace SoundByte.UWP.Views.ImportViews
{
    public sealed partial class WelcomeView
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void NavigateAppModeView(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.NavigateTo(typeof(ImportModeView));
        }
    }
}
