using System;

/// <summary>
/// A Barlett-Hann window function.
/// </summary>

namespace MinimSharp.Analysis
{
    public class BartlettHannWindow : WindowFunction
    {
        public BartlettHannWindow()
        {

        }

        protected override float Value(int length, int index)
        {
            return (float)(0.62 - 0.48 * Math.Abs(index / (length - 1) - 0.5) - 0.38 * Math.Cos(TWO_PI * index / (length - 1)));
        }

        public string ToString()
        {
            return "Bartlett-Hann Window";
        }
    }
}
