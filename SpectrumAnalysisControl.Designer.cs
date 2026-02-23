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
            pnlInfo = new Panel();
            btnAnalyze = new Button();
            tbxSelection = new TextBox();
            lblSelection = new System.Windows.Forms.Label();
            tbxMouseHz = new TextBox();
            tbxMouseTime = new TextBox();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            richTextBox1 = new RichTextBox();
            lblInfo = new System.Windows.Forms.Label();
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            pnlInfo.SuspendLayout();
            SuspendLayout();
            // 
            // spectrumPlots
            // 
            spectrumPlots.BackColor = SystemColors.ControlDark;
            spectrumPlots.DisplayScale = 1F;
            spectrumPlots.Dock = DockStyle.Fill;
            spectrumPlots.Location = new Point(132, 0);
            spectrumPlots.Margin = new Padding(2);
            spectrumPlots.Name = "spectrumPlots";
            spectrumPlots.Size = new Size(791, 552);
            spectrumPlots.TabIndex = 0;
            // 
            // pnlInfo
            // 
            pnlInfo.BackColor = SystemColors.GradientActiveCaption;
            pnlInfo.BorderStyle = BorderStyle.FixedSingle;
            pnlInfo.Controls.Add(btnAnalyze);
            pnlInfo.Controls.Add(tbxSelection);
            pnlInfo.Controls.Add(lblSelection);
            pnlInfo.Controls.Add(tbxMouseHz);
            pnlInfo.Controls.Add(tbxMouseTime);
            pnlInfo.Controls.Add(label2);
            pnlInfo.Controls.Add(label1);
            pnlInfo.Controls.Add(richTextBox1);
            pnlInfo.Controls.Add(lblInfo);
            pnlInfo.Dock = DockStyle.Left;
            pnlInfo.Location = new Point(0, 0);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(132, 552);
            pnlInfo.TabIndex = 0;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Location = new Point(26, 426);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(75, 23);
            btnAnalyze.TabIndex = 7;
            btnAnalyze.Text = "Analyze";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // tbxSelection
            // 
            tbxSelection.BackColor = SystemColors.ControlLightLight;
            tbxSelection.Location = new Point(3, 397);
            tbxSelection.Name = "tbxSelection";
            tbxSelection.ReadOnly = true;
            tbxSelection.Size = new Size(124, 23);
            tbxSelection.TabIndex = 6;
            // 
            // lblSelection
            // 
            lblSelection.AutoSize = true;
            lblSelection.Location = new Point(2, 379);
            lblSelection.Name = "lblSelection";
            lblSelection.Size = new Size(58, 15);
            lblSelection.TabIndex = 5;
            lblSelection.Text = "Selection:";
            // 
            // tbxMouseHz
            // 
            tbxMouseHz.BackColor = SystemColors.ControlLightLight;
            tbxMouseHz.Location = new Point(82, 343);
            tbxMouseHz.Name = "tbxMouseHz";
            tbxMouseHz.ReadOnly = true;
            tbxMouseHz.Size = new Size(44, 23);
            tbxMouseHz.TabIndex = 4;
            tbxMouseHz.Text = "0";
            // 
            // tbxMouseTime
            // 
            tbxMouseTime.BackColor = SystemColors.ControlLightLight;
            tbxMouseTime.Location = new Point(82, 318);
            tbxMouseTime.Name = "tbxMouseTime";
            tbxMouseTime.ReadOnly = true;
            tbxMouseTime.Size = new Size(44, 23);
            tbxMouseTime.TabIndex = 3;
            tbxMouseTime.Text = "0";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 346);
            label2.Name = "label2";
            label2.Size = new Size(63, 15);
            label2.TabIndex = 2;
            label2.Text = "Mouse Hz:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 321);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 2;
            label1.Text = "Mouse Time:";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(3, 32);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(123, 268);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "Shift-scroll to expand vertically.\nCtrl-scroll to expand horizontally.\nClick-drag to zoom.\nAlt-drag to select analysis rectangle.\n";
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
            // formsPlot1
            // 
            formsPlot1.DisplayScale = 1F;
            formsPlot1.Location = new Point(103, 96);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(806, 431);
            formsPlot1.TabIndex = 2;
            // 
            // SpectrumAnalysisControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(spectrumPlots);
            Controls.Add(pnlInfo);
            Name = "SpectrumAnalysisControl";
            Size = new Size(923, 552);
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
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
        private Panel pnlInfo;
        private RichTextBox richTextBox1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private TextBox tbxMouseTime;
        private TextBox tbxMouseHz;
        private TextBox tbxSelection;
        private System.Windows.Forms.Label lblSelection;
        private Button btnAnalyze;
        private ScottPlot.WinForms.FormsPlot formsPlot1;
    }
}
