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
    public partial class SpecrumAnalysisControl : UserControl
    {
        public SpecrumAnalysisControl()
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
                if (Data != null)
                    Data.Dispose();
                waveStream.Dispose();
            }
            if (sourceWaveStream != null)
            {
                waveStream = sourceWaveStream;
                if (waveStream.WaveFormat != null)
                {
                    Data = new SpectrumData(waveStream,
                        (long)(waveStream.Length / sizeof(float)),
                        1024,
                        1024);
                    LoadPlotData();
                }
            }
        }

        private Plot FreqLevelPlot => spectrumPlots.Multiplot.GetPlot(0);
        private Plot FftPlot => spectrumPlots.Multiplot.GetPlot(1);
        private Plot PowerPlot => spectrumPlots.Multiplot.GetPlot(2);

        private BarPlot FreqLevelBars;
        private BarPlot TimeLevelBars;
        private Heatmap HMap = new Heatmap(new double[0, 0]);
        double[,] HeatmapData = new double[0, 0];
        private int dataHeight;
        private int dataWidth;
        private int loadedSamples;
        private double barWidth = 0.3D;
        private WaveStream? waveStream;
        private AudioMultiStream audioMultiStream;

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private SpectrumData? Data;

        private int desiredLatency = 200;

        private int bufferSize => Data == null ? 0 : Data.WaveFormat.ConvertLatencyToByteSize(desiredLatency);

        private int totalTime => Data == null ? 0 : Data.WaveFormat.AverageBytesPerSecond * desiredLatency / 1000;

        private void ReadSampleSource()
        {
            if (Data != null)
            {
                int readCount = bufferSize;
                byte[] buffer = new byte[bufferSize * sizeof(float)];
                while (readCount >= bufferSize)
                {
                    readCount = Data.Read(buffer, 0, bufferSize * sizeof(float));
                    if (readCount > 0)
                    {
                        float[] floats = new float[buffer.Length / sizeof(float)];
                        Buffer.BlockCopy(buffer, 0, floats, 0, buffer.Length);
                        loadedSamples += readCount;
                    }
                }
                LoadPlotData();
            }
        }

        private void SetupSubgrids()
        {
            spectrumPlots.Multiplot.AddPlots(3);
            FreqLevelBars = new BarPlot(new List<Bar>());
            FreqLevelBars.Horizontal = true;

            TimeLevelBars = new BarPlot(new List<Bar>());
            TimeLevelBars.Horizontal = false;

            // use a fixed layout to ensure all plots remain aligned
            PixelPadding padding = new(20, 20, 20, 10);
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
            if (Data == null || Data.Length == 0)
            {
                LoadDummyData();
                return;
            }

            Data.ReadSource();
            // load the frequency levels
            List<Bar> fBars = FreqLevelBars.Bars;
            if (fBars.Count == 0)
            {
                for (int i = 0; i < Data.FrequencyMags.Count() / 2; i++)
                {
                    var bar = new Bar()
                    {
                        Position = i,
                        Size = barWidth,
                        Value = Data.FrequencyMags[i].AvgValue,
                        Error = Data.FrequencyMags[i].Range / 2,
                        Orientation = ScottPlot.Orientation.Horizontal
                    };
                    fBars.Add(bar);
                }
            }
            var fBarPlot = FreqLevelPlot.Add.Bars(fBars);

            // load the frequency levels
            List<Bar> tBars = TimeLevelBars.Bars;
            if (tBars.Count == 0)
            {
                for (int i = 0; i < Data.FftCount; i++)
                {
                    var bar = new Bar()
                    {
                        Position = i,
                        Size = barWidth,
                        Value = Data.TimeMags[i].AvgValue,
                        Error = Data.TimeMags[i].Range / 2,
                        Orientation = ScottPlot.Orientation.Vertical
                    };
                    tBars.Add(bar);
                }
            }
            var tBarPlot = PowerPlot.Add.Bars(tBars);

            FreqLevelPlot.Axes.Margins(left: 0);
            FftPlot.Axes.Margins(left: 0, right: 0);
            PowerPlot.Axes.Margins(left: 0, right: 0);

            //spectrumPlots.Multiplot.SharedAxes.ShareX([FftPlot, PowerPlot]);

            HeatmapData = new double[Data.FftWindowSize / 2, Data.FftColumns.GetLength(0)];
            for (int x = 0; x < Data.FftColumns.GetLength(0); x++)
            {
                for (int y = 0; y < Data.FftWindowSize / 2; y++)
                {
                    HeatmapData[(Data.FftWindowSize / 2) - 1 - y, x] = Data.FftColumns[x, y].Magnitude;
                }
            }

            FftPlot.PlottableList.Remove(HMap);
            HMap = new ScottPlot.Plottables.Heatmap(HeatmapData);
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
