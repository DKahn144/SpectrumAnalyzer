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

            SetupSubgrids();
            LoadDummyData();
        }

        private Plot FreqLevelPlot;
        private Plot FftPlot;
        private Plot PowerPlot;
        private BarPlot FreqLevelBars;
        private Heatmap HMap = new Heatmap(new double[0,0]);
        double[,] HeatmapData;
        private int dataHeight;
        private int dataWidth;
        private int loadedSamples;
        double barWidth = 0.01D;

        private List<float[]> dataValues = new List<float[]>();

        private List<Complex[]> fftValues = new List<Complex[]>();

        private ISampleProvider sampleProvider;

        private int desiredLatency = 200;
        private int bufferSize => sampleProvider.WaveFormat.ConvertLatencyToByteSize(desiredLatency);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISampleProvider SampleProvider
        {
            get { return sampleProvider; }
            set { setSampleProvider(value); }
        }

        private void setSampleProvider(ISampleProvider value)
        {
            if (sampleProvider != value)
            {
                dataValues.Clear();
                fftValues.Clear();
                sampleProvider = value;
                ReadSampleSource();
            }
        }

        private void ReadSampleSource()
        {
            if (sampleProvider != null)
            {
                int readCount = bufferSize;
                float[] buffer = new float[bufferSize];
                List<double[]> bufferList = new List<double[]>();
                while (readCount >= bufferSize)
                {
                    readCount = sampleProvider.Read(buffer, 0, bufferSize);
                    if (readCount > 0)
                    {
                        bufferList.Add(buffer.Select(f => (double)f).ToArray<double>());
                        loadedSamples += readCount;
                    }
                }
                LoadPlotData(bufferList);
            }
        }

        private void SetupSubgrids()
        {
            spectrumPlots.Multiplot.AddPlots(3);
            FreqLevelPlot = spectrumPlots.Multiplot.GetPlot(0);
            FftPlot = spectrumPlots.Multiplot.GetPlot(1);
            PowerPlot = spectrumPlots.Multiplot.GetPlot(2);
            FreqLevelBars = new BarPlot(new List<Bar>());
            FreqLevelBars.Horizontal = true;

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


        public void LoadPlotData(List<double[]> data)
        {
            // clear any dummy data
            for (int i = 0; i < spectrumPlots.Multiplot.Subplots.Count; i++)
            {
                spectrumPlots.Multiplot.GetPlot(i).Clear();
            }
            if (data.Count == 0)
            {
                LoadDummyData();
                return;
            }

            List<Bar> bars = FreqLevelBars.Bars;
            if (bars.Count == 0)
            {
                for (int i = 0; i < data[0].Length; i++)
                    bars.Add(new Bar());
            }
            // load the frequency levels


            double maxBarLength = 0;
            for (int i = 0; i < data[0].Length; i++)
            {
                IEnumerable<double> idoubles = data.Select(dd => dd[i]);
                double avg = idoubles.Average();
                double err = idoubles.Where(d => d < avg).Average();
                bars[i] = new ScottPlot.Bar()
                {
                    Position = i * barWidth,
                    Value = avg,
                    Error = avg - (err / 3.0),
                    Size = barWidth
                };
                if (avg > maxBarLength)
                    maxBarLength = avg;
            }

            HeatmapData = new double[data[0].Length, data.Count];
            for (int x = 0; x < data.Count; x++)
            {
                for (int y = 0; y < data[0].Length; y++)
                {
                    HeatmapData[y, x] = data[x][y];
                }
            }
            FftPlot.PlottableList.Remove(HMap);
            HMap = new ScottPlot.Plottables.Heatmap(HeatmapData);
            HMap.Colormap = new ScottPlot.Colormaps.Turbo();
            FftPlot.PlottableList.Add(HMap);


            var barPlot = FreqLevelPlot.Add.Bars(bars);

            FreqLevelPlot.Axes.Margins(left: 0);
            FftPlot.Axes.Margins(left: 0, right: 0);
            PowerPlot.Axes.Margins(left: 0, right: 0);

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

        void AddSample(List<double[]> sampleFft)
        {
            for (int x = loadedSamples; x < (loadedSamples + sampleFft.Count); x++)
            {
                for (int y = 0; y < sampleFft[x].Length; y++)
                {
                    HeatmapData[y, x] = sampleFft[x][y];
                }
                loadedSamples++;
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
