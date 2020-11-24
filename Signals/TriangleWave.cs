using System;

namespace MinimSharp.Signals
{
    public class TriangleWave : Oscillator
    {
        public TriangleWave(float frequency, float amplitude, float sampleRate) : base(frequency, amplitude, sampleRate)
        {

        }

        protected override float Value(float step)
        {
            return 1.0f - 4.0f * (float)Math.Abs(Math.Round(step) - step);
        }
    }
}
