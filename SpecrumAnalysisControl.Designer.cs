using ScottPlot;

namespace SpectrumAnalyzer
{
    partial class SpecrumAnalysisControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            spectrumPlots = new ScottPlot.WinForms.FormsPlot();
            panel1 = new Panel();
            panel2 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // spectrumPlots
            // 
            spectrumPlots.BackColor = SystemColors.ControlDark;
            spectrumPlots.DisplayScale = 1F;
            spectrumPlots.Dock = DockStyle.Fill;
            spectrumPlots.Location = new Point(0, 0);
            spectrumPlots.Margin = new Padding(2);
            spectrumPlots.Name = "spectrumPlots";
            spectrumPlots.Size = new Size(986, 569);
            spectrumPlots.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(97, 569);
            panel1.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.GradientActiveCaption;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(97, 565);
            panel2.TabIndex = 0;
            // 
            // SpecrumAnalysisControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel1);
            Controls.Add(spectrumPlots);
            Name = "SpecrumAnalysisControl";
            Size = new Size(986, 569);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        #region Manually entered initialization

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            spectrumPlots.Multiplot.AddPlot();
            spectrumPlots.Multiplot.AddPlot();
        }
        
        #endregion

        private ScottPlot.WinForms.FormsPlot spectrumPlots;
        private Panel panel1;
        private Panel panel2;
    }
}
