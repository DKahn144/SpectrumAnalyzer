using NAudio.Dsp;
using NAudio.Wave;
using ScottPlot;
using ScottPlot.MultiplotLayouts;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace SpectrumAnalyzer
{
    public partial class SpectrumAnalysisControl : UserControl
    {
        public SpectrumAnalysisControl()
        {
            InitializeComponent();
            //audioMultiStream = new AudioMultiStream();
            SetupSubgrids();
            LoadDummyData();
        }

        public void SetAudioFileSource(WaveStream sourceWaveStream)
        {

            if (waveStream != null && waveStream != sourceWaveStream)
            {
                if (data != null)
                    data.Dispose();
                waveStream.Dispose();
            }
            if (sourceWaveStream != null)
            {
                waveStream = sourceWaveStream;
                if (waveStream.WaveFormat != null)
                {
                    data = new SpectrumData(waveStream,
                        1024,
                        1024);
                    LoadPlotData();
                }
            }
        }

        public void UpdateFilePosition()
        {
            if (waveStream != null)
            {
                filePos.IsVisible = true;
                var time = waveStream.CurrentTime;
                if (!FftPlot.PlottableList.Contains(filePos))
                {
                    FftPlot.PlottableList.Add(filePos);
                    filePos.Color = ScottPlot.Colors.Gray;
                    filePos.LineWidth = 4;
                }
                filePos.X = time.TotalSeconds / SpectrumData.SecondsPerBuffer;
                Refresh();
            }
            else
            {
                filePos.IsVisible = false;
            }

        }

        public SpectrumData? Data => data;

        public WaveStream? WaveStream => waveStream;

        private Plot FreqLevelPlot => spectrumPlots.Multiplot.GetPlot(0);
        private Plot FftPlot => spectrumPlots.Multiplot.GetPlot(1);
        private Plot PowerPlot => spectrumPlots.Multiplot.GetPlot(2);

        private BarPlot FreqLevelBars;
        private BarPlot TimeLevelBars;
        private Heatmap HMap = new Heatmap(new double[0, 0]);
        private double[,] HeatmapData = new double[0, 0];
        private int dataHeight;
        private int dataWidth;
        private int loadedSamples;
        private double barWidth = 0.3D;
        private WaveStream? waveStream;
        private VerticalLine filePos = new VerticalLine();

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private SpectrumData? data;

        private int desiredLatency = 200;

        private int bufferSize => data == null ? 0 : data.WaveFormat.ConvertLatencyToByteSize(desiredLatency);

        private int totalTime => data == null ? 0 : data.WaveFormat.AverageBytesPerSecond * desiredLatency / 1000;

        private void SetupSubgrids()
        {
            spectrumPlots.Multiplot.AddPlots(3);
            FreqLevelBars = new BarPlot(new List<Bar>());
            FreqLevelBars.Horizontal = true;

            TimeLevelBars = new BarPlot(new List<Bar>());
            TimeLevelBars.Horizontal = false;

            // use a fixed layout to ensure all plots remain aligned
            PixelPadding padding = new(46, 10, 40, 10);
            foreach (Plot plot in spectrumPlots.Multiplot.GetPlots())
                plot.Layout.Fixed(padding);

            MyCustomGrid customGrid = new();
            customGrid.Set(FreqLevelPlot, new MyGridCell(0, 0, 6, 8, 5, 1));
            customGrid.Set(FftPlot, new MyGridCell(0, 1, 6, 8, 5, 7));
            customGrid.Set(PowerPlot, new MyGridCell(5, 1, 6, 8, 1, 7));


            spectrumPlots.Multiplot.Layout = customGrid;
            spectrumPlots.Multiplot.SharedAxes.ShareX([FftPlot, PowerPlot]);
            spectrumPlots.Multiplot.SharedAxes.ShareY([FreqLevelPlot, FftPlot]);

            FreqLevelPlot.Axes.Left.TickLabelStyle.IsVisible = false;
            FreqLevelPlot.Axes.Left.MajorTickStyle.Length = 0;
            FreqLevelPlot.Axes.Left.MinorTickStyle.Length = 0;
            FreqLevelPlot.Layout.Fixed(new PixelPadding(10, 10, 40, 10));

            spectrumPlots.Refresh();
        }

        public void LoadDummyData()
        {
            // add sample data to each subplot
            for (int i = 0; i < spectrumPlots.Multiplot.Subplots.Count; i++)
            {
                double[] ys = ScottPlot.Generate.Sin(oscillations: i + 1);
                for (int j = 0; j < ys.Length; j++)
                    ys[j] += 1;
                spectrumPlots.Multiplot.GetPlot(i).Add.Signal(ys);
            }
            var bars = new ScottPlot.Bar[]
            {
                new() { Position = 0, Value = 10, Error = 0, Size = barWidth },
                new() { Position = barWidth, Value = 13, Error = 0, Size = barWidth },
                new() { Position = barWidth*2, Value = 16, Error = 2, Size = barWidth },
                new() { Position = barWidth*3, Value = 18, Error = 3, Size = barWidth },
                new() { Position = barWidth*4, Value = 14, Error = 1, Size = barWidth },
                new() { Position = barWidth*5, Value = 10, Error = 4, Size = barWidth },
                new() { Position = barWidth*6, Value = 5, Error = 0, Size = barWidth },
            };
            HeatmapData = ScottPlot.Generate.Sin2D(50, 2, 3, .3, multiple: 48);
            HMap = new ScottPlot.Plottables.Heatmap(HeatmapData);

        }


        public void LoadPlotData()
        {
            // clear any dummy data
            for (int i = 0; i < spectrumPlots.Multiplot.Subplots.Count; i++)
            {
                spectrumPlots.Multiplot.GetPlot(i).Clear();
            }
            if (data == null || data.Length == 0)
            {
                LoadDummyData();
                return;
            }

            data.ReadSource();
            // load the frequency levels
            List<Bar> fBars = FreqLevelBars.Bars;
            fBars.Clear();
            for (int i = 0; i < data.FrequencyMags.Count() / 2; i++)
            {
                var bar = new Bar()
                {
                    Position = i,
                    Size = barWidth,
                    Value = data.FrequencyMags[i].AvgValue,
                    Error = data.FrequencyMags[i].RMSValue,
                    Orientation = ScottPlot.Orientation.Horizontal
                };
                fBars.Add(bar);
            }
            var fBarPlot = FreqLevelPlot.Add.Bars(fBars);

            // load the frequency levels
            List<Bar> tBars = TimeLevelBars.Bars;
            tBars.Clear();
            for (int i = 0; i < data.FftCount; i++)
            {
                var bar = new Bar()
                {
                    Position = i,
                    Size = barWidth,
                    Value = data.TimeMags[i].AvgValue,
                    Error = data.TimeMags[i].RMSValue,
                    Orientation = ScottPlot.Orientation.Vertical
                };
                tBars.Add(bar);
            }
            var tBarPlot = PowerPlot.Add.Bars(tBars);

            FreqLevelPlot.Axes.Margins(left: 0);
            FftPlot.Axes.Margins(left: 0, right: 0);
            PowerPlot.Axes.Margins(left: 0, right: 0);

            //spectrumPlots.Multiplot.SharedAxes.ShareX([FftPlot, PowerPlot]);

            static string HzTickLabelFormatter(double y) => $"{(2 * y * SpectrumData.HertzFactor):N0}";
            static string TimeTickLabelFormatter(double y) => $"{(y * SpectrumData.SecondsPerBuffer):00}";

            ScottPlot.TickGenerators.NumericFixedInterval leftTickGen = new ScottPlot.TickGenerators.NumericFixedInterval(50.0F/SpectrumData.HertzFactor);

            ScottPlot.TickGenerators.NumericFixedInterval bottomTickGen = new ScottPlot.TickGenerators.NumericFixedInterval(1.0F/SpectrumData.SecondsPerBuffer);

            // tell our major tick generator to only show major ticks that are whole integers
           // leftTickGen.TickDensity = 1;
           // leftTickGen.IntegerTicksOnly = true;
            // tell our custom tick generator to use our new label formatter
            leftTickGen.LabelFormatter = HzTickLabelFormatter;
            // tell the left axis to use our custom tick generator
            FftPlot.Axes.Left.TickGenerator = leftTickGen;

            //bottomTickGen.IntegerTicksOnly = false;
            bottomTickGen.LabelFormatter = TimeTickLabelFormatter;
            FftPlot.Axes.Bottom.TickGenerator = bottomTickGen;
            FftPlot.Axes.Bottom.Label.Text = "Seconds";
            FftPlot.Axes.Bottom.Label.FontSize = 11;

            FftPlot.Axes.Left.Label.Text = "Hz";
            FftPlot.Axes.Left.Label.FontSize = 11;
            PowerPlot.Axes.Bottom.Label.Text = "FFT Windows";
            PowerPlot.Axes.Bottom.Label.FontSize = 11;

            HeatmapData = new double[data.FftWindowSize / 2, data.FftColumns.GetLength(0)];
            double maxMag = 0;
            for (int x = 0; x < data.FftColumns.GetLength(0); x++)
            {
                for (int y = 0; y < data.FftWindowSize / 2; y++)
                {
                    double mag = data.FftColumns[x, y].Magnitude;
                    HeatmapData[(data.FftWindowSize / 2) - 1 - y, x] = mag;
                    if (mag > maxMag) maxMag = mag;
                }
            }

            FftPlot.PlottableList.Remove(HMap);
            HMap = new ScottPlot.Plottables.Heatmap(HeatmapData);
            HMap.Smooth = true;
            HMap.ManualRange = new ScottPlot.Range(0, maxMag);
            HMap.Colormap = new ScottPlot.Colormaps.Turbo();
            //RenderPack rp = new RenderPack();
            //HMap.Render();
            FftPlot.PlottableList.Add(HMap);

            spectrumPlots.Refresh();
        }

        public void SetFFTData(List<double[]> data, int totalLength = 0, int height = 0)
        {
            if (data.Count > totalLength)
                totalLength = data.Count;
            if (data.Count > 0 && data[0].Length > height)
            {
                height = data[0].Length;
                dataHeight = height;
                dataWidth = totalLength;
                HeatmapData = new double[totalLength, height];
                for (int x = 0; x < data.Count; x++)
                {
                    for (int y = 0; y < data[0].Length; y++)
                    {
                        HeatmapData[y, x] = data[x][y];
                    }
                }
                loadedSamples = data[0].Length;
            }
            spectrumPlots.Refresh();
        }

        void UpdateFreqLevels(List<double> levels)
        {
            // update frequency level bars
            for (int i = 0; i < levels.Count && i < FreqLevelBars.Bars.Count; i++)
            {
                FreqLevelBars.Bars[i].Value = levels[i];
            }
            spectrumPlots.Refresh();
        }
    }
}
