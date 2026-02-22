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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

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
            SetupRectangleSelector();
        }

        private void SetupRectangleSelector()
        {
            // add events to trigger in response to mouse actions
            spectrumPlots.MouseMove += OnMouseMove;
            spectrumPlots.MouseDown += OnMouseDown;
            spectrumPlots.MouseUp += OnMouseUp;
        }

        public void SetAudioFileSource(WaveStream sourceWaveStream, SpectrumData? sourceData = null)
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
                    if (sourceData != null)
                    {
                        data = sourceData;
                    }
                    else
                    {
                        data = new SpectrumData(waveStream, 1024, 1024);
                    }
                    LoadPlotData();
                }
            }
        }

        public void UpdateFilePosition()
        {
            if (waveStream != null)
            {
                var time = waveStream.CurrentTime;
                if (time.TotalMilliseconds == 0)
                {
                    FftPlot.PlottableList.Remove(filePos);
                }
                else
                {
                    if (!FftPlot.PlottableList.Contains(filePos))
                    {
                        FftPlot.PlottableList.Add(filePos);
                        filePos.Color = ScottPlot.Colors.Gray;
                        filePos.LineWidth = 4;
                        filePos.IsVisible = true;
                    }
                }
                filePos.X = time.TotalSeconds / Data!.SecondsPerBuffer;
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
        Coordinates MouseDownCoordinates;
        Coordinates MouseNowCoordinates;
        CoordinateRect MouseSelectionRect => new(MouseDownCoordinates, MouseNowCoordinates);
        private bool altMouseIsDown = false;
        private ScottPlot.Plottables.Rectangle? RectanglePlot;

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
            int freqBarCount = data.FrequencyMags.Count();
            if (!data.FftLoaded)
                freqBarCount /= 2;
            for (int i = 0; i < freqBarCount; i++)
            {
                if (data.FrequencyMags[i] != null)
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
            }
            var fBarPlot = FreqLevelPlot.Add.Bars(fBars);

            // load the frequency levels
            List<Bar> tBars = TimeLevelBars.Bars;
            tBars.Clear();
            for (int i = 0; i < data.TimeMags.Length; i++)
            {
                if (data.TimeMags[i] != null)
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
            }
            var tBarPlot = PowerPlot.Add.Bars(tBars);

            FreqLevelPlot.Axes.Margins(left: 0);
            FftPlot.Axes.Margins(left: 0, right: 0);
            PowerPlot.Axes.Margins(left: 0, right: 0);

            //string HzTickLabelFormatter(double y) => $"{(2 * y * SpectrumData.HertzFactor):N0}";
            string HzTickLabelFormatter(double y) => $"{(2 * y * Data!.HertzFactor):N0}";
            string TimeTickLabelFormatter(double y) => $"{(y * Data!.SecondsPerBuffer):00}";

            ScottPlot.TickGenerators.NumericFixedInterval leftTickGen = new ScottPlot.TickGenerators.NumericFixedInterval(50.0F / Data!.HertzFactor);

            ScottPlot.TickGenerators.NumericFixedInterval bottomTickGen = new ScottPlot.TickGenerators.NumericFixedInterval(1.0F / Data!.SecondsPerBuffer);

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

            HeatmapData = new double[data.FftColumns.GetLength(1), data.FftColumns.GetLength(0)];
            double maxMag = 0;
            for (int x = 0; x < data.FftColumns.GetLength(0); x++)
            {
                for (int y = 0; y < data.FftColumns.GetLength(1); y++)
                {
                    double mag = data.FftColumns[x, y].Magnitude;
                    HeatmapData[data.FftColumns.GetLength(1) - 1 - y, x] = mag;
                    if (mag > maxMag) maxMag = mag;
                }
            }

            FftPlot.PlottableList.Remove(HMap);
            HMap = FftPlot.Add.Heatmap(HeatmapData);
            HMap.Smooth = !Data.FftLoaded;
            HMap.ManualRange = new ScottPlot.Range(0, maxMag);
            HMap.Colormap = new ScottPlot.Colormaps.Turbo();
            //RenderPack rp = new RenderPack();
            //HMap.Render();

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


        private void AddRectangle()
        {
            ClearRectangle();
            RectanglePlot = FftPlot.Add.Rectangle(0, 0, 0, 0);
            RectanglePlot.IsVisible = false;
            RectanglePlot.FillStyle.Color = Colors.Red.WithAlpha(.1);
            RectanglePlot.LinePattern = LinePattern.Dashed;
            RectanglePlot.LineColor = Colors.Red;
            RectanglePlot.LineWidth = 2;
        }

        private void ClearRectangle()
        {
            if (RectanglePlot != null)
                RectanglePlot.IsVisible = false;
            RectanglePlot = null;
            lblSelection.Visible = false;
            tbxSelection.Visible = false;
            btnAnalyze.Visible = false;
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            altMouseIsDown = !data!.FftLoaded && spectrumPlots.UserInputProcessor.KeyState.IsPressed(ScottPlot.Interactivity.StandardKeys.Alt);
            if (altMouseIsDown)
            {
                AddRectangle();
                RectanglePlot?.IsVisible = true;
                MouseDownCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
                MouseNowCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
                RectanglePlot?.CoordinateRect = MouseSelectionRect;
                lblSelection.Visible = true;
                tbxSelection.Visible = true;
                btnAnalyze.Visible = true;
                SetSelectionText();
                spectrumPlots.UserInputProcessor.Disable(); // disable the default click-drag-pan behavior
                spectrumPlots.Refresh();
            }
            else
            {
                ClearRectangle();
                spectrumPlots.Refresh();
            }
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (altMouseIsDown)
            {
                spectrumPlots.UserInputProcessor.Enable(); // re-enable the default click-drag-pan behavior
                RectanglePlot?.LinePattern = LinePattern.Solid;
                altMouseIsDown = false;
            }
            // update the plot
            spectrumPlots.Refresh();
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            MouseNowCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
            tbxMouseTime.Text = (MouseNowCoordinates.X * Data!.SecondsPerBuffer).ToString("0.0");
            tbxMouseHz.Text = (MouseNowCoordinates.Y * Data!.HertzFactor * 2).ToString("0.0");
            if (altMouseIsDown)
            {
                RectanglePlot?.CoordinateRect = MouseSelectionRect;
                RectanglePlot?.LinePattern = LinePattern.Dashed;
                RectanglePlot?.LineColor = Colors.Red;
                SetSelectionText();
                spectrumPlots.Refresh();
            }
        }

        private void SetSelectionText()
        {
            tbxSelection.Text = $"({Math.Round(MouseSelectionRect.Left, 1):N1}, {Math.Round(MouseSelectionRect.Bottom, 1):N1}), ({Math.Round(MouseSelectionRect.Right, 1):N1}, {Math.Round(MouseSelectionRect.Top, 1):N1})";
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (RectanglePlot != null && data != null)
            {
                int leftWindow = (int)  Math.Round(RectanglePlot.CoordinateRect.Left);
                int rightWindow = (int)  Math.Round(RectanglePlot.CoordinateRect.Right);
                if (leftWindow == rightWindow) rightWindow++;
                int bottom = (int) Math.Round(RectanglePlot.CoordinateRect.Bottom);
                int top = (int) Math.Round(RectanglePlot.CoordinateRect.Top);
                if (bottom == top) top++;
                System.Numerics.Complex[,] selectedColumns = new System.Numerics.Complex[rightWindow - leftWindow, top - bottom];
                for (int i = (int) leftWindow; i < (int) rightWindow; i++)
                {
                    for (int j = bottom; j < top; j++)
                    {
                        selectedColumns[i - leftWindow, j - bottom] = data.FftColumns[i, j];
                    }
                }
                int leftSample = (int)(leftWindow * data.FftWindowSize);
                int rightSample = (int)(rightWindow * data.FftWindowSize);
                int sampleCount = rightSample - leftSample;
                byte[] src = new byte[sampleCount * data.WaveFormat.BlockAlign];
                MemoryStream rectStream = new(src);
                RawSourceWaveStream rawSourceWaveStream = new RawSourceWaveStream(rectStream, waveStream!.WaveFormat);

                var spectrumDetail = new SpectrumDetailViewer();
                var detailAnalysis = spectrumDetail.SpectrumAnalysisControl;
                detailAnalysis.SetAudioDataSource(rawSourceWaveStream, selectedColumns, data.BufferSize, data.FftWindowSize);

                spectrumDetail.Left = this.Left + 20;
                spectrumDetail.Top = this.Top + 20;
                spectrumDetail.Height = this.Height;
                spectrumDetail.Width = this.Width;
                spectrumDetail.Text = $"Detailed Analysis ({leftWindow:N2} sec to {rightWindow:N2} sec, {bottom} hz to {top} hz)";
                spectrumDetail.Show();
            }
        }

        private void SetAudioDataSource(RawSourceWaveStream rawSourceWaveStream, System.Numerics.Complex[,] fftSelection, int bufferSize, int windowSize)
        {
            data = new SpectrumData(rawSourceWaveStream, fftSelection, bufferSize, windowSize);
            SetAudioFileSource(rawSourceWaveStream, data);
        }
    }

}
