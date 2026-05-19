using ScottPlot;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpectrumAnalyzer
{
    public class SelectionManager
    {

        internal SpectrumAnalysisControl Control;
        private List<ISelectionMethod> selectionMethods = new List<ISelectionMethod>();

        public SelectionManager(SpectrumAnalysisControl control)
        { 
            this.Control = control;
            this.Control.SelectionMgr = this;
        }

        public void RegisterSelectionMethod(ISelectionMethod method)
        {
            if (method == null) 
                throw new ArgumentNullException(nameof(method));

            if (GetMethodByName(method.Name) == null)
            {
                selectionMethods.Add(method);
                method.Manager = this;
            }
        }

        public ISelectionMethod? GetMethodByName(string name)
        {
            return selectionMethods.FirstOrDefault(m => String.Compare(m.Name, name, true) == 0);
        }

        public EventHandler<EventArgs> SelectionMethodChanged;

        public ISelectionMethod? ActiveSelectionMethod { get; private set; }
        
        public bool IsActive => ActiveSelectionMethod != null;

        public IEnumerable<ISelectionMethod> SelectionMethods => selectionMethods;

        public System.Drawing.Color ActiveColor 
        { 
            get 
            { 
                if (ActiveSelectionMethod != null)
                {
                    return ActiveSelectionMethod.Forecolor;
                }
                return System.Drawing.Color.DarkGray;
            } 
        }

        public void UseSelectionMethod(string name)
        {
            var method = GetMethodByName(name);
            if (method != null)
            {
                if (method == ActiveSelectionMethod)
                    return;
                if (ActiveSelectionMethod != null)
                {
                    ActiveSelectionMethod.Deactivate();
                }
                ActiveSelectionMethod = method;
                ActiveSelectionMethod.Activate();
            }
            else
            {
                if (ActiveSelectionMethod != null)
                {
                    ActiveSelectionMethod.Deactivate();
                }
                ActiveSelectionMethod = null;
            }
            SelectionMethodChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void HandleMouseDown(Coordinates mouseDownCoordinates, MouseEventArgs e)
        {
            if (ActiveSelectionMethod != null)
            {
                ActiveSelectionMethod.MouseDown(mouseDownCoordinates, e);
            }
        }

        internal void HandleMouseMove(Coordinates coordinates, MouseEventArgs e)
        {
            if (ActiveSelectionMethod != null)
            {
                ActiveSelectionMethod.MouseMove(coordinates, e);
            }
        }

        internal void HandleMouseUp(Coordinates mouseDownCoordinates, MouseEventArgs e)
        {
            if (ActiveSelectionMethod != null)
            {
                ActiveSelectionMethod.MouseUp(mouseDownCoordinates, e);
            }
        }

        internal string[] GetSelectionText()
        {
            if (ActiveSelectionMethod != null)
            {
                return ActiveSelectionMethod.SelectionText();
            }
            return Array.Empty<string>();
        }

        internal void ApplySelection()
        {
            if (ActiveSelectionMethod != null)
            {
                ActiveSelectionMethod.ApplySelection();
            }
        }
    }
}
