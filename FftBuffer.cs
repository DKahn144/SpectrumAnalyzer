using NAudio.Dsp;
using NAudio.Wave;

namespace SpectrumAnalyzer
{
    public class FftBuffer
    {
        public List<float[]> Samples = new List<float[]>();

        public List<NAudio.Dsp.Complex[]> FftResults = new List<NAudio.Dsp.Complex[]>();


        //public List<float> FFTAvg => GetAvgMag();
        //public List<float> FFTErr => GetAvgErr();

        public List<float> MaxPowerValues = new List<float>();

        public List<float> MinPowerValues = new List<float>();

        public List<float> AvgPowerValues = new List<float>();

        public List<float> PowerRanges = new List<float>();

        public int SampleCount => Samples.Count;

        public WaveFormat WaveFormat => waveFormat;

        public int MaxSampleSize => maxSampleSize;

        public List<float> FreqPower => GetFreqPower();

        public List<float> FreqPowerRange => GetFreqPowerRange();

        private int maxSampleSize;
        private WaveFormat waveFormat;
        private List<float[]> fftTotals = new List<float[]>();
        private List<float> freqPower = new List<float>();
        private List<float> freqPowerRange = new List<float>();

        public void Clear()
        {
            Samples.Clear();
            FftResults.Clear();
            MaxPowerValues.Clear();
            MinPowerValues.Clear();
            AvgPowerValues.Clear();
            fftTotals.Clear();
            freqPower.Clear();
            freqPowerRange.Clear();
            maxSampleSize = 0;
        }

        public FftBuffer(WaveFormat _waveFormat)
        {
            waveFormat = _waveFormat;
        }

        public void AddSample(float[] sample)
        {
            Samples.Add(sample);
            int len = sample.Length;
            if (len % 2 == 1)
                len--;
            if (maxSampleSize < len)
                maxSampleSize = len;
            float max = 0;
            float min = 0;
            float avg = 0;
            float range = 0;
            NAudio.Dsp.Complex[] fftBuff = new NAudio.Dsp.Complex[len];
            int fftPos = 0;
            for (int i = 0; i < len; i++)
            {
                fftBuff[i].X = (float)(sample[i] * FastFourierTransform.HammingWindow(i, len));
                fftBuff[i].Y = 0;
                fftPos++;
                max = Math.Max(max, sample[i]);
                min = Math.Min(min, sample[i]);
            }
            avg = (max + min) / 2;
            range = (max - min) / 2;
            MaxPowerValues.Add(max);
            MinPowerValues.Add(min);
            AvgPowerValues.Add(avg);
            PowerRanges.Add(range);
            int m = (int)Math.Log(sample.Length, 2.0);
            NAudio.Dsp.FastFourierTransform.FFT(true, m, fftBuff);
            AdjustFreqMagnitudes(fftBuff, FftResults.Count);
            FftResults.Add(fftBuff);
            var fftMags = new float[fftBuff.Length];
            for (int j = 0; j < fftBuff.Length; j++)
            {
                float mag = (float)Math.Sqrt(fftBuff[j].X * fftBuff[j].X + fftBuff[j].Y * fftBuff[j].Y);
                fftMags[j] = mag;
            }
            fftTotals.Add(fftMags);
        }

        private void AdjustFreqMagnitudes(Complex[] fftBuff, int count)
        {

        }

        public FftBuffer GetSlice(int freqTop, int freqBottom, int startTime, int endTime)
        {
            FftBuffer sliceBuffer = new FftBuffer(waveFormat);
            int sampleRate = waveFormat.SampleRate;

            //sliceBuffer.Samples.AddRange(this.Samples.Skip(startTime).Take(endTime - startTime));
            sliceBuffer.FftResults.AddRange(this.FftResults.Skip(startTime/this.SampleCount).Take(endTime/this.SampleCount));
            sliceBuffer.FftResults.ForEach(fft => {
                for (int i = 0; i < fft.Length; i++)
                {
                    if (i < freqBottom || i > freqTop)
                    {
                        fft[i].X = 0;
                        fft[i].Y = 0;
                    }
                }
            });
            int m = (int)Math.Log(this.SampleCount, 2.0);
            sliceBuffer.FftResults.ForEach(fft => {
                NAudio.Dsp.FastFourierTransform.FFT(false, m, fft);
                var newSamples = fft.Select(c => (float) Math.Sqrt(c.X*c.X + c.Y*c.Y)).ToArray();
                sliceBuffer.AddSample(newSamples);
            });
            return sliceBuffer;
        }

        private List<float> GetFreqPower()
        {
            if (freqPower.Count == 0)
            {
                LoadFreqLevels();
            }
            return freqPower;
        }

        private List<float> GetFreqPowerRange()
        {
            if (freqPowerRange.Count == 0)
            {
                LoadFreqLevels();
            }
            return freqPowerRange;
        }

        private void LoadFreqLevels()
        {
            for (int i = 0; i < maxSampleSize; i++)
            {
                float total = 0;
                float minimag = 0;
                float maximag = 0;
                foreach (var mags in fftTotals)
                {
                    total += mags[i];
                    minimag = Math.Min(minimag, mags[i]);
                    maximag = Math.Max(maximag, mags[i]);
                }
                freqPower.Add(total / fftTotals.Count);
                freqPowerRange.Add((maximag - minimag) / 2);
            }

            foreach (var mags in fftTotals)
            {
                var mag = new float[Samples.Count];
                foreach(int j in mags)
                {
                    //mag[j] = 
                }
                //avgMags.Add(total / Samples.Count);
            }
        }
    }
}