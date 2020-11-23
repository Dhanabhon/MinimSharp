using System;

/// <summary>
/// A Lanczos window function.
/// </summary>

namespace MinimSharp.Analysis
{
    public class LanczosWindow : WindowFunction
    {
        public LanczosWindow()
        {

        }

        protected override float Value(int length, int index)
        {
            float x = 2.0f * index / (length - 1) - 1.0f;
            return (float)(Math.Sin(Math.PI * x) / (Math.PI * x));
        }

        public string ToString()
        {
            return "Lanczos Window";
        }
    }
}
