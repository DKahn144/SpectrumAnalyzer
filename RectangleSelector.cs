using ScottPlot;
using ScottPlot.AxisPanels;
using SpectrumProcessor;
using System;
using System.Collections.Generic;
using System.Text;
using static ScottPlot.Colors;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpectrumAnalyzer
{
    public class RectangleSelector : ISelectionMethod
    {
        public RectangleSelector()
        {
            name = "Rectangle";
            description = "Select a rectangle from the spetrum heatmap and drill down";
            forecolor = System.Drawing.Color.DarkRed;
            plottablecolor = Colors.Red;
        }

        protected bool isActive = false;
        protected bool DraggingShape = false;
        protected virtual ScottPlot.Plottables.Rectangle? CurrentShape => plottables.FirstOrDefault() as ScottPlot.Plottables.Rectangle;
        protected System.Drawing.Color forecolor;
        protected ScottPlot.Color plottablecolor;
        protected ScottPlot.Color plottableFillColor => plottablecolor.WithAlpha(.1);
        protected OutputType outputType = OutputType.NewSpectrumViewer;
        protected List<IPlottable> plottables = new List<IPlottable>();
        protected string name = string.Empty;
        protected string description = string.Empty;

        protected SpectrumData Data => Manager.Control.Data!;

        #region fixed public members

        public string Name => name;

        public string Description => description;

        public required SelectionManager Manager { get; set; }

        public System.Drawing.Color Forecolor => forecolor;

        public ScottPlot.Color Plottablecolor => plottablecolor;

        public OutputType ResultingType => outputType;

        public bool IsActive => isActive;

        public List<IPlottable> Plottables => plottables;

        public void Activate()
        {
            isActive = true;
            foreach(var item in plottables)
            {
                Manager.Control.FftPlot.PlottableList.Add(item);
                item.IsVisible = true;
            }
        }

        public void Deactivate()
        {
            isActive = false;
            foreach (var item in plottables)
            {
                item.IsVisible = false;
                Manager.Control.FftPlot.PlottableList.Remove(item);
            }
        }

        public void Reset()
        {
            plottables.Clear();
        }

        #endregion

        public virtual string[] SelectionText()
        {
            return plottables.Select(p => PlottableDescription(p)).ToArray();
        }

        protected virtual string PlottableDescription(ScottPlot.IPlottable item)
        {
            if (item is ScottPlot.Plottables.Rectangle)
            {
                var rect = item as ScottPlot.Plottables.Rectangle;
                return $"({rect.X1:N1},{rect.Y1:N1}) - ({rect.X2:N1},{rect.Y2:N1})";
            }
            return item?.ToString() ?? string.Empty;
        }

        public virtual void MouseDown(Coordinates coordinates, MouseEventArgs e)
        {
            if (CurrentShape != null && CurrentShape.CoordinateRect.Contains(coordinates))
            {
                DraggingShape = true;
                MouseMove(coordinates, e);
            }
            else
            {
                ClearPlottables();
                CreateRectangle(coordinates);
                DraggingShape = true;
            }
            Manager.SelectionMethodChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void MouseMove(Coordinates coordinates, MouseEventArgs e)
        {
            if (DraggingShape && CurrentShape != null)
            {
                // closest corner?

                double leftdiff = Math.Abs(CurrentShape.CoordinateRect.Left - coordinates.X);
                double rightdiff = Math.Abs(CurrentShape.CoordinateRect.Right - coordinates.X);
                double bottomdiff = Math.Abs(CurrentShape.CoordinateRect.Bottom - coordinates.Y);
                double topdiff = Math.Abs(CurrentShape.CoordinateRect.Top - coordinates.Y);

                CurrentShape.CoordinateRect = new CoordinateRect(
                    leftdiff < rightdiff ? coordinates.X : CurrentShape.CoordinateRect.Left,
                    leftdiff > rightdiff ? coordinates.X : CurrentShape.CoordinateRect.Right,
                    bottomdiff < topdiff ? coordinates.Y : CurrentShape.CoordinateRect.Bottom,
                    bottomdiff > topdiff ? coordinates.Y : CurrentShape.CoordinateRect.Top);

                Manager.SelectionMethodChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual void MouseUp(Coordinates coordinates, MouseEventArgs e)
        {
            if (DraggingShape && CurrentShape != null)
            {
                DraggingShape = false;
                CurrentShape.LinePattern = LinePattern.Solid;
                Manager.SelectionMethodChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected void ClearPlottables()
        {
            foreach(var item in plottables)
            {
                Manager.Control.FftPlot.Remove(item);
            }
            plottables.Clear();
        }

        protected ScottPlot.Plottables.Rectangle CreateRectangle(Coordinates coordinates)
        {
            var rect = Manager.Control.FftPlot.Add.Rectangle(0, 0, 0, 0);
            rect.IsVisible = true;
            rect.LineColor = plottablecolor;
            rect.FillStyle.Color = plottableFillColor;
            rect.LinePattern = LinePattern.Dashed;
            rect.LineWidth = 2;
            rect.CoordinateRect = new (coordinates.X, coordinates.X+1, coordinates.Y, coordinates.Y+1);
            plottables.Add(rect);
            DraggingShape = true;
            return rect;
        }

        public virtual void ApplySelection()
        {
            ScottPlot.Plottables.Rectangle? rect = plottables.OfType<ScottPlot.Plottables.Rectangle>().FirstOrDefault();
            if (rect != null)
            {
                rect = ReviseRectangle(rect);
                var records = GetRecordsFromSelection(rect);
                float[] sampleSet = GetNewSampleSet(records, rect);
                string newFileName = $"{Data.SourceFileName.Replace(".wav", GetNewFilenamePart(rect))}.wav";
                var waveStream = BuildWaveStream(sampleSet, newFileName);
                var data = BuildSpectrumData(waveStream, Data.FftWindowHeight, newFileName);
                OpenNewViewer(waveStream, data);
            }
        }

        protected ScottPlot.Plottables.Rectangle ReviseRectangle(ScottPlot.Plottables.Rectangle rect)
        {
            double left = Math.Round(rect.CoordinateRect.Left);
            double right = Math.Round(rect.CoordinateRect.Right);
            double bottom = Math.Round(rect.CoordinateRect.Bottom);
            double top = Math.Round(rect.CoordinateRect.Top);
            if (left < 0) left = 0;
            if (right < 0) right = 0;
            if (right > Data.FftCount) right = Data.FftCount;
            if (bottom < 0) bottom = 0;
            if (top < 0) top = 0;
            if (top > Data.BufferSize) top = Data.BufferSize;
            if (left == right) right++;
            if (bottom == top) top++;
            return new ScottPlot.Plottables.Rectangle()
            {
                LineColor = rect.LineColor,
                LineWidth = rect.LineWidth,
                FillColor = rect.FillColor,
                LineStyle = rect.LineStyle,
                X1 = left,
                X2 = right,
                Y1 = bottom,
                Y2 = top
            };
        }

        protected virtual List<FftRecord> GetRecordsFromSelection(ScottPlot.Plottables.Rectangle rect)
        {
            List<FftRecord> sampleRecords = Data.Records.Records
                .Skip((int) rect.X1)
                .Take((int) (rect.X2 - rect.X1))
                .Select(rec => rec.Copy()).ToList();
            sampleRecords.ForEach(r => r.CalculateSlope(sampleRecords.ToArray()));

            if (rect.Y2 >= Data.FftWindowHeight || rect.Y1 > 0)
            {
                foreach(var rec in sampleRecords)
                {
                    for (int j = 0; j < Data.FftWindowHeight; j++)
                    {
                        if (j <= rect.Y1 || j > rect.Y2)
                            rec.FftColumn[j] = 0;
                    }
                }
            }
            return sampleRecords;
        }

        /// <summary>
        /// Create an array of audio samples for building a new audio stream for the selected data.
        /// </summary>
        /// <param name="records">The selected FFT records</param>
        /// <param name="rect">The rectangle defining the selection</param>
        /// <returns></returns>
        protected virtual float[] GetNewSampleSet(List<FftRecord> records, ScottPlot.Plottables.Rectangle? rect = null)
        {
            if (rect != null && (rect.Y2 > 0 || rect.Y1 > 0))
            {
                // generate samples from the FFT columns for each record
                records.ForEach(r => r.Samples = SpectrumData.GetReverseFFT(r.FftColumn, (int) rect.Y1, (int) rect.Y2));
            }
            var samplesCollection = records.SelectMany(r => r.Samples).ToArray();
            return samplesCollection;
        }

        protected virtual string GetNewFilenamePart(ScottPlot.Plottables.Rectangle rect)
        {
            string windowStr = $":col{rect.X1:N0}-col{rect.X2:N0}";
            if (rect.Y2 > 0 || rect.Y1 > 0)
                windowStr += $":{rect.Y1 * Data.HertzFactor:N0}hz - {rect.Y2 * Data.HertzFactor:N0}hz";
            return windowStr;
        }

        protected virtual AudioFileSampleWaveStream BuildWaveStream(float[] samples, string newFilename)
        {
            AudioFileSampleWaveStream rectStream = new(samples, 0, samples.Length * Data.WaveFormat.BitsPerSample / 8,
                Data.WaveFormat, newFilename);
            return rectStream;
        }

        protected virtual SpectrumData BuildSpectrumData(AudioFileSampleWaveStream waveStream, int windowSize, string newFilename)
        {
            return new SpectrumData(waveStream, windowSize, newFilename);
        }

        protected virtual void OpenNewViewer(AudioFileSampleWaveStream waveStr, SpectrumData newData)
        {
            var spectrumViewer = new SpectrumViewer();
            spectrumViewer.SetAudioWaveSource(waveStr, newData);
            spectrumViewer.Left = Manager.Control.Left + 20;
            spectrumViewer.Top = Manager.Control.Top + 20;
            spectrumViewer.Height = Manager.Control.Height;
            spectrumViewer.Width = Manager.Control.Width;
            spectrumViewer.Text = $"Detailed Analysis ({waveStr.FileName})";
            spectrumViewer.spectrumAnalysisControl1.EnableSaveWaveFile(true);
            spectrumViewer.Show();
            spectrumViewer.Focus();
        }
    }
}
