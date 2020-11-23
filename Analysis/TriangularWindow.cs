using System;

namespace MinimSharp.Analysis
{
    /// <summary>
    /// A Triangular window function.
    /// </summary>
    public class TriangularWindow : WindowFunction
    {
        public TriangularWindow()
        {

        }

        protected override float Value(int length, int index)
        {
            return 2.0f / length * (length / 2.0f - Math.Abs(index - (length - 1) / 2.0f));
        }

        public string ToString()
        {
            return "Triangular Window";
        }
    }
}
