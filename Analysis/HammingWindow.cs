using System;

/// <summary>
/// A Hamming window function.
/// </summary>

namespace MinimSharp.Analysis
{
    public class HammingWindow : WindowFunction
    {
        public HammingWindow()
        {

        }

        protected override float Value(int length, int index)
        {
            return 0.54f - 0.46f * (float)Math.Cos(TWO_PI * index / (length - 1.0f));
        }

        public override string ToString()
        {
            return "Hamming Window";
        }
    }
}
