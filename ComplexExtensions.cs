using System;
using System.Collections.Generic;
using System.Text;

namespace SpectrumAnalyzer
{
    public static class ComplexExtensions
    {
        public static double Magnitude(this NAudio.Dsp.Complex c)
        {
            return Math.Sqrt(c.X * c.X + c.Y * c.Y);
        }
    }
}
