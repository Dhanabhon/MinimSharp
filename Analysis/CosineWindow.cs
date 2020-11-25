using System;

/// <summary>
/// A Cosine window function
/// </summary>

namespace MinimSharp.Analysis
{
    public class CosineWindow : WindowFunction
    {
        public CosineWindow()
        {

        }

        protected override float Value(int length, int index)
        {
            return (float)(Math.Cos(Math.PI * index / (length - 1) - Math.PI / 2.0));
        }

        public override string ToString()
        {
            return "Cosine Window";
        }
    }
}
