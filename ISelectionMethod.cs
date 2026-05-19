using ScottPlot;
using SpectrumProcessor;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Sms;

namespace SpectrumAnalyzer
{
    public interface ISelectionMethod
    {
        string Name { get; }

        string Description { get; }

        System.Drawing.Color Forecolor { get; }

        ScottPlot.Color Plottablecolor { get; }

        OutputType ResultingType { get; }

        bool IsActive { get; }

        void Activate();

        void Deactivate();

        void Reset();

        List<IPlottable> Plottables { get; }

        string[] SelectionText();

        void MouseDown(Coordinates coordinates, MouseEventArgs e);

        void MouseMove(Coordinates coordinates, MouseEventArgs e);

        void MouseUp(Coordinates coordinates, MouseEventArgs e);

        void ApplySelection();

        SelectionManager Manager { get; set; }
    }


    public enum OutputType
    {
        /// <summary>
        /// produces a new SpectrumViewer window with selected data
        /// </summary>
        NewSpectrumViewer,
        /// <summary>
        /// Produces a new set of power pars over the selected time
        /// </summary>
        TimeBars,
        /// <summary>
        /// Produces a collection of bars that visualize the frequency spectrum of the selected audio signal.
        /// </summary>
        FrequencyBars
    }
}
