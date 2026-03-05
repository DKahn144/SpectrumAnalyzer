using FftSharp;
using NAudio.Dsp;
using NAudio.Wave;
using ScottPlot;
using ScottPlot.MultiplotLayouts;
using ScottPlot.Plottables;
using SpectrumProcessor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SpectrumAnalyzer
{
    public partial class SpectrumAnalysisControl : UserControl
    {
        public SpectrumAnalysisControl()
        {
            InitializeComponent();
            if (firstInstance == null)
                firstInstance = this;
            else
                tbxFFTWindowSize.Enabled = false;
            //audioMultiStream = new AudioMultiStream();
            FreqLevelBars = new BarPlot(new List<Bar>());
            TimeLevelBars = new BarPlot(new List<Bar>());
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

        public void SetAudioFileSource(SampleWaveStream sourceWaveStream, SpectrumData? sourceData = null)
        {

            if (waveStream != null && waveStream != sourceWaveStream)
            {
                if (data != null)
                {
                    data.Dispose();
                    data = null;
                }
                waveStream.Dispose();
            }
            if (sourceWaveStream != null)
            {
                waveStream = sourceWaveStream;
                if (waveStream is AudioFileSampleWaveStream)
                {
                    this.fileName = ((AudioFileSampleWaveStream)waveStream).FileName;
                }
                if (waveStream.WaveFormat != null)
                {
                    if (sourceData != null)
                    {
                        data = sourceData;
                    }
                    else
                    {
                        data = new SpectrumData(waveStream, fFTWindowSize, this.fileName);
                        data.ReadSource();
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

        public SampleWaveStream? WaveStream => waveStream;

        private Plot FreqLevelPlot => spectrumPlots.Multiplot.GetPlot(0);
        private Plot FftPlot => spectrumPlots.Multiplot.GetPlot(1);
        private Plot PowerPlot => spectrumPlots.Multiplot.GetPlot(2);
        private Plot ExtraPlot => spectrumPlots.Multiplot.GetPlot(3);

        private BarPlot FreqLevelBars;
        private BarPlot TimeLevelBars;
        private BarPlot timeLineBars = new BarPlot(new List<Bar>());
        private Heatmap HMap = new Heatmap(new double[0, 0]);
        private double[,] HeatmapData = new double[0, 0];
        private int dataHeight;
        private int dataWidth;
        private int loadedSamples;
        private double barWidth = 0.3D;
        private SampleWaveStream? waveStream;
        private VerticalLine filePos = new VerticalLine();

        private List<int> powersOf2 = new List<int> { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2014, 4096 };

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private SpectrumData? data;

        private int desiredLatency = 200;
        Coordinates MouseDownCoordinates;
        Coordinates MouseNowCoordinates;
        CoordinateRect MouseSelectionRect => new(MouseDownCoordinates, MouseNowCoordinates);
        CoordinateRect MouseSelectionRect1 => new(MouseDownCoordinates.X, MouseDownCoordinates.Y, MouseNowCoordinates.X + 1, MouseNowCoordinates.Y + 1);
        private bool altMouseIsDown = false;
        private bool altShiftMouseIsDown = false;
        private int timeLineY = 0;
        private ScottPlot.Plottables.Rectangle? RectanglePlot;
        private ScottPlot.Plottables.Rectangle? TransformTimeLine;

        private int bufferSize => data == null ? 0 : data.WaveFormat.ConvertLatencyToByteSize(desiredLatency);

        private int totalTime => data == null ? 0 : data.WaveFormat.AverageBytesPerSecond * desiredLatency / 1000;

        private int fFTWindowSize => int.Parse(tbxFFTWindowSize.Text);

        private static SpectrumAnalysisControl? firstInstance;
        private bool getSampleTime;
        private string fileName = "";
        private int reduceBy => 2;


        public static int FFTWindowSize => firstInstance != null ? firstInstance.fFTWindowSize : 1024;

        public string FileName => fileName;

        public int TotalTime => totalTime;

        private void SetupSubgrids()
        {
            spectrumPlots.Multiplot.AddPlots(4);
            FreqLevelBars.Horizontal = true;
            TimeLevelBars.Horizontal = false;

            // use a fixed layout to ensure all plots remain aligned
            PixelPadding padding = new(46, 10, 40, 10);
            foreach (Plot plot in spectrumPlots.Multiplot.GetPlots())
                plot.Layout.Fixed(padding);

            MyCustomGrid customGrid = new();
            customGrid.Set(FreqLevelPlot, new MyGridCell(0, 0, 7, 8, 5, 1));
            customGrid.Set(FftPlot, new MyGridCell(0, 1, 7, 8, 5, 7));
            customGrid.Set(PowerPlot, new MyGridCell(5, 1, 7, 8, 1, 7));
            customGrid.Set(ExtraPlot, new MyGridCell(6, 0, 7, 8, 1, 8));


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
            int freqBarCount = data.FrequencyMags.Count() / reduceBy;
            ValueMeasure[] freqVals = data.FrequencyMags.Take(freqBarCount).ToArray();
            //if (!data.FftLoaded)
            //{
            //    freqVals = new ValueMeasure[freqBarCount / 4];
            //    for (int i = 0; i < freqBarCount / 4; i++) freqVals[i] = data.FrequencyMags[i];
            //}
            LoadBarPlot(fBars, freqVals, ScottPlot.Orientation.Horizontal);
            FreqLevelBars = FreqLevelPlot.Add.Bars(fBars);

            // Determine average weighted frequency nds display it
            float avgFrequency = 0;
            float avgVolume = 0;
            (avgFrequency, avgVolume) = data.GetWeightedAvgFreq();
            tbxAvgFreq.Text = Math.Round(avgFrequency, 1).ToString("0.0");
            tbxAvgVol.Text = Math.Round(avgVolume, 1).ToString("0.0");

            // load the timespan levels
            List<Bar> tBars = TimeLevelBars.Bars;
            LoadBarPlot(tBars, data.TimeMags, ScottPlot.Orientation.Vertical);
            TimeLevelBars = PowerPlot.Add.Bars(tBars);

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
            HeatmapData = new double[data.FftColumns.GetLength(1)/ reduceBy, data.FftColumns.GetLength(0)];
            double maxMag = 0;
            for (int x = 0; x < data.FftColumns.GetLength(0); x++)
            {
                for (int y = 0; y < data.FftColumns.GetLength(1)/ reduceBy; y++)
                {
                    double mag = data.FftColumns[x, y].Magnitude;
                    if (mag > 2.0D && y > 450)
                    {

                    }
                    HeatmapData[(data.FftColumns.GetLength(1)/ reduceBy) - 1 - y, x] = mag;
                    if (mag > maxMag) maxMag = mag;
                }
            }

            FftPlot.PlottableList.Remove(HMap);
            HMap = FftPlot.Add.Heatmap(HeatmapData);
            HMap.Smooth = !Data.FftLoaded;
            cbxSmooth.Checked = HMap.Smooth;
            HMap.ManualRange = new ScottPlot.Range(0, maxMag);
            HMap.Colormap = new ScottPlot.Colormaps.Turbo();
            //RenderPack rp = new RenderPack();
            //HMap.Render();

            ExtraPlot.Clear();

            spectrumPlots.Refresh();
        }

        private void LoadBarPlot(List<Bar> bars, ValueMeasure[] values, ScottPlot.Orientation orientation)
        {
            bars.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null)
                {
                    var bar = new Bar()
                    {
                        Position = i,
                        Size = barWidth,
                        Value = values[i].AvgValue,
                        Error = values[i].RMSValue,
                        Orientation = orientation
                    };
                    bars.Add(bar);
                }
            }
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


        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            MouseDownCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
            if (Data != null &&
                spectrumPlots.UserInputProcessor.KeyState.IsPressed(ScottPlot.Interactivity.StandardKeys.Alt))
            {
                altShiftMouseIsDown = spectrumPlots.UserInputProcessor.KeyState.IsPressed(ScottPlot.Interactivity.StandardKeys.Shift);
                altMouseIsDown = !altShiftMouseIsDown;
                if (altMouseIsDown)
                {
                    ClearTransformLine();
                    MouseNowCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
                    AddRectangle();
                }
                if (altShiftMouseIsDown)
                {
                    ClearRectangle();
                    MouseNowCoordinates = FftPlot.GetCoordinates(e.X, e.Y + 1);
                    timeLineY = e.Y;
                    AddTransformLine();
                }
                spectrumPlots.UserInputProcessor.Disable(); // disable the default click-drag-pan behavior
            }
            else
            {
                spectrumPlots.UserInputProcessor.Enable(); // disable the default click-drag-pan behavior
                if (getSampleTime)
                {
                    DisplaySampleTime();
                    getSampleTime = false;
                }
                if (filePos.X > 0)
                {
                    if (waveStream != null && MouseDownCoordinates.X != filePos.X)
                    {
                        // MouseDownCoordinates.X = waveStream.CurrentTime.TotalSeconds / Data.SecondsPerBuffer (buffers so far)
                        // ((AudioFileReader)waveStream).Seek((long)MouseDownCoordinates.X, SeekOrigin.Current);

                        waveStream.Position = (long)((MouseDownCoordinates.X * waveStream.Length) / (waveStream.TotalTime.TotalSeconds / Data!.SecondsPerBuffer));

                        filePos.X = MouseDownCoordinates.X;
                        var time = waveStream.CurrentTime;
                    }
                }
            }
            spectrumPlots.Refresh();
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            MouseNowCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
            if (Data != null)
            {
                tbxMouseTime.Text = (MouseNowCoordinates.X * Data!.SecondsPerBuffer).ToString("0.0");
                tbxMouseHz.Text = (MouseNowCoordinates.Y * Data!.HertzFactor * 2).ToString("0.0");
                if (altMouseIsDown)
                {
                    RectanglePlot?.CoordinateRect = MouseSelectionRect;
                    SetSelectionText();
                    spectrumPlots.Refresh();
                }
                else if (altShiftMouseIsDown)
                {
                    MouseNowCoordinates = FftPlot.GetCoordinates(e.X, timeLineY);
                    var top = TransformTimeLine?.CoordinateRect.Top;
                    TransformTimeLine?.CoordinateRect = MouseSelectionRect;
                    SetSelectionText();
                    spectrumPlots.Refresh();
                }
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
            else if (altShiftMouseIsDown)
            {
                spectrumPlots.UserInputProcessor.Enable(); // re-enable the default click-drag-pan behavior
                TransformTimeLine?.LinePattern = LinePattern.Solid;
                altShiftMouseIsDown = false;
            }
            spectrumPlots.Refresh();
        }

        private void AddRectangle()
        {
            ClearRectangle();
            RectanglePlot = FftPlot.Add.Rectangle(0, 0, 0, 0);
            RectanglePlot.IsVisible = true;
            RectanglePlot.FillStyle.Color = Colors.Red.WithAlpha(.1);
            RectanglePlot.LinePattern = LinePattern.Dashed;
            RectanglePlot.LineColor = Colors.Red;
            RectanglePlot.LineWidth = 2;
            RectanglePlot.CoordinateRect = MouseSelectionRect;
            EnableAnalyzeBtn(System.Drawing.Color.DarkRed);
            SetSelectionText();
            spectrumPlots.UserInputProcessor.Disable(); // disable the default click-drag-pan behavior
        }

        private void ClearRectangle()
        {
            if (RectanglePlot != null)
            {
                FftPlot.PlottableList.Remove(RectanglePlot);
                RectanglePlot.IsVisible = false;
                RectanglePlot = null;
            }
        }

        private void AddTransformLine()
        {
            ClearTransformLine();
            TransformTimeLine = FftPlot.Add.Rectangle(0, 0, 0, 0);
            TransformTimeLine.FillStyle.Color = Colors.Cyan.WithAlpha(.1);
            TransformTimeLine.LinePattern = LinePattern.Dashed;
            TransformTimeLine.LineColor = Colors.Cyan;
            TransformTimeLine.LineWidth = 1;
            TransformTimeLine.CoordinateRect = MouseSelectionRect;
            TransformTimeLine.IsVisible = true;
            EnableAnalyzeBtn(System.Drawing.Color.DarkCyan);
            spectrumPlots.UserInputProcessor.Disable(); // disable the default click-drag-pan behavior
            SetSelectionText();
        }

        private void ClearTransformLine()
        {
            if (TransformTimeLine != null)
            {
                TransformTimeLine.IsVisible = false;
                TransformTimeLine = null;
                altShiftMouseIsDown = false;
                UndoTimeLineBars();
            }
        }

        private void SetSelectionText()
        {
            tbxSelection.Text = $"({Math.Round(MouseSelectionRect.Left, 1):N1}, {Math.Round(MouseSelectionRect.Bottom, 1):N1}), ({Math.Round(MouseSelectionRect.Right, 1):N1}, {Math.Round(MouseSelectionRect.Top, 1):N1})";
        }

        private void EnableAnalyzeBtn(System.Drawing.Color forecolor)
        {
            lblSelection.Visible = true;
            tbxSelection.Visible = true;
            lblSelection.ForeColor = forecolor;
            tbxSelection.ForeColor = forecolor;
            btnAnalyze.ForeColor = forecolor;
            btnClearAnalysis.ForeColor = forecolor;
            btnAnalyze.Visible = true;
            btnClearAnalysis.Visible = true;
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (data == null)
                return;
            if (RectanglePlot != null)
            {
                SampleWaveStream? rectStream = null;
                int newSampleRate = data.WaveFormat.SampleRate;
                byte[]? sampleBuffer = null;
                int leftWindow = (int)Math.Round(RectanglePlot.CoordinateRect.Left);
                if (leftWindow < 0) leftWindow = 0;
                int rightWindow = (int)Math.Round(RectanglePlot.CoordinateRect.Right);
                if (leftWindow == rightWindow) rightWindow++;
                int window = rightWindow - leftWindow;
                int bottom = (int)Math.Round(RectanglePlot.CoordinateRect.Bottom);
                if (bottom < 0) bottom = 0;
                int top = (int)Math.Round(RectanglePlot.CoordinateRect.Top);
                if (bottom == top) top++;
                if (bottom == 0 && top >= data.FftColumns.GetLength(1) / reduceBy)
                {
                    top = -1;
                    // pass a slice of the original sample data to the child window, full height since all frequencies are selected
                    var sampleByteBuffer = data.RawBufferSegment(leftWindow, rightWindow).SelectMany(b => b.ToList());
                    sampleBuffer = sampleByteBuffer.ToArray();
                }
                else
                {
                    int height = data.FftWindowSize / reduceBy;
                    var fft = new FFTData();
                    fft.SetFftColumns(data.FftColumns, leftWindow, rightWindow, bottom, top);
                    sampleBuffer = data.GetReverseFFT(fft.FftColumns);
                    /*
                    float[] selectedSamples = new float[height];
                    sampleBuffer = new byte[window * sizeof(float)];
                    for (int i = leftWindow; i < rightWindow; i++)
                    {
                        System.Numerics.Complex sum = 0;
                        for (int j = 0; j < height; j++)
                        {
                            if (j < bottom || j >= top)
                                selectedColumns[i - leftWindow, j] = 0;
                            else
                            {
                                var value =  data.FftColumns[i, j];
                                selectedColumns[i - leftWindow, j] = value;
                                sum += value;
                            }
                        }
                        selectedSamples[i - leftWindow] = (float) (sum.Real * height) / (top - bottom);

                        byte[] byte4 = BitConverter.GetBytes(selectedSamples[i - leftWindow]);
                        for (int k = 0; k < 4; k++)
                            sampleBuffer[((i - leftWindow) * sizeof(float)) + k] = byte4[k];
                    }
                    */
                }

                WaveFormat newWaveFormat = new WaveFormat(newSampleRate, data.WaveFormat.BitsPerSample, data.WaveFormat.Channels);

                rectStream = new(sampleBuffer, 0, sampleBuffer.Length, newWaveFormat);
                rectStream.FFTData.SetFftColumns(data.FftColumns, leftWindow, rightWindow, bottom, top);



                var spectrumDetail = new SpectrumDetailViewer();
                var detailAnalysis = spectrumDetail.SpectrumAnalysisControl;
                detailAnalysis.SetAudioDataSource(rectStream);

                spectrumDetail.Left = this.Left + 20;
                spectrumDetail.Top = this.Top + 20;
                spectrumDetail.Height = this.Height;
                spectrumDetail.Width = this.Width;
                spectrumDetail.Text = $"Detailed Analysis ({leftWindow:N2} sec to {rightWindow:N2} sec, {bottom} hz to {top} hz)";
                spectrumDetail.Show();
            }
            else if (TransformTimeLine != null)
            {
                int leftWindow = (int)Math.Round(TransformTimeLine.CoordinateRect.Left);
                if (leftWindow < 0) leftWindow = 0;
                int rightWindow = (int)Math.Round(TransformTimeLine.CoordinateRect.Right);
                if (leftWindow == rightWindow) rightWindow++;
                int bottom = (int)Math.Round(TransformTimeLine.CoordinateRect.Bottom);
                if (bottom < 0) bottom = 0;

                var fftSlice = new System.Numerics.Complex[rightWindow - leftWindow];
                for (int i = leftWindow; i < rightWindow; i++) fftSlice[i - leftWindow] = data.FftColumns[i, bottom];

                int window = rightWindow - leftWindow;
                int testVal = GetLargest2Factor(window);
                var fftSlice2 = new System.Numerics.Complex[testVal];
                for (int i = 0; i < testVal; i++) fftSlice2[i] = fftSlice[i];

                // perform an FFT on the selected slice
                fftSlice2 = Data!.PerformFFTInPlace(fftSlice2);
                ValueMeasure[] fftValue = new ValueMeasure[data.FftColumns.Length];
                for (int i = 0; i < data.FftColumns.Length; i++)
                {
                    fftValue[i] = new ValueMeasure();
                    if (i >= leftWindow && i < (leftWindow + fftSlice2.Length/2))
                    {
                        fftValue[i].AddValue(0, (float)fftSlice2[i - leftWindow].Magnitude);
                    }
                }
                List<Bar> bars = new List<Bar>();
                LoadBarPlot(bars, fftValue, ScottPlot.Orientation.Vertical);
                PowerPlot.PlottableList.Clear();
                timeLineBars = PowerPlot.Add.Bars(bars);
                PowerPlot.Axes.Bottom.Label.Text = $"FFT for {bottom * data.HertzFactor * 2} Hz selected, {testVal} samples.";
                PowerPlot.Axes.Bottom.Label.FontSize = 10;
                spectrumPlots.Refresh();
            }
        }

        private void btnClearAnalysis_Click(object sender, EventArgs e)
        {
            ClearRectangle();
            ClearTransformLine();
            lblSelection.Visible = false;
            tbxSelection.Visible = false;
            btnAnalyze.Visible = false;
            btnClearAnalysis.Visible = false;
            PowerPlot.Axes.Bottom.Label.Text = "FFT Windows";
            PowerPlot.Axes.Bottom.Label.FontSize = 11;
            spectrumPlots.Refresh();
        }

        private void UndoTimeLineBars()
        {
            if (timeLineBars.Bars.Count > 0)
            {
                PowerPlot.PlottableList.Remove(timeLineBars);
                TimeLevelBars = PowerPlot.Add.Bars(TimeLevelBars.Bars);
                timeLineBars.Bars.Clear();
                spectrumPlots.Refresh();
            }
        }

        private void SetAudioDataSource(SampleWaveStream waveStream)
        {
            data = new SpectrumData(waveStream, fFTWindowSize);
            SetAudioFileSource(waveStream, data);
        }

        private void SetAudioDataSource(AudioFileSampleWaveStream source)
        {
            data = new SpectrumData(source, fFTWindowSize, source.FileName);
            SetAudioFileSource(source, data);
        }

        private void tbxFFTWindowSize_TextChanged(object sender, EventArgs e)
        {
            var digitstr = RemoveNonDigits(tbxFFTWindowSize.Text);
            if (digitstr != tbxFFTWindowSize.Text)
                tbxFFTWindowSize.Text = digitstr;
        }

        private void TbxFFTWindowSize_LostFocus(object sender, EventArgs e)
        {
            var windowSize = GetLargest2Factor(tbxFFTWindowSize.Text);
            tbxFFTWindowSize.Text = windowSize.ToString();
        }

        private int GetLargest2Factor(string text)
        {
            string val = RemoveNonDigits(text);
            if (val.Length == 0)
                return 0;
            return GetLargest2Factor(int.Parse(val));
        }

        private string RemoveNonDigits(string text)
        {
            var sb = new StringBuilder();
            for (int t = 0; t < text.Length; t++)
            {
                if (char.IsDigit(text[t]))
                    sb.Append(text[t]);
            }
            return sb.ToString();
        }

        private int GetLargest2Factor(int window)
        {
            int testVal = 1;
            while (testVal < window)
            {
                testVal *= 2;
            }
            testVal /= 2;
            return testVal;
        }

        private void btnShowSignal_Click(object sender, EventArgs e)
        {
            getSampleTime = true;
        }

        private void DisplaySampleTime()
        {
            double columnPos = MouseNowCoordinates.X;
            if (data != null)
            {
                var samplePos = (int)(columnPos * FFTWindowSize);
                int samplesPerBuffer = data.WaveFormat.ConvertLatencyToByteSize(FFTWindowSize) / data.WaveFormat.BlockAlign;
                float[] samples = new Span<float>(data.SampleValues, samplePos, FFTWindowSize).ToArray();
                ExtraPlot.Clear();
                ExtraPlot.Add.Signal(samples);
            }
        }

        private void cbxSmooth_CheckedChanged(object sender, EventArgs e)
        {
            HMap.Smooth = cbxSmooth.Checked;
            spectrumPlots.Refresh();
        }
    }

}
