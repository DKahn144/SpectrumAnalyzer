using Microsoft.UI;
using ScottPlot;
using SpectrumProcessor;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpectrumAnalyzer
{
    public class SegmentSelector : RectangleSelector
    {
        private ScottPlot.Plottables.Rectangle? currentShape;

        public SegmentSelector()
        {
            name = "Segments";
            description = "Select a series of segments from the spectrum heatmap to join into a new file";
            forecolor = System.Drawing.Color.DarkGoldenrod;
            plottablecolor = ScottPlot.Colors.DarkGoldenRod;
        }

        protected override ScottPlot.Plottables.Rectangle? CurrentShape => currentShape;

        protected override string PlottableDescription(ScottPlot.IPlottable item)
        {
            if (item is ScottPlot.Plottables.Rectangle)
            {
                var rect = item as ScottPlot.Plottables.Rectangle;
                return $"({rect?.X1:N1} - {rect?.X2:N1})";
            }
            return item?.ToString() ?? string.Empty;
        }

        public override void MouseDown(Coordinates coordinates, MouseEventArgs e)
        {
            if (CurrentShape != null && CurrentShape.CoordinateRect.Contains(coordinates))
            {
                DraggingShape = true;
                MouseMove(coordinates, e);
            }
            else
            {
                //ClearPlottables();
                currentShape = CreateSegment(coordinates);
                DraggingShape = true;
                
            }
            Manager.SelectionMethodChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void MouseMove(Coordinates coordinates, MouseEventArgs e)
        {
            if (DraggingShape && CurrentShape != null)
            {
                if (coordinates.Y > 0 && coordinates.X >= 0)
                {
                    if (coordinates.X < CurrentShape.X2 && coordinates.X > CurrentShape.X1)
                    {
                        CurrentShape.X2 = coordinates.X;
                    }
                    else
                    {
                        if (coordinates.X > CurrentShape.X2)
                        {
                            CurrentShape.X2 = coordinates.X;
                        }
                        else if (coordinates.X < CurrentShape.X1)
                        {
                            CurrentShape.X1 = coordinates.X;
                        }
                        var otherRect = RegionWasSelected(coordinates, CurrentShape);
                        if (otherRect != null && otherRect != CurrentShape)
                        {
                            MergeRegion(otherRect);
                        }
                    }
                }
                Manager.SelectionMethodChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void MouseUp(Coordinates coordinates, MouseEventArgs e)
        {
            DraggingShape = false;
            if (currentShape != null)
            {
                currentShape.LinePattern = LinePattern.Solid;
                currentShape = null;
            }
            Manager.SelectionMethodChanged?.Invoke(this, EventArgs.Empty);
        }

        private void MergeRegion(ScottPlot.Plottables.Rectangle otherRect)
        {
            if (CurrentShape == null)
            {
                currentShape = otherRect;
                return;
            }
            CurrentShape?.X1 = Math.Min(CurrentShape.X1, otherRect.X1);
            CurrentShape?.X2 = Math.Max(CurrentShape.X2, otherRect.X2);
            plottables.Remove(otherRect);
            Manager.Control.FftPlot.PlottableList.Remove(otherRect);
        }

        private ScottPlot.Plottables.Rectangle? RegionWasSelected(Coordinates point, ScottPlot.Plottables.Rectangle? excludeRect = null)
        {
            foreach (var rect in plottables.Cast<ScottPlot.Plottables.Rectangle>())
            {
                if (rect == excludeRect)
                    continue;
                if (rect.CoordinateRect.ContainsX(point.X) ||
                    (excludeRect != null && rect.X1 <= excludeRect.X2 && rect.X2 >= excludeRect.X1))
                {
                    return rect;
                }
            }
            return null;
        }

        protected ScottPlot.Plottables.Rectangle CreateSegment(Coordinates coordinates)
        {
            var segment = base.CreateRectangle(coordinates);
            segment.Y1 = 0;
            segment.Y2 = Data.FftWindowHeight;
            return segment;
        }

        public override void ApplySelection()
        {
            if (plottables.Count > 0)
            {
                List<FftRecord> selection = new List<FftRecord>();
                foreach (ScottPlot.Plottables.Rectangle? rect in plottables.OfType<ScottPlot.Plottables.Rectangle>())
                {
                    var sampleRecords = Data.Records.Records
                        .Skip((int)rect.X1)
                        .Take((int)(rect.X2 - rect.X1))
                        .Select(rec => rec.Copy());
                    selection.AddRange(sampleRecords);
                }

                float[] sampleSet = GetNewSampleSet(selection);
                string namePart = $" {(int) (selection.Count*100/Data.Records.Records.Count())} pct";
                string newFileName = Data.SourceFileName.Replace(".wav", $"{namePart}.wav");
                var waveStream = BuildWaveStream(sampleSet, newFileName);
                var data = BuildSpectrumData(waveStream, Data.FftWindowHeight, newFileName);
                OpenNewViewer(waveStream, data);

            }
        }


    }
}
