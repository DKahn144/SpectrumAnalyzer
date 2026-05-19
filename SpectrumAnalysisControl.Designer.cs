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

        protected override void OnGotFocus(EventArgs e)
        {
            if (lbxSelection.Items.Count > 0 && lbxSelection.SelectedIndex == -1)
            {
                lbxSelection.SelectedIndex = 0;
            }
            base.OnGotFocus(e);
            
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpectrumAnalysisControl));
            spectrumPlots = new ScottPlot.WinForms.FormsPlot();
            pnlInfo = new Panel();
            cbxFFTWindow = new ComboBox();
            cbxShowSignal = new CheckBox();
            lbxSelection = new ListBox();
            cbxSelectionMethod = new ComboBox();
            btnSaveWaveFile = new Button();
            btnSaveData = new Button();
            cbxSmooth = new CheckBox();
            tbxAvgVol = new TextBox();
            lblAvgVol = new System.Windows.Forms.Label();
            tbxAvgFreq = new TextBox();
            btnClearAnalysis = new Button();
            btnAnalyze = new Button();
            lblSelectionMethod = new System.Windows.Forms.Label();
            lblSelection = new System.Windows.Forms.Label();
            tbxMouseY = new TextBox();
            tbxMouseHz = new TextBox();
            tbxMouseX = new TextBox();
            label4 = new System.Windows.Forms.Label();
            tbxMouseTime = new TextBox();
            label2 = new System.Windows.Forms.Label();
            lblAvgFreq = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            lblFFTWindow = new System.Windows.Forms.Label();
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
            spectrumPlots.Location = new Point(151, 0);
            spectrumPlots.Margin = new Padding(2);
            spectrumPlots.Name = "spectrumPlots";
            spectrumPlots.Size = new Size(1363, 797);
            spectrumPlots.TabIndex = 0;
            // 
            // pnlInfo
            // 
            pnlInfo.BackColor = SystemColors.GradientActiveCaption;
            pnlInfo.BorderStyle = BorderStyle.FixedSingle;
            pnlInfo.Controls.Add(cbxFFTWindow);
            pnlInfo.Controls.Add(cbxShowSignal);
            pnlInfo.Controls.Add(lbxSelection);
            pnlInfo.Controls.Add(cbxSelectionMethod);
            pnlInfo.Controls.Add(btnSaveWaveFile);
            pnlInfo.Controls.Add(btnSaveData);
            pnlInfo.Controls.Add(cbxSmooth);
            pnlInfo.Controls.Add(tbxAvgVol);
            pnlInfo.Controls.Add(lblAvgVol);
            pnlInfo.Controls.Add(tbxAvgFreq);
            pnlInfo.Controls.Add(btnClearAnalysis);
            pnlInfo.Controls.Add(btnAnalyze);
            pnlInfo.Controls.Add(lblSelectionMethod);
            pnlInfo.Controls.Add(lblSelection);
            pnlInfo.Controls.Add(tbxMouseY);
            pnlInfo.Controls.Add(tbxMouseHz);
            pnlInfo.Controls.Add(tbxMouseX);
            pnlInfo.Controls.Add(label4);
            pnlInfo.Controls.Add(tbxMouseTime);
            pnlInfo.Controls.Add(label2);
            pnlInfo.Controls.Add(lblAvgFreq);
            pnlInfo.Controls.Add(label3);
            pnlInfo.Controls.Add(lblFFTWindow);
            pnlInfo.Controls.Add(label1);
            pnlInfo.Controls.Add(richTextBox1);
            pnlInfo.Controls.Add(lblInfo);
            pnlInfo.Dock = DockStyle.Left;
            pnlInfo.Location = new Point(0, 0);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(151, 797);
            pnlInfo.TabIndex = 0;
            // 
            // cbxFFTWindow
            // 
            cbxFFTWindow.FormattingEnabled = true;
            cbxFFTWindow.Location = new Point(86, 217);
            cbxFFTWindow.Name = "cbxFFTWindow";
            cbxFFTWindow.Size = new Size(55, 23);
            cbxFFTWindow.TabIndex = 24;
            cbxFFTWindow.SelectedIndexChanged += cbxFFTWindow_SelectedIndexChanged;
            // 
            // cbxShowSignal
            // 
            cbxShowSignal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbxShowSignal.AutoSize = true;
            cbxShowSignal.Location = new Point(21, 681);
            cbxShowSignal.Name = "cbxShowSignal";
            cbxShowSignal.Size = new Size(90, 19);
            cbxShowSignal.TabIndex = 23;
            cbxShowSignal.Text = "Show Signal";
            cbxShowSignal.UseVisualStyleBackColor = true;
            cbxShowSignal.CheckedChanged += cbxShowSignal_CheckedChanged;
            // 
            // lbxSelection
            // 
            lbxSelection.FormattingEnabled = true;
            lbxSelection.Location = new Point(11, 509);
            lbxSelection.Name = "lbxSelection";
            lbxSelection.Size = new Size(130, 79);
            lbxSelection.TabIndex = 22;
            // 
            // cbxSelectionMethod
            // 
            cbxSelectionMethod.FormattingEnabled = true;
            cbxSelectionMethod.Location = new Point(11, 464);
            cbxSelectionMethod.Name = "cbxSelectionMethod";
            cbxSelectionMethod.Size = new Size(130, 23);
            cbxSelectionMethod.TabIndex = 21;
            cbxSelectionMethod.SelectedIndexChanged += cbxSelectionMethod_SelectedIndexChanged;
            cbxSelectionMethod.VisibleChanged += cbxSelectionMethod_VisibleChanged;
            cbxSelectionMethod.GotFocus += CbxSelectionMethod_GotFocus;
            // 
            // btnSaveWaveFile
            // 
            btnSaveWaveFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSaveWaveFile.ForeColor = SystemColors.ActiveCaptionText;
            btnSaveWaveFile.Location = new Point(18, 740);
            btnSaveWaveFile.Name = "btnSaveWaveFile";
            btnSaveWaveFile.Size = new Size(113, 23);
            btnSaveWaveFile.TabIndex = 14;
            btnSaveWaveFile.Text = "Save to wave file";
            btnSaveWaveFile.UseVisualStyleBackColor = true;
            btnSaveWaveFile.Click += btnSaveWaveFile_Click;
            // 
            // btnSaveData
            // 
            btnSaveData.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSaveData.ForeColor = SystemColors.ActiveCaptionText;
            btnSaveData.Location = new Point(18, 769);
            btnSaveData.Name = "btnSaveData";
            btnSaveData.Size = new Size(113, 23);
            btnSaveData.TabIndex = 14;
            btnSaveData.Text = "Save to data file";
            btnSaveData.UseVisualStyleBackColor = true;
            btnSaveData.Click += btnSaveData_Click;
            // 
            // cbxSmooth
            // 
            cbxSmooth.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cbxSmooth.AutoSize = true;
            cbxSmooth.Location = new Point(21, 706);
            cbxSmooth.Name = "cbxSmooth";
            cbxSmooth.Size = new Size(68, 19);
            cbxSmooth.TabIndex = 13;
            cbxSmooth.Text = "Smooth";
            cbxSmooth.UseVisualStyleBackColor = true;
            cbxSmooth.CheckedChanged += cbxSmooth_CheckedChanged;
            // 
            // tbxAvgVol
            // 
            tbxAvgVol.Location = new Point(97, 278);
            tbxAvgVol.Name = "tbxAvgVol";
            tbxAvgVol.ReadOnly = true;
            tbxAvgVol.Size = new Size(44, 23);
            tbxAvgVol.TabIndex = 11;
            // 
            // lblAvgVol
            // 
            lblAvgVol.AutoSize = true;
            lblAvgVol.Location = new Point(6, 281);
            lblAvgVol.Name = "lblAvgVol";
            lblAvgVol.Size = new Size(50, 15);
            lblAvgVol.TabIndex = 10;
            lblAvgVol.Text = "Avg Vol:";
            // 
            // tbxAvgFreq
            // 
            tbxAvgFreq.Location = new Point(97, 249);
            tbxAvgFreq.Name = "tbxAvgFreq";
            tbxAvgFreq.ReadOnly = true;
            tbxAvgFreq.Size = new Size(44, 23);
            tbxAvgFreq.TabIndex = 9;
            // 
            // btnClearAnalysis
            // 
            btnClearAnalysis.Enabled = false;
            btnClearAnalysis.ForeColor = SystemColors.ControlDarkDark;
            btnClearAnalysis.Location = new Point(18, 630);
            btnClearAnalysis.Name = "btnClearAnalysis";
            btnClearAnalysis.Size = new Size(114, 23);
            btnClearAnalysis.TabIndex = 8;
            btnClearAnalysis.Text = "Clear";
            btnClearAnalysis.UseVisualStyleBackColor = true;
            btnClearAnalysis.Click += btnClearAnalysis_Click;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Enabled = false;
            btnAnalyze.Font = new Font("Segoe UI", 9F);
            btnAnalyze.ForeColor = SystemColors.ControlDarkDark;
            btnAnalyze.Location = new Point(18, 601);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(114, 23);
            btnAnalyze.TabIndex = 7;
            btnAnalyze.Text = "Analyze";
            btnAnalyze.UseVisualStyleBackColor = true;
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // lblSelectionMethod
            // 
            lblSelectionMethod.AutoSize = true;
            lblSelectionMethod.Location = new Point(11, 446);
            lblSelectionMethod.Name = "lblSelectionMethod";
            lblSelectionMethod.Size = new Size(100, 15);
            lblSelectionMethod.TabIndex = 5;
            lblSelectionMethod.Text = "Selection Method";
            // 
            // lblSelection
            // 
            lblSelection.AutoSize = true;
            lblSelection.Location = new Point(11, 491);
            lblSelection.Name = "lblSelection";
            lblSelection.Size = new Size(58, 15);
            lblSelection.TabIndex = 5;
            lblSelection.Text = "Selection:";
            // 
            // tbxMouseY
            // 
            tbxMouseY.BackColor = SystemColors.ControlLightLight;
            tbxMouseY.Location = new Point(97, 405);
            tbxMouseY.Name = "tbxMouseY";
            tbxMouseY.ReadOnly = true;
            tbxMouseY.Size = new Size(44, 23);
            tbxMouseY.TabIndex = 4;
            tbxMouseY.Text = "0";
            // 
            // tbxMouseHz
            // 
            tbxMouseHz.BackColor = SystemColors.ControlLightLight;
            tbxMouseHz.Location = new Point(97, 341);
            tbxMouseHz.Name = "tbxMouseHz";
            tbxMouseHz.ReadOnly = true;
            tbxMouseHz.Size = new Size(44, 23);
            tbxMouseHz.TabIndex = 4;
            tbxMouseHz.Text = "0";
            // 
            // tbxMouseX
            // 
            tbxMouseX.BackColor = SystemColors.ControlLightLight;
            tbxMouseX.Location = new Point(97, 376);
            tbxMouseX.Name = "tbxMouseX";
            tbxMouseX.ReadOnly = true;
            tbxMouseX.Size = new Size(44, 23);
            tbxMouseX.TabIndex = 3;
            tbxMouseX.Text = "0";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 408);
            label4.Name = "label4";
            label4.Size = new Size(53, 15);
            label4.TabIndex = 2;
            label4.Text = "Mouse Y";
            // 
            // tbxMouseTime
            // 
            tbxMouseTime.BackColor = SystemColors.ControlLightLight;
            tbxMouseTime.Location = new Point(97, 312);
            tbxMouseTime.Name = "tbxMouseTime";
            tbxMouseTime.ReadOnly = true;
            tbxMouseTime.Size = new Size(44, 23);
            tbxMouseTime.TabIndex = 3;
            tbxMouseTime.Text = "0";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 344);
            label2.Name = "label2";
            label2.Size = new Size(63, 15);
            label2.TabIndex = 2;
            label2.Text = "Mouse Hz:";
            // 
            // lblAvgFreq
            // 
            lblAvgFreq.AutoSize = true;
            lblAvgFreq.Location = new Point(6, 252);
            lblAvgFreq.Name = "lblAvgFreq";
            lblAvgFreq.Size = new Size(57, 15);
            lblAvgFreq.TabIndex = 2;
            lblAvgFreq.Text = "Avg Freq:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 379);
            label3.Name = "label3";
            label3.Size = new Size(53, 15);
            label3.TabIndex = 2;
            label3.Text = "Mouse X";
            // 
            // lblFFTWindow
            // 
            lblFFTWindow.AutoSize = true;
            lblFFTWindow.Location = new Point(6, 220);
            lblFFTWindow.Name = "lblFFTWindow";
            lblFFTWindow.Size = new Size(74, 15);
            lblFFTWindow.TabIndex = 2;
            lblFFTWindow.Text = "FFT window:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 315);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 2;
            label1.Text = "Mouse Time:";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(3, 32);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(141, 175);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = resources.GetString("richTextBox1.Text");
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
            Size = new Size(1514, 797);
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
            ResumeLayout(false);
        }

        private void CbxSelectionMethod_GotFocus(object sender, EventArgs e)
        {
            cbxSelectionMethod.ForeColor = SystemColors.ControlText;
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
        private System.Windows.Forms.Label lblSelection;
        private Button btnAnalyze;
        private ScottPlot.WinForms.FormsPlot formsPlot1;
        private Button button1;
        private Button btnClearAnalysis;
        private System.Windows.Forms.Label lblFFTWindow;
        private TextBox tbxAvgFreq;
        private System.Windows.Forms.Label lblAvgFreq;
        private System.Windows.Forms.Label lblAvgVol;
        private TextBox tbxAvgVol;
        private CheckBox cbxSmooth;
        private Button btnSaveData;
        private TextBox tbxMouseY;
        private TextBox tbxMouseX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private ComboBox cbxSelectionMethod;
        private System.Windows.Forms.Label lblSelectionMethod;
        private ListBox lbxSelection;
        private CheckBox cbxShowSignal;
        private Button btnSaveWaveFile;
        private ComboBox cbxFFTWindow;
    }
}
