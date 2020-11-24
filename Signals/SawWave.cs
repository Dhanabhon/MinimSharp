using System;

namespace MinimSharp.Signals
{
    public class SawWave : Oscillator
    {
        public SawWave(float frequency, float amplitude, float sampleRate) : base(frequency, amplitude, sampleRate)
        {

        }

        protected override float Value(float step)
        {
            return 2.0f * (step - (float)Math.Round(step));
        }
    }
}
