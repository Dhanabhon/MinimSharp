
namespace MinimSharp.Analysis
{
    /// <summary>
    /// A Rectangular window function
    /// A Rectangular window is equivalent to using no window at all.
    /// </summary>
    public class RectangularWindow : WindowFunction
    {
        public RectangularWindow()
        {

        }

        protected override float Value(int length, int index)
        {
            return 1.0f;
        }

        public override string ToString()
        {
            return "Rectangular Window";
        }
    }
}
