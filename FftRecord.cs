using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SpectrumAnalyzer
{
    public class FftRecord
    {
        public FftRecord(FftRecords records, Complex[] fftData, float[] _samples, int itemNo)
        {
            FftRecords = records ?? throw new ArgumentNullException(nameof(records));
            FftData = fftData ?? throw new ArgumentNullException(nameof(fftData));
            samples = _samples;
            Id = itemNo;
            CalculateParams();
        }

        private FftRecords FftRecords;
        private SpectrumData Data => FftRecords.Data;
        private float medianFrequency;
        private float totalPower;
        private float[] samples;

        public Complex[] FftData { get; }
        public float[] Samples => samples;
        public int Id { get; }
        public float AvgFreq { get { return medianFrequency; } set { medianFrequency = value; } }
        public float Power => totalPower;
        public int NextByPitch { set; get; }
        public int PrevByPitch { set; get; }

        private void CalculateParams()
        {
            float totFreq = 0, totalValues = 0;
            int ilimit = 400;
            if (ilimit > FftData.Length)
                ilimit = FftData.Length;

            for (int i = 0; i < ilimit; i++)
            {
                float mag = (float) FftData[i].Magnitude;
                totFreq += i * mag;
                totalValues += mag;
            }
            medianFrequency = totalValues == 0 ? 0 : totFreq /* * Data.HertzFactor */ / totalValues;
            totalPower = totalValues;
        }

    }
}
