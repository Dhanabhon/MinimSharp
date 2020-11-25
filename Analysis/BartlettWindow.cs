using System;

/// <summary>
/// A Bartlett window function.
/// </summary>

namespace MinimSharp
{
    public class BartlettWindow : WindowFunction
    {
         public BartlettWindow()
        {

        }

        protected override float Value(int length, int index)
        {
            return 2.0f / (length - 1) * ((length - 1) / 2.0f - Math.Abs(index - (length - 1) / 2.0f));
        }

        public override string ToString()
        {
            return "Bartlett Window";
        }
    }
}
