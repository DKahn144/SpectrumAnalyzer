using ScottPlot;
using ScottPlot.Colormaps;
using SpectrumProcessor;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SpectrumAnalyzer
{
    public class SignalInspector : RectangleSelector
    {
        List<ScottPlot.Bar> bars = new List<ScottPlot.Bar>();

        public SignalInspector()
        {
            name = "Signal Inspector";
            description = "Select a region of the spectrum heatmap to inspect the signal in that region";
            forecolor = System.Drawing.Color.DarkGreen;
            plottablecolor = ScottPlot.Colors.Green;
        }

        public override void MouseDown(Coordinates coordinates, MouseEventArgs e)
        {
            if (Manager.Control.EventInHeatmap(coordinates))
            {
                Manager.Control.LoadPowerPlot();
            }
            base.MouseDown(coordinates, e);
        }

        public override void ApplySelection()
        {
            ScottPlot.Plottables.Rectangle? rect = plottables.OfType<ScottPlot.Plottables.Rectangle>().FirstOrDefault();
            if (rect != null)
            {
                rect = ReviseRectangle(rect);
                var records = GetRecordsFromSelection(rect);

                int leftWindow = (int) rect.X1;
                int rightWindow = (int) rect.X2;
                int bottom = (int) rect.Y1;
                int top = (int) rect.Y2;
                int window = rightWindow - leftWindow;
                var fftSlice = new System.Numerics.Complex[window];
                //ValueMeasure[] fftValues = new ValueMeasure[window];
                ValueMeasure[] fftValues = records.Select(r => r.Magnitude).ToArray();
                for (int i = leftWindow; i < rightWindow; i++)
                {
                    int n = i - leftWindow;
                    //fftValues[n] = new ValueMeasure();
                    var record = records[i - leftWindow];
                    for (int j = bottom; j < top; j++)
                    {
                        
                        fftValues[n].AddValue(j, (float)(record.FftColumn[j].Magnitude));
                    }
                }
                FftRecords.CalculateSmoothCurve(records.ToArray());
                string label =  $"FFT for {(int)(bottom * Data.HertzFactor)} to {(int)(top * Data.HertzFactor)} Hz selected, {window} samples.";
                bars = Manager.Control.LoadPowerBars(bars, fftValues, label, leftWindow, 10);
            }
        }
    }
}
