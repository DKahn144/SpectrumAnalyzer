using NAudio.Wave;

namespace SpectrumAnalyzer
{
    public partial class SpectrumViewer : Form
    {
        public SpectrumViewer()
        {
            InitializeComponent();
        }

        private string selectedFileName;

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            string allExtensions = "Wave files | *.wav";
            openFileDialog.Filter = allExtensions;
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    selectedFileName = openFileDialog.FileName;
                    tbxCurrentFile.Text = $"{Path.GetFileName(selectedFileName)}";
                    fileReader = new AudioFileReader(selectedFileName);
                    lblFileInfo.Text = fileReader.WaveFormat.ToString();
                    lblTotalTime.Text = fileReader.TotalTime.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}", ex.Message), "Error Loading File");
                    return;
                }
                specrumAnalysisControl1.SampleProvider = fileReader;
            }
        }

        private AudioFileReader fileReader;
        public AudioFileReader FileReader => fileReader;
    }
}
