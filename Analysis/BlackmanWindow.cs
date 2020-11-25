using System;

/// <summary>
/// A Blackman window function
/// </summary>

namespace MinimSharp.Analysis
{
    public class BlackmanWindow : WindowFunction
    {
        protected float alpha;

        public BlackmanWindow() : this(0.16f)
        {

        }

        /// <summary>
        /// Constructs a Blackman window.
        /// </summary>
        /// <param name="alpha">float: the alpha value to use for this window</param>
        public BlackmanWindow(float alpha)
        {
            this.alpha = alpha;
        }

        protected override float Value(int length, int index)
        {
            float a0 = (1.0f - this.alpha) / 2.0f;
            float a1 = 0.5f;
            float a2 = this.alpha / 2.0f;

            return a0 - a1 * (float)Math.Cos(TWO_PI * index / (length - 1)) + a2 * (float)Math.Cos(4 * Math.PI * index / (length - 1));
        }

        public override string ToString()
        {
            return "Blackman Window";
        }
    }
}
