using FftSharp;
using Microsoft.UI;
using Microsoft.UI.Xaml.Shapes;
using NAudio.Dsp;
using NAudio.Wave;
using ScottPlot;
using ScottPlot.MultiplotLayouts;
using ScottPlot.Plottables;
using ScottPlot.WinForms;
using SpectrumProcessor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using Windows.Storage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SpectrumAnalyzer
{
    public partial class SpectrumAnalysisControl : UserControl
    {
        public SpectrumAnalysisControl()
        {
            InitializeComponent();
            SetupSubgrids();
            LoadDummyData();
            SetupRectangleSelector();
        }

        internal void spectrumAnalysisControl1_Load(object sender, EventArgs e)
        {
            powersOf2.ForEach(p => cbxFFTWindow.Items.Add(p));
            cbxFFTWindow.SelectedItem = fFTWindowSize;
        }

        private void SetupRectangleSelector()
        {
            // add events to trigger in response to mouse actions
            spectrumPlots.MouseMove += OnMouseMove;
            spectrumPlots.MouseDown += OnMouseDown;
            spectrumPlots.MouseUp += OnMouseUp;
            spectrumPlots.MouseClick += OnMouseClick;
            spectrumPlots.MouseDoubleClick += OnMouseDoubleClick;
        }

        public void SetAudioFileSource(SampleWaveStream? sourceWaveStream, SpectrumData data)
        {
            if (waveStream != null && waveStream != sourceWaveStream)
            {
                this.data = null;
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
                    if (data == null)
                    {
                        this.data = new SpectrumData(waveStream, SpectrumData.DefaultWindowSize, this.fileName);
                    }
                    else
                    {
                        this.data = data;
                    }
                    //cbxSelectionMethod.SelectedIndex = 0;
                    this.data.ReadSource();
                    LoadPlotData();
                }
            }
        }

        public void UpdateFilePosition()
        {
            if (waveStream != null && Data != null)
            {
                var time = waveStream.CurrentTime;
                filePos.X = waveStream.Position / Data!.SampleSize;
                filePos.IsVisible = true;
                if (waveStream.Position > 0 && !FftPlot.PlottableList.Contains(filePos))
                {
                    FftPlot.PlottableList.Add(filePos);
                }
                RefreshControl(spectrumPlots);
            }
            else
            {
                filePos.IsVisible = false;
            }

        }

        public SpectrumData? Data => data;

        public SampleWaveStream? WaveStream => waveStream;

        public void EnableSaveWaveFile(bool enable)
        {
            btnSaveWaveFile.Enabled = enable;
            btnSaveData.Visible = enable;
            btnSaveData.Enabled = enable;
        }

        private const string NoItem = "None";


        private Plot FreqLevelPlot => spectrumPlots.Multiplot.GetPlot(0);
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
        private List<int> powersOf2 = new List<int> { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 };
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private SpectrumData? data;
        private int desiredLatency = 200;
        private int timeLineY = 0;
        private int fFTWindowSize = 1024;
        private ScottPlot.Plottables.Rectangle? RectanglePlot;
        private ScottPlot.Plottables.Rectangle? TransformTimeLine;
        private static SpectrumAnalysisControl? firstInstance;
        private string fileName = "";
        Coordinates MouseDownCoordinates;
        Coordinates MouseNowCoordinates;
        CoordinateRect MouseSelectionRect => new(MouseDownCoordinates, MouseNowCoordinates);

        public int FFTWindowSize => fFTWindowSize;

        public string FileName => fileName;

        public Plot FftPlot => spectrumPlots.Multiplot.GetPlot(1);

        private void SetupSubgrids()
        {
            spectrumPlots.Multiplot.AddPlots(4);
            // use a fixed layout to ensure all plots remain aligned
            PixelPadding padding = new(46, 10, 40, 10);
            foreach (Plot plot in spectrumPlots.Multiplot.GetPlots())
                plot.Layout.Fixed(padding);

            FreqLevelBars = new BarPlot(new List<Bar>());
            FreqLevelBars.Horizontal = true;

            TimeLevelBars = new BarPlot(new List<Bar>());
            TimeLevelBars.Horizontal = false;

            MyCustomGrid customGrid = new();
            customGrid.Set(FreqLevelPlot, new MyGridCell(0, 0, 7, 8, 5, 1));
            customGrid.Set(FftPlot, new MyGridCell(0, 1, 7, 8, 5, 7));
            customGrid.Set(PowerPlot, new MyGridCell(5, 1, 7, 8, 1, 7));
            customGrid.Set(ExtraPlot, new MyGridCell(6, 0, 7, 8, 1, 8));

            var leftAxis = ExtraPlot.Axes.Left;
            var bottomAxis = ExtraPlot.Axes.Bottom;
            bottomAxis.Label.Text = "Sample";
            bottomAxis.Label.FontSize = 11;
            bottomAxis.Range.Min = -25;
            bottomAxis.Range.Max = 1050;
            leftAxis.Label.Text = "Magnitude";
            leftAxis.Label.FontSize = 11;
            ExtraPlot.Axes.AutoScaleY();

            spectrumPlots.Multiplot.Layout = customGrid;
            spectrumPlots.Multiplot.SharedAxes.ShareX([FftPlot, PowerPlot]);
            spectrumPlots.Multiplot.SharedAxes.ShareY([FreqLevelPlot, FftPlot]);

            FreqLevelPlot.Axes.Left.TickLabelStyle.IsVisible = false;
            FreqLevelPlot.Axes.Left.MajorTickStyle.Length = 0;
            FreqLevelPlot.Axes.Left.MinorTickStyle.Length = 0;
            FreqLevelPlot.Layout.Fixed(new PixelPadding(10, 10, 40, 10));
            DisableAnalyzeButton();
            FftPlot.PlottableList.Add(filePos);
            filePos.Color = ScottPlot.Colors.Gray;
            filePos.LineWidth = 4;
            filePos.IsVisible = false;
            RefreshControl(spectrumPlots);
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
            if (data == null || data.FftCount == 0)
            {
                LoadDummyData();
                return;
            }

            // load the frequency levels
            LoadFrequencyData(Data!);

            LoadPowerPlot();

            //string HzTickLabelFormatter(double y) => $"{(2 * y * SpectrumData.HertzFactor):N0}";
            string HzTickLabelFormatter(double y) =>
                $"{(y * Data!.HertzFactor):N0}";
            string TimeTickLabelFormatter(double y) =>
                $"{(y * Data!.SecondsPerBuffer):00}";

            ScottPlot.TickGenerators.NumericFixedInterval leftTickGen = new ScottPlot.TickGenerators.NumericFixedInterval(50.0F / Data!.HertzFactor);

            ScottPlot.TickGenerators.NumericFixedInterval bottomTickGen = new ScottPlot.TickGenerators.NumericFixedInterval(1.0F / Data!.SecondsPerBuffer);

            // tell our major tick generator to only show major ticks that are whole integers
            // leftTickGen.TickDensity = 1;
            // leftTickGen.IntegerTicksOnly = true;
            // tell our custom tick generator to use our new label formatter
            leftTickGen.LabelFormatter = HzTickLabelFormatter;
            // tell the left axis to use our custom tick generator
            FftPlot.Axes.Left.TickGenerator = leftTickGen;
            FftPlot.Axes.Margins(left: 0, right: 0);

            //bottomTickGen.IntegerTicksOnly = false;
            bottomTickGen.LabelFormatter = TimeTickLabelFormatter;
            FftPlot.Axes.Bottom.TickGenerator = bottomTickGen;
            FftPlot.Axes.Bottom.Label.Text = "Seconds";
            FftPlot.Axes.Bottom.Label.FontSize = 11;

            FftPlot.Axes.Left.Label.Text = "Hz";
            FftPlot.Axes.Left.Label.FontSize = 11;
            int colHeight = data.FftWindowHeight;
            int colCount = data.Records.Records.Length;
            HeatmapData = new double[colHeight, colCount];
            double maxMag = 0;
            for (int x = 0; x < colCount; x++)
            {
                for (int y = 0; y < colHeight; y++)
                {
                    double mag = data.Records.Records[x].FftColumn[y].Magnitude;
                    if (mag > 2.0D && y > 450)
                    {

                    }
                    try
                    {
                        HeatmapData[colHeight - 1 - y, x] = mag;
                        if (mag > maxMag) maxMag = mag;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
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

            RefreshControl(spectrumPlots);
        }

        private void LoadFrequencyData(SpectrumData data, int itemId = -1)
        {
            List<Bar> fBars = FreqLevelBars.Bars;
            fBars.Clear();
            ValueMeasure[] freqVals = data.FrequencyMags.ToArray();
            if (itemId >= 0)
            {
                freqVals = data.Records.Records[itemId].Magnitude.GetValueMeasuresFromValues();
            }
            LoadBarPlotData(fBars, freqVals, ScottPlot.Orientation.Horizontal);
            FreqLevelBars = FreqLevelPlot.Add.Bars(fBars);
            FreqLevelPlot.Axes.Margins(left: 0);

            // Determine average weighted frequency nds display it
            float avgFrequency = 0;
            float avgVolume = 0;
            (avgFrequency, avgVolume) = data.GetWeightedAvgFreq(itemId);
            tbxAvgFreq.Text = Math.Round(avgFrequency, 1).ToString("0.0");
            tbxAvgVol.Text = Math.Round(avgVolume, 1).ToString("0.0");
        }


        /// <summary>
        /// load the timespan levels
        /// </summary>
        public void LoadPowerPlot()
        {
            LoadPowerBars(TimeLevelBars.Bars, data!.TimeMags, "FFT Windows", 0, 11);
            TimeLevelBars = PowerPlot.Add.Bars(TimeLevelBars.Bars);
        }

        public List<Bar> LoadPowerBars(List<Bar> bars, ValueMeasure[] values, string label, int startX, int fontSize)
        {
            PowerPlot.Axes.Bottom.Label.Text = label;
            PowerPlot.Axes.Bottom.Label.FontSize = fontSize;
            PowerPlot.PlottableList.Clear();
            List<CoordinateLine> sLines = new List<CoordinateLine>();
            LoadBarPlotData(bars, values, ScottPlot.Orientation.Vertical, startX, sLines);
            for (int i = 0; i < sLines.Count; i++)
            {
                PowerPlot.Add.Line(sLines[i]);
            }
            foreach (var line in PowerPlot.GetPlottables<LinePlot>())
            {
                line.Color = ScottPlot.Colors.Red;
                line.LineWidth = 2;
            }
            PowerPlot.Add.Bars(bars);
            PowerPlot.Axes.Margins(left: 0, right: 0);
            PowerPlot.Axes.AutoScaleY();
            RefreshControl(spectrumPlots);
            return bars;
        }

        private void LoadBarPlotData(List<Bar> bars, ValueMeasure[] values,
            ScottPlot.Orientation orientation, int startPos = 0, List<CoordinateLine>? avgLines = null)
        {
            float[] smoothValues = new float[values.Length];
            int box = 5;
            bars.Clear();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null)
                {
                    var bar = new Bar()
                    {
                        Position = i + startPos,
                        Size = barWidth,
                        Value = values[i].AvgValue,
                        Error = values[i].RMSValue,
                        Orientation = orientation
                    };
                    bars.Add(bar);

                }
            }
            if (avgLines != null)
            {
                avgLines.Clear();
                for (int i = 0; i < values.Length - 1; i++)
                {
                    float startX, startY, endX, endY;
                    if (orientation == ScottPlot.Orientation.Vertical)
                    {
                        startX = i + startPos;
                        startY = values[i].SmoothValue;
                        endX = i + 1 + startPos;
                        endY = values[i + 1].SmoothValue;
                    }
                    else
                    {
                        startY = i + startPos;
                        startX = values[i].SmoothValue;
                        endY = i + 1 + startPos;
                        endX = values[i + 1].SmoothValue;
                    }
                    avgLines.Add(new CoordinateLine(startX, startY, endX, endY));
                }
            }
        }

        public bool EventInHeatmap(Coordinates coords)
        {
            return coords.X >= -1 &&
                coords.X < Data?.FftCount + 2 &&
                coords.Y >= -2 &&
                coords.Y <= Data?.FftWindowHeight + 2;
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            MouseDownCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
            DisplayCoords(MouseDownCoordinates);
            if (EventInHeatmap(MouseDownCoordinates))
            {
                if (SelectionMgr.IsActive)
                {
                    spectrumPlots.UserInputProcessor.Disable(); // disable the default click-drag-pan behavior
                    SelectionMgr.HandleMouseDown(MouseDownCoordinates, e);
                }
                else
                {
                    spectrumPlots.UserInputProcessor.Enable(); // disable the default click-drag-pan behavior
                    int x = (int)MouseDownCoordinates.X;
                    if (cbxShowSignal.Checked)
                    {
                        filePos.IsVisible = true;
                        if (!FftPlot.PlottableList.Contains(filePos))
                        {
                            FftPlot.PlottableList.Add(filePos);
                        }
                        filePos.X = x;
                        DisplaySampleTime();
                        LoadFrequencyData(Data!, x);
                    }
                    if (filePos.IsVisible)
                    {
                        if (waveStream != null && x >= 0)
                        {
                            filePos.IsVisible = true;
                            filePos.X = x;
                            waveStream.Position = (long)(x * Data!.SampleSize);
                            //waveStream.Position = (long)((x * waveStream.Length) / (waveStream.CurrentTime.TotalSeconds / Data!.SecondsPerBuffer));
                        }
                    }
                }
            }
            RefreshControl(spectrumPlots);
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            MouseNowCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
            DisplayCoords(MouseNowCoordinates);
            if (SelectionMgr.IsActive && EventInHeatmap(MouseNowCoordinates))
            {
                SelectionMgr.HandleMouseMove(MouseNowCoordinates, e);
            }
            RefreshControl(spectrumPlots);
            if (Data != null)
            {
                var mouseTime = MouseNowCoordinates.X * Data!.SecondsPerBuffer;
                int minutes = (int)(mouseTime / 60);
                var seconds = mouseTime - (minutes * 60);
                tbxMouseTime.Text = MouseNowCoordinates.X >= 0 ? $"{minutes}:{seconds:N2}" : "";
                tbxMouseHz.Text = MouseNowCoordinates.Y >= 0 ? (MouseNowCoordinates.Y * Data!.HertzFactor).ToString("0.0") : "";
            }
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            MouseNowCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
            if (SelectionMgr.IsActive)
            {
                SelectionMgr.HandleMouseUp(MouseNowCoordinates, e);
                RefreshControl(spectrumPlots);
            }
            spectrumPlots.UserInputProcessor.Enable(); // disable the default click-drag-pan behavior
        }

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MouseNowCoordinates = FftPlot.GetCoordinates(e.X, e.Y);
            }
        }

        private void OnMouseDoubleClick(object? sender, MouseEventArgs e)
        {
        }

        private void DisplayCoords(Coordinates mouseCoordinates)
        {
            tbxMouseX.Text = $"{mouseCoordinates.X:N2}";
            tbxMouseY.Text = $"{mouseCoordinates.Y:N2}";
        }

        private int GetLargest2Factor(string text)
        {
            string val = RemoveNonDigits(text);
            if (val.Length == 0)
                return 0;
            return GetLargest2Factor(int.Parse(val));
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

        private void DisplaySampleTime()
        {
            double columnPos = MouseNowCoordinates.X;
            if (data != null && data.Records.Length >= (int)columnPos)
            {
                var record = data.Records.Records[(int)columnPos];
                var samplePos = (int)(columnPos * FFTWindowSize);
                float[] samples = record.Samples;
                ExtraPlot.Clear();
                ExtraPlot.Add.Signal(samples);
                ExtraPlot.Axes.AutoScaleY();
            }
        }

        private void cbxSmooth_CheckedChanged(object sender, EventArgs e)
        {
            HMap.Smooth = cbxSmooth.Checked;
            RefreshControl(spectrumPlots);
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            if (Data != null && Data.Records != null)
            {
                var saveFileDialog = new SaveFileDialog();
                string allExtensions = "Wave Data files | *.wavd";
                saveFileDialog.Filter = allExtensions;
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(FileName);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FftRecords newRecords = Data.Records;
                        JsonSerializerOptions options = new JsonSerializerOptions { };
                        options.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
                        string json = JsonSerializer.Serialize<FftRecords>(newRecords, options);
                        File.WriteAllText(saveFileDialog.FileName, json);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("{0}", ex.Message), "Error Saving File");
                        return;
                    }
                }
            }
        }



        #region Selecting areas on the heatmap

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SelectionManager SelectionMgr { get; set; }

        private void cbxSelectionMethod_VisibleChanged(object sender, EventArgs e)
        {
            if (cbxSelectionMethod.Items.Count == 0 && SelectionMgr != null)
            {
                cbxSelectionMethod.Items.Add("None");
                foreach (string name in SelectionMgr.SelectionMethods.Select(mbox => mbox.Name))
                {
                    cbxSelectionMethod.Items.Add(name);
                }
                cbxSelectionMethod.SelectedIndex = 0;
                RefreshControl(cbxSelectionMethod);
            }
        }

        private void cbxSelectionMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSelectionMethod.SelectedItem != null && cbxSelectionMethod.Text != NoItem)
            {
                SelectionMgr.UseSelectionMethod(cbxSelectionMethod.SelectedItem.ToString()!);
                EnableAnalyzeBtn();
            }
            else
            {
                SelectionMgr.UseSelectionMethod("");
                DisableAnalyzeButton();
            }

            SetSelectionText();
            RefreshControl(this);
        }

        /// <summary>
        /// Event called when the selection manager's status changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void OnSelectionMethodChanged(object? sender, EventArgs e)
        {
            if (!SelectionMgr.IsActive)
            {
                DeactivateSelectorControls();
            }
            else if (SelectionMgr.ActiveSelectionMethod != null)
            {
                SetSelectorControlColors(SelectionMgr.ActiveSelectionMethod.Forecolor);
            }
            else
            {
                SetSelectorControlColors(System.Drawing.Color.DarkGray);
            }
            SetSelectionText();
            RefreshControl(this);
        }

        private void DeactivateSelectorControls()
        {
            SetSelectorControlColors(System.Drawing.Color.DarkGray);
            btnAnalyze.Enabled = false;
            btnClearAnalysis.Enabled = false;
            lbxSelection.Items.Clear();
        }

        private void SetSelectorControlColors(System.Drawing.Color actionColor)
        {
            cbxSelectionMethod.ForeColor = actionColor;
            //lblSelectionMethod.ForeColor = actionColor;
            lbxSelection.ForeColor = actionColor;
            //lblSelection.ForeColor = actionColor;
            btnAnalyze.ForeColor = actionColor;
            btnClearAnalysis.ForeColor = actionColor;
        }

        private void EnableAnalyzeBtn()
        {
            var fontStyle = System.Drawing.FontStyle.Bold;
            var color = SelectionMgr == null ? 
                System.Drawing.Color.DarkGray : SelectionMgr.ActiveColor;
            lblSelection.Visible = true;
            lbxSelection.Visible = true;
            lbxSelection.ForeColor = color;
            cbxSelectionMethod.Font = new Font(btnAnalyze.Font, fontStyle);
            lbxSelection.Font = new Font(btnAnalyze.Font, fontStyle);
            btnAnalyze.ForeColor = color;
            btnClearAnalysis.ForeColor = color;
            btnAnalyze.Enabled = true;
            btnAnalyze.Font = new Font(btnAnalyze.Font, fontStyle);
            btnClearAnalysis.Enabled = true;
            btnClearAnalysis.Font = new Font(btnClearAnalysis.Font, fontStyle);
        }

        private void SetSelectionText()
        {
            string[] textItems = SelectionMgr.GetSelectionText();
            string selections = String.Join(',', textItems);
            IEnumerable<string> listItems = lbxSelection.Items.Cast<string>();
            string listText = String.Join(',', listItems);
            if (String.Compare(selections, listText) == 0)
                return;

            lbxSelection.BeginUpdate();
            try
            {
                lbxSelection.Items.Clear();
                foreach (var item in textItems)
                    lbxSelection.Items.Add(item);
            }
            finally
            {
                lbxSelection.EndUpdate();
            }
        }

        private void DisableAnalyzeButton()
        {
            EnableAnalyzeBtn();
            btnAnalyze.Enabled = false;
            btnClearAnalysis.Enabled = false;
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (data == null)
                return;

            SelectionMgr.ApplySelection();
        }

        private void btnClearAnalysis_Click(object sender, EventArgs e)
        {
            SetSelectionText();
            cbxSelectionMethod.SelectedIndex = 0;
            btnAnalyze.Enabled = false;
            btnClearAnalysis.Enabled = false;
            LoadPowerPlot();
            RefreshControl(spectrumPlots);
        }

        private void RefreshControl(Control ctrl)
        {
            SpectrumViewer.RunInUIContext(() => ctrl.Refresh());
        }

        #endregion

        private void cbxShowSignal_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbxShowSignal.Checked && Data != null)
            {
                LoadFrequencyData(Data!);
                ExtraPlot.PlottableList.Clear();
                RefreshControl(spectrumPlots);
            }
        }

        private void btnSaveWaveFile_Click(object sender, EventArgs e)
        {
            if (Data != null && Data.Records != null)
            {
                var saveFileDialog = new SaveFileDialog();
                string allExtensions = "Wave files | *.wav";
                saveFileDialog.Filter = allExtensions;
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(FileName);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        WaveFileWriter.CreateWaveFile(saveFileDialog.FileName, this.WaveStream);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("{0}", ex.Message), "Error Saving Wave File");
                        return;
                    }
                }
            }
        }

        private void cbxFFTWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxFFTWindow.SelectedItem != null)
            {
                int lastWindowSize = fFTWindowSize;
                int windowSize = int.Parse(cbxFFTWindow.SelectedItem.ToString()!);
                fFTWindowSize = windowSize;
                if (Data != null && Data.FftWindowSize != windowSize)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        var newData = new SpectrumData(this.WaveStream, fFTWindowSize, this.FileName);
                        SetAudioFileSource(this.WaveStream, newData);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("{0}", ex.Message), "Error Changing FFT Window Size");
                        cbxFFTWindow.SelectedItem = lastWindowSize;
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }
    }
}
