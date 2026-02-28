using FftSharp;
using NAudio.Dsp;
using NAudio.Wave;
using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpectrumAnalyzer
{
    public class SpectrumData : RawSourceWaveStream
    {
        /// <summary>
        /// complete array of sample values
        /// </summary>
        public float[] SampleValues => sampleValues;

        /// <summary>
        /// Signal magnitudes by frequency.
        /// </summary>
        public ValueMeasure[] FrequencyMags => frequencyMags;

        /// <summary>
        /// Signal magnitudes by frequency.
        /// </summary>
        public ValueMeasure[] FrequencyPower => frequencyPower;

        /// <summary>
        /// Signal magnitude series over time.
        /// </summary>
        public ValueMeasure[] TimeMags => timeMags;

        public override WaveFormat WaveFormat => sourceStream.WaveFormat;

        public System.Numerics.Complex[,] FftColumns => fftColumns;

        /// <summary>
        /// number of samples between each FFT measurement and vertical height of FFT array
        /// </summary>
        public int FftWindowSize => fftWindowSize;

        /// <summary>
        /// Number of samples in each buffer read
        /// </summary>
        public int BufferSize => bufferSize;

        public string FileName => fileName;

        /// <summary>
        /// Current number of entries into the FFT array
        /// </summary>
        public long FftIndex => fftIndex;

        public float HertzFactor => getHertzFactor();

        public double SecondsPerBuffer => getSecondsPerBuffer();

        public int FftCount => (int) sourceStream.Length / (fftWindowSize * bytesPerSampleTot);

        private float hertzFactor = 0;
        private float getHertzFactor()
        {
            if (hertzFactor == 0 && fftWindowSize > 0)
            {
                hertzFactor = (float) WaveFormat.SampleRate / (fftWindowSize * 2);
            }
            return hertzFactor;
        }
        private double secondsPerBuffer = 0;
        private double getSecondsPerBuffer()
        {
            if (secondsPerBuffer == 0 && fftWindowSize > 0)
            {
                secondsPerBuffer = ((double) fftWindowSize) / WaveFormat.SampleRate;
            }
            return secondsPerBuffer;
        }

        private WaveStream sourceStream;
        private int bufferSize = 0;
        private float sampleCount => (float) (sourceStream!.WaveFormat.SampleRate * sourceStream!.TotalTime.TotalSeconds);
        private long sampleReadCount = 0;
        private long lastFftPosition = 0;
        private int fftIndex = 0;
        private int fftWindowSize = 0;
        private ValueMeasure[] frequencyMags = new ValueMeasure[0];
        private ValueMeasure[] frequencyPower = new ValueMeasure[0];
        private ValueMeasure[] timeMags = new ValueMeasure[0];
        private float[] sampleValues = new float[0];
        private System.Numerics.Complex[,] fftColumns = new System.Numerics.Complex[0,0];
        private bool fftLoaded;
        private System.Numerics.Complex[,]? fftSelection;
        private int fftSelectionHeight = 0;
        private IWindow fftWindow = Window.GetWindows().First(w => w.Name == "Hanning");
        private FftRecords fftRecords;
        private string fileName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Audio source stream</param>
        /// <param name="_sampleCount">Total number of samples (audio values) expected</param>
        /// <param name="_bufferSize">Number of samples in one buffer read</param>
        /// <param name="_fftWindowSize">Number of samples to be fed into each FFT window</param>
        public SpectrumData(WaveStream source, string _fileName) 
            : base(source, source.WaveFormat)
        {
            sourceStream = source;
            fileName = _fileName;
            bufferSize = SpectrumAnalysisControl.FFTWindowSize;
            fftWindowSize = SpectrumAnalysisControl.FFTWindowSize;
            fftRecords = new FftRecords(this);
            ResetArrays();
        }

        public SpectrumData(WaveStream source, System.Numerics.Complex[,] fftSelection)
            : this(source)
        {
            this.fftSelection = fftSelection;
            this.fftLoaded = true;
            this.fftSelectionHeight = fftSelection.GetLength(1);
        }

        private void ResetArrays()
        {
            if (!fftLoaded)
            {
                fftColumns = new System.Numerics.Complex[FftCount, fftWindowSize];
            }
            else
            {
                fftColumns = fftSelection!;
            }
            sampleValues = new float[(int) sampleCount];
            timeMags = new ValueMeasure[(int) (sampleCount / bufferSize)];
            frequencyMags = new ValueMeasure[fftWindowSize];
            fftIndex = 0;
            lastFftPosition = 0;
            hertzFactor = (float)WaveFormat.SampleRate / (fftWindowSize * 2);
            secondsPerBuffer = ((double)fftWindowSize) / WaveFormat.SampleRate;
        }

        public void OpenFileReader(string filePath)
        {
            sourceStream = new AudioFileReader(filePath);
        }

        public IWindow FftWindow { get { return fftWindow; } set { fftWindow = value; } }

        private int bytesPerSampleTot => sourceStream.WaveFormat.BitsPerSample * sourceStream.WaveFormat.Channels / 8;

        public bool FftLoaded => this.fftLoaded;

        public void ReadSource()
        {
            if (fftIndex >= FftCount)
                return;

            ResetArrays();
            byte[] buffer = new byte[bufferSize * bytesPerSampleTot];
            int bytesRead = 0;
            long totalBytesRead = 0;
            do
            {
                bytesRead = Read(buffer, 0, buffer.Length);
                totalBytesRead += bytesRead;
            } 
            while (bytesRead > 0);
            fftRecords.BuildCrossRefLinks();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //int readCount = (int) ((sampleCount - sampleReadCount < count) ? sampleCount - sampleReadCount : count);
            int bytesRead = sourceStream.Read(buffer, offset, count);
            // Process the read data into sampleValues and fftData here
            // Update timeMags and frequencyMags accordingly
            int samplesRead = bytesRead / bytesPerSampleTot;
            long lastReadPosition = sampleReadCount;
            long currenReadPosition = lastReadPosition;
            for (int i = 0; i < samplesRead; i++)
            {
                var m = offset + (i * bytesPerSampleTot);
                if (buffer.Length > m &&
                    sampleValues.Length > m &&
                    currenReadPosition < sampleValues.Length)
                    sampleValues[currenReadPosition++] = BitConverter.ToSingle(buffer, m);
                else
                {
                }
            }
            sampleReadCount = currenReadPosition;

            while (sampleReadCount - lastFftPosition >= bufferSize)
            {
                double[] doubles = GetSampleArray(sampleValues, lastFftPosition);
                System.Numerics.Complex[] fftColumn = new System.Numerics.Complex[fftWindowSize / WaveFormat.Channels];
                if (!fftLoaded)
                {
                    fftColumn = GenerateFFTForSamples(doubles, lastFftPosition);
                    fftIndex = AddFftColumn(fftColumn, fftIndex);
                }
                else
                {
                    if (fftIndex < fftSelection!.GetLength(0))
                    {
                        fftColumn = new System.Numerics.Complex[fftSelectionHeight];
                        for (int i = 0; i < fftSelectionHeight; i++) fftColumn[i] = fftSelection![fftIndex, i];
                    }
                }
                AddMagnitudes(fftColumn, fftIndex);
                var record = new FftRecord(fftRecords, fftColumn, doubles.Select(d => Convert.ToSingle(d)).ToArray(), fftIndex++);
                fftRecords.AddRecord(record);
                lastFftPosition += bufferSize;
            }
            return bytesRead;
        }

        private double[] GetSampleArray(float[] sampleValues, long startPosition)
        {
            double[] doubles = new double[fftWindowSize / WaveFormat.Channels];
            for (int j = 0; j < doubles.Length; j++)
            {
                doubles[j] = Convert.ToDouble(sampleValues[startPosition + (j * WaveFormat.Channels)]);
            }
            return doubles;
        }

        public int AddFftColumn(System.Numerics.Complex[] fftColumn, int index)
        {
            for (int j = 0; j < fftColumn.Length; j++)
            {
                fftColumns[fftIndex, j] = fftColumn[j];
            }
            //lastFftPosition += fftWindowSize;
            return index++;
        }

        private System.Numerics.Complex[] GenerateFFTForSamples(double[] samples, long lastFftPosition)
        {
            // Convert the relevant segment of sampleValues to double[]
            // Calculate the FFT as an array of complex numbers
            FftWindow.ApplyInPlace(samples);
            // Calculate the FFT as an array of complex numbers
            System.Numerics.Complex[] fftColumn = FftSharp.FFT.Forward(samples);
            for (int j = 0; j < fftColumn.Length; j++)
            {
                fftColumns[fftIndex, j] = fftColumn[j];
            }
            return fftColumn;
        }

        public System.Numerics.Complex[] PerformFFTInPlace(System.Numerics.Complex[] samples)
        {
            var doubles = samples.Select(s => s.Magnitude).ToArray();
            FftWindow.ApplyInPlace(doubles);
            System.Numerics.Complex[] sampleColumn = FftSharp.FFT.Forward(doubles);
            return sampleColumn;
        }

        private void AddMagnitudes(System.Numerics.Complex[] fftCol, int fftInd)
        {
            // or get the magnitude (units²) or power (dB) as real numbers
            double[] magnitude = FftSharp.FFT.Magnitude(fftCol, false);
            float totalMag = 0;
            for (int j = 0; j < magnitude.Length && j < frequencyMags.Length; j++)
            {
                // Update frequency mags
                if (frequencyMags[j] == null)
                    frequencyMags[j] = new ValueMeasure();
                frequencyMags[j].AddValue(fftInd, (float)magnitude[j]);
                totalMag += (float)magnitude[j];
            }
            if (timeMags.Length > fftInd)
            {
                if (timeMags[fftInd] == null)
                    timeMags[fftInd] = new ValueMeasure();
                timeMags[fftInd].AddValue(0, totalMag);
            }
        }

        /// <summary>
        /// get averages over this sample
        /// </summary>
        /// <returns>average frequency, average volume</returns>
        internal (float, float) GetWeightedAvgFreq()
        {
            return fftRecords.GetAvgFreqPower();
        }
    }
}
