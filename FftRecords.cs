using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpectrumAnalyzer
{
    public class FftRecords
    {
        public FftRecords(SpectrumData _data)
        {
            data = _data;
            
            Records = new FftRecord[FftCount];
        }

        public SpectrumData Data => data;

        private SpectrumData data;
        private float delta = 0.01F;
        private List<FftRecord> LostRecords = new List<FftRecord>();
        private SortedList<float, FftRecord> sortedRecords = new SortedList<float, FftRecord>();
        
        public int FftCount => (int) data.FftCount;

        public float Delta => delta;

        public FftRecord[] Records { get; }

        public SortedList<float, FftRecord> SortedRecords => sortedRecords;

        public WaveFormat WavFormat => data.WaveFormat;

        public long Length => data.Length;

        public void AddRecord(FftRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            if (record.Id < 0 || record.Id >= FftCount)
                throw new ArgumentOutOfRangeException(nameof(record.Id), $"ItemNo must be between 0 and {FftCount - 1}.");
            Records[record.Id] = record;
            int count = 0;
            while (sortedRecords.ContainsKey(record.AvgFreq) && count < 100)
            {
                record.AvgFreq += 0.000001F;
                count++;
                if (count >= 100)
                {
                    LostRecords.Add(record);
                    return;   // should be a very rare event
                }
            }
            sortedRecords.Add(record.AvgFreq, record);
        }

        public FftRecord GetRecord(int itemNo)
        {
            if (itemNo < 0 || itemNo >= FftCount)
                throw new ArgumentOutOfRangeException(nameof(itemNo), $"ItemNo must be between 0 and {FftCount - 1}.");
            return Records[itemNo];
        }

        public FftRecord GetNextRecordByPitch(FftRecord lastRec, float pitch)
        {
            FftRecord? testRecord = lastRec;
            if (testRecord == null)
            {
                testRecord = GetRecord(0);
            }
            bool pitchUp = testRecord.AvgFreq <= pitch;
            bool pitchDown = testRecord.AvgFreq > pitch;
            int steps = 0;

            if (pitchUp)
            {
                while (testRecord.AvgFreq < pitch)
                {
                    if (testRecord.Id == lastRec!.Id + 1 &&
                        testRecord.AvgFreq >= pitch - delta)
                    {
                        return testRecord;
                    }
                    testRecord = GetRecord(testRecord.NextByPitch);
                }
                if (testRecord.AvgFreq > pitch)
                {
                     return testRecord;
                }
                return testRecord;
            }
            else if (pitchDown)
            {
                while (testRecord.AvgFreq < pitch)
                {
                    if (testRecord.Id == lastRec!.Id + 1 &&
                        testRecord.AvgFreq >= pitch - delta)
                    {
                        return testRecord;
                    }
                    testRecord = GetRecord(testRecord.PrevByPitch);
                }
                if (testRecord.AvgFreq > pitch)
                {
                    return testRecord;
                }
            }
            return testRecord;
        }

        public void BuildCrossRefLinks()
        {
            for (int i = 0; i < Records.Length; i++)
            {
                FftRecord record = Records[i];
                if (record == null)
                    continue;
                float prevKey, nextKey;
                FftRecord prevRec, nextRec;
                int keyPos = sortedRecords.Keys.IndexOf(record.AvgFreq);
                if (keyPos == -1)
                {
                    AddRecord(record);
                    keyPos = sortedRecords.Keys.IndexOf(record.AvgFreq);
                }
                if (keyPos > 0)
                {
                    prevKey = sortedRecords.Keys[keyPos - 1];
                    prevRec = sortedRecords[prevKey];
                    record.PrevByPitch = prevRec.Id;
                    prevRec.NextByPitch = record.Id;
                }
                else
                {
                    // keyPos = 0
                    record.PrevByPitch = -1;
                    if (sortedRecords.Keys.Count > keyPos)
                    {
                        nextKey = sortedRecords.Keys[keyPos + 1];
                        nextRec = sortedRecords[nextKey];
                        record.NextByPitch = nextRec.Id;
                        nextRec.PrevByPitch = record.Id;
                    }
                }

                for (int j = i + 1; j < FftCount; j++)
                {
                    FftRecord nextRecord = Records[j];
                    if (nextRecord == null)
                        continue;
                    if (nextRecord.AvgFreq >= record.AvgFreq - delta &&
                        nextRecord.AvgFreq <= record.AvgFreq + delta)
                    {
                        record.NextByPitch = nextRecord.Id;
                        nextRecord.PrevByPitch = record.Id;
                        break;
                    }
                }
            }
        }

        public (float, float) GetAvgFreqPower()
        {
            float totFreq = 0, totalValues = 0;
            int reccount = 0;
            for (int i = 0; i < Records.Length; i++)
            {
                FftRecord record = Records[i];
                if (record == null)
                    continue;
                totFreq += record.AvgFreq;
                reccount++;
                totalValues += record.Power;
            }
            if (reccount == 0)
                return (0F, 0F);
            return (totFreq / reccount, totalValues / reccount);
        }
    }
}
