using System;

/// <summary>
/// A Gauss window function.
/// </summary>

namespace MinimSharp.Analysis
{
    public class GaussWindow : WindowFunction
    {
        private readonly double alpha;

        public GaussWindow() : this(0.25)
        {

        }

        /// <summary>
        /// Constructs a Gauss window function.
        /// </summary>
        /// <param name="alpha">double: the alpha value to use for this window</param>
        public GaussWindow(double alpha)
        {
            if (alpha < 0.0 || alpha > 0.5)
            {
                MinimSharp.Error("Range for GaussWindow out of bounds. Value must be <= 0.5");
                return;
            }
            this.alpha = alpha;
        }

        protected override float Value(int length, int index)
        {
            return (float)Math.Pow(Math.E, -0.5 * Math.Pow((index - (length - 1) / (double)2) / (this.alpha * (length - 1) / 2.0), 2.0));
        }

        public string ToString()
        {
            return "Gauss Window";
        }
    }
}
