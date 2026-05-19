namespace SpectrumAnalyzer
{
    partial class HelpForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            rtbHelpText = new RichTextBox();
            SuspendLayout();
            // 
            // rtbHelpText
            // 
            rtbHelpText.BorderStyle = BorderStyle.FixedSingle;
            rtbHelpText.Location = new Point(12, 12);
            rtbHelpText.Name = "rtbHelpText";
            rtbHelpText.ReadOnly = true;
            rtbHelpText.Size = new Size(686, 558);
            rtbHelpText.TabIndex = 0;
            rtbHelpText.Text = "The Help info for the Spectrum Analyzer";
            // 
            // HelpForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(710, 582);
            Controls.Add(rtbHelpText);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "HelpForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Spectrum Analyzer Help";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox rtbHelpText;
    }
}