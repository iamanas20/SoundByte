using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Controls
{
    public sealed partial class NowPlayingBar
    {
        public PlaybackViewModel PlaybackViewModel { get; private set; }

        public NowPlayingBar()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                PlaybackViewModel = new PlaybackViewModel();
            };

            Unloaded += (sender, args) =>
            {
                PlaybackViewModel?.Dispose();
                PlaybackViewModel = null;
            };
        }

        public async void ShowCompactView()
        {
            await App.SwitchToCompactView();
        }
    }
}