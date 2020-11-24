
namespace MinimSharp.Signals
{
    public class SquareWave : Oscillator
    {
        public SquareWave(float frequency, float amplitude, float sampleRate) : base(frequency, amplitude, sampleRate)
        {

        }

        protected override float Value(float step)
        {
            return step < 0.5 ? 1 : -1;
        }
    }
}
