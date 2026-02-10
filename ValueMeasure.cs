using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace SpectrumAnalyzer
{
    public class ValueMeasure
    {
        public int ValueCount => Values.Count;

        public List<float> Values = new List<float>();

        public float AvgValue => Values.Average();

        public float MaxValue => Values.Max();

        public float MinValue => Values.Min();

        public float Range => (MaxValue - MinValue)/2;

        public void AddValue(int position, float value)
        {
            while (Values.Count <= position) Values.Add(0);
            Values[position] = Math.Abs(value);
        }

        public void AddArray(float[] values, int startPos = 0, float divideBy = 1F)
        {
            int posn = startPos;
            foreach(float value in values)
            {
                AddValue(posn++, value / divideBy);
            }
        }
    }
}
