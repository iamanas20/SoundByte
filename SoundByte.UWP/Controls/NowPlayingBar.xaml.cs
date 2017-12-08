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
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using AudioVisualizer;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.Controls
{
    public sealed partial class NowPlayingBar
    {
        private ArrayData _emptySpectrum = new ArrayData(2, 20);
        private ArrayData _previousSpectrum;
        private ArrayData _previousPeakSpectrum;

        private TimeSpan _rmsRiseTime = TimeSpan.FromMilliseconds(50);
        private TimeSpan _rmsFallTime = TimeSpan.FromMilliseconds(50);
        private TimeSpan _peakRiseTime = TimeSpan.FromMilliseconds(100);
        private TimeSpan _peakFallTime = TimeSpan.FromMilliseconds(1000);
        private TimeSpan _frameDuration = TimeSpan.FromMilliseconds(16.7);

        private object _sizeLock = new object();
        private float _visualizerWidth = 0.0f;
        private float _visualizerHeight = 0.0f;

        AudioVisualizer.PlaybackSource _source;

        private void NowPlayingBar_OnLoaded(object sender, RoutedEventArgs e)
        {




        }

        public NowPlayingBar()
        {
            InitializeComponent();

           
        }


        public PlaybackService Service => PlaybackService.Instance;

        private void CreateVisualizer()
        { 
            if (PlaybackService.Instance.VisualizationSource != null)
            {
                PlaybackService.Instance.VisualizationSourceChanged += Current_VisualizationSourceChanged;

                if (PlaybackService.Instance.VisualizationSource.Source != null)
                {
                    SetSource();
                }
            }
        }

        private void Current_VisualizationSourceChanged(object sender, IVisualizationSource e)
        {
            SetSource();
        }

        private void SetSource()
        {
            Visualizer.Source = PlaybackService.Instance.VisualizationSource.Source;
            PlaybackService.Instance.VisualizationSource.Source.IsSuspended = false;
        }

        private void NavigateTrack()
        {
            App.NavigateTo(typeof(NowPlayingView));
        }

        public async void ShowCompactView()
        {
            await App.SwitchToCompactView();
        }

        private void Visualizer_OnDraw(IVisualizer sender, VisualizerDrawEventArgs args)
        {
            var drawingSession = (CanvasDrawingSession)args.DrawingSession;

            var spectrum = args.Data != null ? args.Data.Spectrum.TransformLinearFrequency(20) : _emptySpectrum;

            _previousSpectrum = spectrum.ApplyRiseAndFall(_previousSpectrum, _rmsRiseTime, _rmsFallTime, _frameDuration);
            _previousPeakSpectrum = spectrum.ApplyRiseAndFall(_previousPeakSpectrum, _peakRiseTime, _peakFallTime, _frameDuration);

            // This is temp hack. once visualizer passes in dimensions won't be necessary
            float w = 0f, h = 0f;

            lock (_sizeLock)
            {
                w = _visualizerWidth;
                h = _visualizerHeight;
            }

            // There are bugs in ConverToLogAmplitude. It is returning 0 if max is not 0 and min negative.
            // The heightScale is a workaround for this
            var s = _previousSpectrum.ConvertToLogAmplitude(-50, 0);
            var p = _previousPeakSpectrum.ConvertToLogAmplitude(-50, 0);
            DrawSpectrumSpline(p[0], drawingSession, Vector2.Zero, w, h, -0.02f, Color.FromArgb(0xff, 0x38, 0x38, 0x38));
            DrawSpectrumSpline(p[1], drawingSession, Vector2.Zero, w, h, -0.02f, Color.FromArgb(0xff, 0x38, 0x38, 0x38), true);
            DrawSpectrumSpline(s[0], drawingSession, Vector2.Zero, w, h, -0.02f, Color.FromArgb(0xff, 0x30, 0x30, 0x30));
            DrawSpectrumSpline(s[1], drawingSession, Vector2.Zero, w, h, -0.02f, Color.FromArgb(0xff, 0x30, 0x30, 0x30), true);
        }

        private void DrawSpectrumSpline(IReadOnlyList<float> data, CanvasDrawingSession session, Vector2 offset,
            float width, float height, float heightScale, Color color, bool rightToLeft = false)
        {
            int segmentCount = data.Count - 1;
            if (segmentCount <= 1 || width <= 0f)
            {
                return;
            }

            CanvasPathBuilder path = new CanvasPathBuilder(session);

            float segmentWidth = width / (float)segmentCount;

            Vector2 prevPosition = rightToLeft ? new Vector2(width + offset.X, data[0] * heightScale * height + offset.Y)
                : new Vector2(offset.X, data[0] * heightScale * height + offset.Y);


            if (rightToLeft)
            {
                path.BeginFigure(width + offset.X, height + offset.Y);
            }
            else
            {
                path.BeginFigure(offset.X, height + offset.Y);
            }

            path.AddLine(prevPosition);

            for (int i = 1; i < data.Count; i++)
            {
                Vector2 position = rightToLeft ? new Vector2(width - (float)i * segmentWidth + offset.X, data[i] * heightScale * height + offset.Y)
                    : new Vector2((float)i * segmentWidth + offset.X, data[i] * heightScale * height + offset.Y);

                if (rightToLeft)
                {
                    Vector2 c1 = new Vector2(position.X + segmentWidth / 2.0f, prevPosition.Y);
                    Vector2 c2 = new Vector2(prevPosition.X - segmentWidth / 2.0f, position.Y);
                    path.AddCubicBezier(c1, c2, position);
                }
                else
                {
                    Vector2 c1 = new Vector2(position.X - segmentWidth / 2.0f, prevPosition.Y);
                    Vector2 c2 = new Vector2(prevPosition.X + segmentWidth / 2.0f, position.Y);
                    path.AddCubicBezier(c1, c2, position);
                }

                prevPosition = position;
            }

            if (rightToLeft)
            {
                path.AddLine(offset.X, height + offset.Y);
            }
            else
            {
                path.AddLine(width + offset.X, height + offset.Y);
            }

            path.EndFigure(CanvasFigureLoop.Closed);

            CanvasGeometry geometry = CanvasGeometry.CreatePath(path);
            session.FillGeometry(geometry, color);
        }

        private void Visualizer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (double.IsNaN(Visualizer.ActualWidth) || double.IsNaN(Visualizer.ActualHeight))
                return;

            lock (_sizeLock)
            {
                _visualizerWidth = (float)Visualizer.ActualWidth;
                _visualizerHeight = (float)Visualizer.ActualHeight;
            }
        }

       
    }
}