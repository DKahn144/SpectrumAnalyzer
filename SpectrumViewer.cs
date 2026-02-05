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
                    var reader = new AudioFileReader(openFileDialog.FileName);
                    lblFileInfo.Text = reader.WaveFormat.ToString();
                    lblTotalTime.Text = reader.TotalTime.ToString();
                    selectedFileName = openFileDialog.FileName;
                    tbxCurrentFile.Text = $"{Path.GetFileName(selectedFileName)}";
                    fileReader = reader;
                    specrumAnalysisControl1.SetAudioFileSource(fileReader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}", ex.Message), "Error Loading File");
                    return;
                }
            }
        }

        private AudioFileReader fileReader;

        private void btnPlay_Click(object sender, EventArgs e)
        {

        }

        private void specrumAnalysisControl1_Load(object sender, EventArgs e)
        {

        }

        public AudioFileReader FileReader => fileReader;
    }
}
