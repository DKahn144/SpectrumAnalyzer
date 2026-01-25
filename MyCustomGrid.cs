using ScottPlot;
using System;
using System.Collections.Generic;
using System.Text;
using ScottPlot.MultiplotLayouts;

namespace SpectrumAnalyzer
{
    public class MyCustomGrid : IMultiplotLayout
    {
        readonly Dictionary<Plot, MyGridCell> PlotCells = [];

        public PixelRect[] GetSubplotRectangles(SubplotCollection subplots, PixelRect figureRect)
        {
            PixelRect[] rectangles = new PixelRect[subplots.Count];

            Plot[] plots = subplots.GetPlots();

            for (int i = 0; i < plots.Length; i++)
            {
                if (PlotCells.TryGetValue(plots[i], out MyGridCell? cell))
                {
                    rectangles[i] = cell.GetRect(figureRect);
                }
                else
                {
                    rectangles[i] = PixelRect.NaN;
                }
            }

            return rectangles;
        }

        public void Set(Plot plot, MyGridCell gridCell)
        {
            PlotCells[plot] = gridCell;
        }
    }
}
