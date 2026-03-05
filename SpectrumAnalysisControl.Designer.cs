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
            btnShowSignal = new Button();
            tbxAvgVol = new TextBox();
            lblAvgVol = new System.Windows.Forms.Label();
            tbxAvgFreq = new TextBox();
            btnClearAnalysis = new Button();
            btnAnalyze = new Button();
            tbxSelection = new TextBox();
            lblSelection = new System.Windows.Forms.Label();
            tbxMouseHz = new TextBox();
            tbxFFTWindowSize = new TextBox();
            tbxMouseTime = new TextBox();
            label2 = new System.Windows.Forms.Label();
            lblAvgFreq = new System.Windows.Forms.Label();
            lblFFTWindow = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            richTextBox1 = new RichTextBox();
            lblInfo = new System.Windows.Forms.Label();
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            cbxSmooth = new CheckBox();
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
            spectrumPlots.Size = new Size(791, 625);
            spectrumPlots.TabIndex = 0;
            // 
            // pnlInfo
            // 
            pnlInfo.BackColor = SystemColors.GradientActiveCaption;
            pnlInfo.BorderStyle = BorderStyle.FixedSingle;
            pnlInfo.Controls.Add(cbxSmooth);
            pnlInfo.Controls.Add(btnShowSignal);
            pnlInfo.Controls.Add(tbxAvgVol);
            pnlInfo.Controls.Add(lblAvgVol);
            pnlInfo.Controls.Add(tbxAvgFreq);
            pnlInfo.Controls.Add(btnClearAnalysis);
            pnlInfo.Controls.Add(btnAnalyze);
            pnlInfo.Controls.Add(tbxSelection);
            pnlInfo.Controls.Add(lblSelection);
            pnlInfo.Controls.Add(tbxMouseHz);
            pnlInfo.Controls.Add(tbxFFTWindowSize);
            pnlInfo.Controls.Add(tbxMouseTime);
            pnlInfo.Controls.Add(label2);
            pnlInfo.Controls.Add(lblAvgFreq);
            pnlInfo.Controls.Add(lblFFTWindow);
            pnlInfo.Controls.Add(label1);
            pnlInfo.Controls.Add(richTextBox1);
            pnlInfo.Controls.Add(lblInfo);
            pnlInfo.Dock = DockStyle.Left;
            pnlInfo.Location = new Point(0, 0);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(132, 625);
            pnlInfo.TabIndex = 0;
            // 
            // btnShowSignal
            // 
            btnShowSignal.Location = new Point(13, 528);
            btnShowSignal.Name = "btnShowSignal";
            btnShowSignal.Size = new Size(101, 23);
            btnShowSignal.TabIndex = 12;
            btnShowSignal.Text = "Show signal";
            btnShowSignal.UseVisualStyleBackColor = true;
            btnShowSignal.Click += btnShowSignal_Click;
            // 
            // tbxAvgVol
            // 
            tbxAvgVol.Location = new Point(82, 280);
            tbxAvgVol.Name = "tbxAvgVol";
            tbxAvgVol.Size = new Size(44, 23);
            tbxAvgVol.TabIndex = 11;
            // 
            // lblAvgVol
            // 
            lblAvgVol.AutoSize = true;
            lblAvgVol.Location = new Point(3, 283);
            lblAvgVol.Name = "lblAvgVol";
            lblAvgVol.Size = new Size(50, 15);
            lblAvgVol.TabIndex = 10;
            lblAvgVol.Text = "Avg Vol:";
            // 
            // tbxAvgFreq
            // 
            tbxAvgFreq.Location = new Point(82, 251);
            tbxAvgFreq.Name = "tbxAvgFreq";
            tbxAvgFreq.Size = new Size(44, 23);
            tbxAvgFreq.TabIndex = 9;
            // 
            // btnClearAnalysis
            // 
            btnClearAnalysis.Location = new Point(26, 455);
            btnClearAnalysis.Name = "btnClearAnalysis";
            btnClearAnalysis.Size = new Size(75, 23);
            btnClearAnalysis.TabIndex = 8;
            btnClearAnalysis.Text = "Clear";
            btnClearAnalysis.UseVisualStyleBackColor = true;
            btnClearAnalysis.Click += btnClearAnalysis_Click;
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
            // tbxFFTWindowSize
            // 
            tbxFFTWindowSize.BackColor = SystemColors.ControlLightLight;
            tbxFFTWindowSize.Location = new Point(82, 223);
            tbxFFTWindowSize.Name = "tbxFFTWindowSize";
            tbxFFTWindowSize.Size = new Size(44, 23);
            tbxFFTWindowSize.TabIndex = 3;
            tbxFFTWindowSize.Text = "1024";
            tbxFFTWindowSize.TextChanged += tbxFFTWindowSize_TextChanged;
            tbxFFTWindowSize.LostFocus += TbxFFTWindowSize_LostFocus;
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
            // lblAvgFreq
            // 
            lblAvgFreq.AutoSize = true;
            lblAvgFreq.Location = new Point(3, 254);
            lblAvgFreq.Name = "lblAvgFreq";
            lblAvgFreq.Size = new Size(57, 15);
            lblAvgFreq.TabIndex = 2;
            lblAvgFreq.Text = "Avg Freq:";
            // 
            // lblFFTWindow
            // 
            lblFFTWindow.AutoSize = true;
            lblFFTWindow.Location = new Point(3, 226);
            lblFFTWindow.Name = "lblFFTWindow";
            lblFFTWindow.Size = new Size(74, 15);
            lblFFTWindow.TabIndex = 2;
            lblFFTWindow.Text = "FFT window:";
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
            richTextBox1.Size = new Size(123, 181);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "Shift-scroll to expand vertically.\nCtrl-scroll to expand horizontally.\nClick-drag to zoom.\nAlt-drag to select analysis rectangle.\nAlt-shift-drag to select one frequency.\n";
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
            // cbxSmooth
            // 
            cbxSmooth.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbxSmooth.AutoSize = true;
            cbxSmooth.Location = new Point(26, 590);
            cbxSmooth.Name = "cbxSmooth";
            cbxSmooth.Size = new Size(68, 19);
            cbxSmooth.TabIndex = 13;
            cbxSmooth.Text = "Smooth";
            cbxSmooth.UseVisualStyleBackColor = true;
            cbxSmooth.CheckedChanged += cbxSmooth_CheckedChanged;
            // 
            // SpectrumAnalysisControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(spectrumPlots);
            Controls.Add(pnlInfo);
            Name = "SpectrumAnalysisControl";
            Size = new Size(923, 625);
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
        private Button button1;
        private Button btnClearAnalysis;
        private TextBox tbxFFTWindowSize;
        private System.Windows.Forms.Label lblFFTWindow;
        private TextBox tbxAvgFreq;
        private System.Windows.Forms.Label lblAvgFreq;
        private System.Windows.Forms.Label lblAvgVol;
        private TextBox tbxAvgVol;
        private Button btnShowSignal;
        private CheckBox cbxSmooth;
    }
}
