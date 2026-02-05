namespace SpectrumAnalyzer
{
    partial class SpectrumViewer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpectrumViewer));
            PaneltoolStrip = new Panel();
            toolStrip1 = new ToolStrip();
            btnOpenFile = new ToolStripButton();
            btnPlay = new ToolStripButton();
            btnPause = new ToolStripButton();
            btnStop = new ToolStripButton();
            tbxCurrentFile = new ToolStripTextBox();
            lblFileInfo = new ToolStripLabel();
            toolStripSeparator1 = new ToolStripSeparator();
            lblTotalTime = new ToolStripLabel();
            panel1 = new Panel();
            specrumAnalysisControl1 = new SpecrumAnalysisControl();
            PaneltoolStrip.SuspendLayout();
            toolStrip1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // PaneltoolStrip
            // 
            PaneltoolStrip.BorderStyle = BorderStyle.FixedSingle;
            PaneltoolStrip.Controls.Add(toolStrip1);
            PaneltoolStrip.Dock = DockStyle.Top;
            PaneltoolStrip.Location = new Point(0, 0);
            PaneltoolStrip.Name = "PaneltoolStrip";
            PaneltoolStrip.Size = new Size(1156, 27);
            PaneltoolStrip.TabIndex = 7;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnOpenFile, btnPlay, btnPause, btnStop, tbxCurrentFile, lblFileInfo, toolStripSeparator1, lblTotalTime });
            toolStrip1.Location = new Point(6, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(375, 25);
            toolStrip1.TabIndex = 7;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnOpenFile
            // 
            btnOpenFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnOpenFile.Image = Properties.Resources.OpenFile;
            btnOpenFile.ImageTransparentColor = Color.Magenta;
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(23, 22);
            btnOpenFile.Text = "btnOpen";
            btnOpenFile.ToolTipText = "Open File";
            btnOpenFile.Click += btnOpenFile_Click;
            // 
            // btnPlay
            // 
            btnPlay.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnPlay.Image = Properties.Resources.Play;
            btnPlay.ImageTransparentColor = Color.Magenta;
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(23, 22);
            btnPlay.Text = "btnPlay";
            btnPlay.Click += btnPlay_Click;
            // 
            // btnPause
            // 
            btnPause.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnPause.Image = Properties.Resources.Pause;
            btnPause.ImageTransparentColor = Color.Magenta;
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(23, 22);
            btnPause.Text = "toolStripButton3";
            btnPause.ToolTipText = "btnPause";
            // 
            // btnStop
            // 
            btnStop.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnStop.Image = Properties.Resources.Stop;
            btnStop.ImageTransparentColor = Color.Magenta;
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(23, 22);
            btnStop.Text = "btnStop";
            btnStop.ToolTipText = "stop playing";
            // 
            // tbxCurrentFile
            // 
            tbxCurrentFile.Name = "tbxCurrentFile";
            tbxCurrentFile.Size = new Size(100, 25);
            tbxCurrentFile.Text = "(Current File)";
            tbxCurrentFile.ToolTipText = "selected file";
            // 
            // lblFileInfo
            // 
            lblFileInfo.Name = "lblFileInfo";
            lblFileInfo.Size = new Size(97, 22);
            lblFileInfo.Text = "(file information)";
            lblFileInfo.ToolTipText = "File information";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // lblTotalTime
            // 
            lblTotalTime.Name = "lblTotalTime";
            lblTotalTime.Size = new Size(66, 22);
            lblTotalTime.Text = "(total time)";
            lblTotalTime.ToolTipText = "total time";
            // 
            // panel1
            // 
            panel1.Controls.Add(specrumAnalysisControl1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 27);
            panel1.Name = "panel1";
            panel1.Size = new Size(1156, 561);
            panel1.TabIndex = 8;
            // 
            // specrumAnalysisControl1
            // 
            specrumAnalysisControl1.Dock = DockStyle.Fill;
            specrumAnalysisControl1.Location = new Point(0, 0);
            specrumAnalysisControl1.Name = "specrumAnalysisControl1";
            specrumAnalysisControl1.Size = new Size(1156, 561);
            specrumAnalysisControl1.TabIndex = 3;
            // 
            // SpectrumViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1156, 588);
            Controls.Add(panel1);
            Controls.Add(PaneltoolStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SpectrumViewer";
            Text = "Spectrum Analyzer";
            PaneltoolStrip.ResumeLayout(false);
            PaneltoolStrip.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel PaneltoolStrip;
        private ToolStrip toolStrip1;
        private ToolStripButton btnOpenFile;
        private ToolStripButton btnPlay;
        private ToolStripButton btnPause;
        private ToolStripButton btnStop;
        private ToolStripTextBox tbxCurrentFile;
        private ToolStripLabel lblFileInfo;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripLabel lblTotalTime;
        private Panel panel1;
        private SpecrumAnalysisControl specrumAnalysisControl1;
    }
}
