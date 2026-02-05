using FftSharp;
using NAudio.Dsp;
using NAudio.Wave;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpectrumAnalyzer
{
    internal class SpectrumData : RawSourceWaveStream
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

        /// <summary>
        /// Current number of entries into the FFT array
        /// </summary>
        public long FftCount => fftIndex;

        private WaveStream sourceStream;
        private int bufferSize = 0;
        private long sampleCount = 0;
        private long sampleReadCount = 0;
        private long lastFftPosition = 0;
        private int fftIndex = 0;
        private int fftWindowSize = 0;
        private ValueMeasure[] frequencyMags = new ValueMeasure[0];
        private ValueMeasure[] frequencyPower = new ValueMeasure[0];
        private ValueMeasure[] timeMags = new ValueMeasure[0];
        private float[] sampleValues = new float[0];
        private System.Numerics.Complex[,] fftColumns = new System.Numerics.Complex[0,0];
        private IWindow fftWindow = Window.GetWindows().First(w => w.Name == "Hanning");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Audio source stream</param>
        /// <param name="_sampleCount">Total number of samples (audio values) expected</param>
        /// <param name="_bufferSize">Number of samples in one buffer read</param>
        /// <param name="_fftWindowSize">Number of samples to be fed into each FFT window</param>
        public SpectrumData(WaveStream source, long _sampleCount, int _bufferSize = 1024, int _fftWindowSize = 1024) 
            : base(source, source.WaveFormat)
        {
            if (_sampleCount == 0 || _bufferSize == 0)
                throw new ArgumentOutOfRangeException("SpectrumData: Sample count and buffer size must be non-zero.");
            sourceStream = source;
            bufferSize = _bufferSize;
            sampleCount = _sampleCount;
            fftWindowSize = _fftWindowSize;
            ResetArrays();
        }

        private void ResetArrays()
        {
            fftColumns = new System.Numerics.Complex[sourceStream.Length / (fftWindowSize * sizeof(float)), fftWindowSize];
            sampleValues = new float[sampleCount];
            timeMags = new ValueMeasure[sampleCount / bufferSize];
            frequencyMags = new ValueMeasure[fftWindowSize];
            fftIndex = 0;
            lastFftPosition = 0;
        }

        public void OpenFileReader(string filePath)
        {
            sourceStream = new AudioFileReader(filePath);
        }

        public IWindow FftWindow { get { return fftWindow; } set { fftWindow = value; } }

        public void ReadSource()
        {
            byte[] buffer = new byte[bufferSize * sizeof(float)];
            int bytesRead = 0;
            do
            {
                bytesRead = Read(buffer, 0, buffer.Length);
            } while (bytesRead > 0);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //int readCount = (int) ((sampleCount - sampleReadCount < count) ? sampleCount - sampleReadCount : count);
            int bytesRead = sourceStream.Read(buffer, offset, count);
            // Process the read data into sampleValues and fftData here
            // Update timeMags and frequencyMags accordingly
            long lastReadPosition = sampleReadCount;
            long currenReadPosition = lastReadPosition;
            for (int i = 0; i < bytesRead /sizeof(float); i++)
            {
                var m = offset + (i * sizeof(float));
                if (buffer.Length > m &&
                    sampleValues.Length > m)
                    sampleValues[currenReadPosition++] = BitConverter.ToSingle(buffer, m);
                else
                {

                }
            }
            sampleReadCount = currenReadPosition;

            while (sampleReadCount - lastFftPosition >= fftWindowSize)
            {
                // convert to double
                var doubles = new double[fftWindowSize];
                for (int j = 0; j < fftWindowSize; j++)
                {
                    doubles[j] = Convert.ToDouble(sampleValues[lastFftPosition + j]);
                }

                FftWindow.ApplyInPlace(doubles);

                // Calculate the FFT as an array of complex numbers
                System.Numerics.Complex[] fftColumn = FftSharp.FFT.Forward(doubles);

                // or get the magnitude (units²) or power (dB) as real numbers
                double[] magnitude = FftSharp.FFT.Magnitude(fftColumn, false);
                float totalMag = 0;
                for (int j = 0; j < fftWindowSize; j++)
                {
                    fftColumns[fftIndex, j] = fftColumn[j];
                    // Update frequency mags
                    if (frequencyMags[j] == null)
                        frequencyMags[j] = new ValueMeasure();
                    frequencyMags[j].AddValue(fftIndex, (float)magnitude[j]);
                    totalMag += (float)magnitude[j];
                }
                if (timeMags[fftIndex] == null)
                    timeMags[fftIndex] = new ValueMeasure();
                timeMags[fftIndex].AddValue(fftIndex, totalMag / fftWindowSize);
                fftIndex++;
                lastFftPosition += fftWindowSize;
            }

            return bytesRead;
        }

        private float[] getFrequencyMags()
        {
            float[] freqMags = new float[fftWindowSize];
            int[] measureCounts = new int[fftWindowSize];
            for (int i = 0; i < fftColumns.GetLength(0); i++)
            {
                for (int j = 0; j < fftWindowSize; j++)
                {
                    var c = fftColumns[i, j];
                    freqMags[j] += (float) c.Magnitude;
                    measureCounts[j]++;
                }
            }
            for (int j = 0; j < fftWindowSize; j++)
            { 
                if (measureCounts[j] > 0)
                    freqMags[j] /= measureCounts[j];
            }
            return freqMags;
        }

        private float[] getFrequencyMagRanges()
        {
            throw new NotImplementedException();
        }

    }
}
