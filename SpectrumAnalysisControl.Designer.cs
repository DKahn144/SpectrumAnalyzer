using ScottPlot;

namespace SpectrumAnalyzer
{
    partial class SpectrumAnalysisControl
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
            pnlInfo = new Panel();
            richTextBox1 = new RichTextBox();
            lblInfo = new System.Windows.Forms.Label();
            panel2 = new Panel();
            panel1.SuspendLayout();
            pnlInfo.SuspendLayout();
            panel2.SuspendLayout();
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
            spectrumPlots.Size = new Size(904, 591);
            spectrumPlots.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(pnlInfo);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(124, 591);
            panel1.TabIndex = 1;
            // 
            // pnlInfo
            // 
            pnlInfo.BackColor = SystemColors.GradientActiveCaption;
            pnlInfo.BorderStyle = BorderStyle.FixedSingle;
            pnlInfo.Controls.Add(richTextBox1);
            pnlInfo.Controls.Add(lblInfo);
            pnlInfo.Dock = DockStyle.Fill;
            pnlInfo.Location = new Point(0, 0);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(124, 591);
            pnlInfo.TabIndex = 0;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(3, 32);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(117, 268);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "Shift-scroll to expand vertically.\nCtrl-scroll to expand horizontally.\nClick-drag to zoom.";
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(3, 14);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(70, 15);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "Information";
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(spectrumPlots);
            panel2.Location = new Point(129, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(877, 591);
            panel2.TabIndex = 2;
            // 
            // SpecrumAnalysisControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel1);
            Controls.Add(panel2);
            Name = "SpecrumAnalysisControl";
            Size = new Size(1006, 591);
            panel1.ResumeLayout(false);
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
            panel2.ResumeLayout(false);
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
        private Panel pnlInfo;
        private RichTextBox richTextBox1;
        private System.Windows.Forms.Label lblInfo;
    }
}
