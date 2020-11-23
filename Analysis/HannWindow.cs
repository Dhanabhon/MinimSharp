using System;

/// <summary>
/// A Hann window function.
/// </summary>

namespace MinimSharp.Analysis
{
    public class HannWindow : WindowFunction
    {
        public HannWindow()
        {

        }

        protected override float Value(int length, int index)
        {
            return 0.5f * (1.0f - (float)Math.Cos(TWO_PI * index / (length - 1.0f)));
        }

        public string ToString()
        {
            return "Hann Window";
        }
    }
}
