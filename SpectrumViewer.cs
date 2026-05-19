using NAudio.Wave;
using SpectrumProcessor;

namespace SpectrumAnalyzer
{
    public partial class SpectrumViewer : Form
    {
        public SpectrumViewer()
        {
            InitializeComponent();
            this.Width = 1600;
            this.Height = 900;
            var selectionMgr = new SelectionManager(this.spectrumAnalysisControl1);
            selectionMgr.RegisterSelectionMethod(new RectangleSelector() { Manager = selectionMgr });
            selectionMgr.RegisterSelectionMethod(new SegmentSelector() { Manager = selectionMgr });
            selectionMgr.RegisterSelectionMethod(new SignalInspector() { Manager = selectionMgr });
            selectionMgr.SelectionMethodChanged += this.spectrumAnalysisControl1.OnSelectionMethodChanged;
        }

        private string selectedFileName;

        private static SynchronizationContext? _syncContext;

        private void SpectrumViewer_Load(object sender, EventArgs e)
        {
            _syncContext = WindowsFormsSynchronizationContext.Current;
        }

        /// <summary>
        /// This is used to run functions such as Refresh on the UI thread to avoid cross-thread
        /// exceptions. If the synchronization context is not available, it will run the action 
        /// directly, which may cause issues if called from a non-UI thread.
        /// </summary>
        /// <param name="action"></param>
        public static void RunInUIContext(Action action)
        {
            if (_syncContext != null)
            {
                _syncContext.Post(_ => action(), null);
            }
            else
            {
                action();
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            btnStop_Click(sender, e);
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
                    this.Cursor = Cursors.WaitCursor;
                    //    .UseWaitCursor = true;
                    this.Refresh();
                    waveReader = new AudioFileSampleWaveStream(openFileDialog.FileName);
                    SetAudioWaveSource(waveReader, openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}", ex.Message), "Error Loading File");
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                    RunInUIContext(this.Refresh);
                }
            }
        }

        public void SetAudioWaveSource(AudioFileSampleWaveStream waveStr, SpectrumData? newData)
        {
            if (waveStr == null)
            {
                throw new ArgumentNullException("SampleWaveStream parameter is required in SetAudioWaveSource()");
            }
            if (newData == null)
            {
                throw new ArgumentNullException("SpectrumData parameter is required in SetAudioWaveSource()");
            }

            selectedFileName = waveStr.FileName;
            waveReader = waveStr;
            UpdateControls(newData);
        }

        public void SetAudioWaveSource(SampleWaveStream waveStr, string fileName = "")
        {
            if (waveStr == null)
            {
                throw new ArgumentNullException("SampleWaveStream parameter is required in SetAudioWaveSource()");
            }
            selectedFileName = fileName;
            waveReader = waveStr;
            var newData = new SpectrumData(waveReader, spectrumAnalysisControl1.FFTWindowSize, selectedFileName);
            UpdateControls(newData);
        }

        private void UpdateControls(SpectrumData newData)
        {
            tbxCurrentFile.Text = $"{Path.GetFileName(selectedFileName)}";
            lblFileInfo.Text = waveReader.WaveFormat.ToString();
            lblTotalTime.Text = waveReader.TotalTime.ToString().PadRight(12);
            toolStripLabelCurrentTime.Text = waveReader.CurrentTime.ToString().PadRight(12);
            spectrumAnalysisControl1.SetAudioFileSource(waveReader, newData);
            ClearWavePlayer();
        }

        private SampleWaveStream? waveReader;
        private AudioPlayer? wavePlayer = null;

        private void ClearWavePlayer()
        {
            if (wavePlayer != null)
            {
                wavePlayer.Dispose();
                wavePlayer = null;
            }
        }

        public AudioFileSampleWaveStream? FileReader => waveReader as AudioFileSampleWaveStream;

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (spectrumAnalysisControl1.Data != null &&
                spectrumAnalysisControl1.Data.FftCount > 0 &&
                waveReader != null)
            {
                timer1.Enabled = true;
                if (wavePlayer != null)
                {
                    if (wavePlayer.PlaybackState == PlaybackState.Stopped)
                    {
                        ClearWavePlayer();
                    }
                    else
                    {
                        wavePlayer?.Play();
                        return;
                    }
                }
                wavePlayer = new AudioPlayer(this);
                wavePlayer.Init(waveReader, spectrumAnalysisControl1.Data.BufferMultiplier);
                wavePlayer?.Play();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            wavePlayer?.Pause();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            wavePlayer?.Stop();
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (wavePlayer != null && waveReader != null)
            {
                TimeSpan currentTime = (wavePlayer.PlaybackState == PlaybackState.Stopped) ? TimeSpan.Zero : waveReader.CurrentTime;
                toolStripLabelCurrentTime.Text = String.Format("{0:00}:{1:00}.{00}",
                    (int)currentTime.TotalMinutes,
                    currentTime.Seconds,
                    currentTime.Milliseconds / 10);
            }
            spectrumAnalysisControl1.UpdateFilePosition();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            var helpForm = new HelpForm();
            helpForm.ShowDialog();
        }
    }
}
