using System;
using Windows.UI.Xaml.Controls;
using AudioVisualizer;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;
using SoundByte.UWP.Views;

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

            //    if (PlaybackService.Instance.VisualizationSource != null)
            //    {
             //       PlaybackService.Instance.VisualizationSource.SourceChanged += VisualizationSourceOnSourceChanged;
              //      Visualizer.Source = PlaybackService.Instance.VisualizationSource.Source;
              //  }
            };

            Unloaded += (sender, args) =>
            {
                PlaybackViewModel?.Dispose();
                PlaybackViewModel = null;
            };
        }

        private void VisualizationSourceOnSourceChanged(object o, IVisualizationSource args)
        {

         //   ((ISpectralAnalyzerSettings)args).ConfigureSpectrum(4096, 0.5f);
            Visualizer.Source = args;
        }


        private void NavigateTrack()
        {
            App.NavigateTo(typeof(NowPlayingView));
        }

        public async void ShowCompactView()
        {
            await App.SwitchToCompactView();
        }

        private void ChangeVisualizatorStyle(object sender, SelectionChangedEventArgs e)
        {
            ChangeVisualizator(vistyle.SelectedIndex);
        }

        private void ChangeVisualizator(int t)
        {
            switch (t)
            {
                case 0: Visualizer.VisualizationStyle = SpectrumVisualizationStyle.Bars; break;
                case 1: Visualizer.VisualizationStyle = SpectrumVisualizationStyle.TopBar; break;
                case 2: Visualizer.VisualizationStyle = SpectrumVisualizationStyle.Blocks; break;
                case 3: Visualizer.VisualizationStyle = SpectrumVisualizationStyle.TopBlock; break;
                case 4: Visualizer.VisualizationStyle = SpectrumVisualizationStyle.Line; break;
            }
        }
    }
}