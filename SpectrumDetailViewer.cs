using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace SpectrumAnalyzer
{
    public partial class SpectrumDetailViewer : Form
    {
        public SpectrumDetailViewer()
        {
            InitializeComponent();
        }

        public SpectrumAnalysisControl SpectrumAnalysisControl => spectrumAnalysisControl1;

        // private string selectedFileName = "";

        /*
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
                    spectrumAnalysisControl1.SetAudioFileSource(fileReader);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}", ex.Message), "Error Loading File");
                    return;
                }
            }
        }
        private AudioFileReader fileReader;
        */

        private IWaveProvider? waveProvider;

        private IWavePlayer? wavePlayer;


        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (spectrumAnalysisControl1.Data != null &&
                spectrumAnalysisControl1.Data.Length > 0)
            {
                if (wavePlayer != null)
                {
                    if (wavePlayer.PlaybackState == PlaybackState.Playing)
                    {
                        return;
                    }
                    else if (wavePlayer.PlaybackState == PlaybackState.Paused)
                    {
                        wavePlayer.Play();
                        return;
                    }
                }
                try
                {
                    CreateWaveOut();
                }
                catch (Exception driverCreateException)
                {
                    MessageBox.Show(String.Format("{0}", driverCreateException.Message));
                    return;
                }

                /*
                ISampleProvider sampleProvider = spectrumAnalysisControl1.WaveStream;
                try
                {
                    sampleProvider = CreateInputStream(fileName);
                }
                catch (Exception createException)
                {
                    MessageBox.Show(String.Format("{0}", createException.Message), "Error Loading File");
                    return;
                }
                */

                //labelTotalTime.Text = String.Format("{0:00}:{1:00}", (int)fileReader.TotalTime.TotalMinutes, fileReader.TotalTime.Seconds);

                try
                {
                    wavePlayer?.Init(waveProvider);
                    // we don't necessarily know the output format until we have initialized
                    //textBoxPlaybackFormat.Text = $"{wavePlayer.OutputWaveFormat}";
                }
                catch (Exception initException)
                {
                    MessageBox.Show(String.Format("{0}", initException.Message), "Error Initializing Output");
                    return;
                }
                timer1.Enabled = true;
                wavePlayer?.Play();
            }
        }
        private void CreateWaveOut()
        {
            CloseWaveOut();
            var latency = SpectrumAnalysisControl.FFTWindowSize;
            wavePlayer = CreateWaveOutDevice(latency);
            wavePlayer?.PlaybackStopped += OnPlaybackStopped;
            waveProvider = spectrumAnalysisControl1.WaveStream as IWaveProvider;
        }

        private IWavePlayer? CreateWaveOutDevice(int latency)
        {
            var waveOut = new WaveOutEvent();
            waveOut.DeviceNumber = 0; // default?
            waveOut.DesiredLatency = latency;
            return waveOut;
        }

        void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(e.Exception.Message, "Playback Device Error");
            }
            if (waveProvider is Stream)
            {
                (waveProvider as Stream)!.Position = 0;
            }
        }

        private void CloseWaveOut()
        {
            if (wavePlayer != null)
            {
                wavePlayer.Stop();
                wavePlayer.Dispose();
                wavePlayer = null;
            }
        }


        private void btnPause_Click(object sender, EventArgs e)
        {
            if (wavePlayer != null)
            {
                if (wavePlayer.PlaybackState == PlaybackState.Playing)
                {
                    wavePlayer.Pause();
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e) => wavePlayer?.Stop();

        private void timer1_Tick(object sender, EventArgs e)
        {
            spectrumAnalysisControl1.UpdateFilePosition();
            if (wavePlayer != null && waveProvider is WaveStream)
            {
                TimeSpan currentTime = (wavePlayer.PlaybackState == PlaybackState.Stopped) ? TimeSpan.Zero : (waveProvider as  WaveStream)!.CurrentTime;
                toolStripLabelCurrentTime.Text = String.Format("{0:00}:{1:00.00}", (int)currentTime.TotalMinutes,
                    currentTime.Seconds);
            }
        }
    }
}
