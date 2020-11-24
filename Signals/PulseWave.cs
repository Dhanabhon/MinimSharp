
namespace MinimSharp.Signals
{
    public class PulseWave : Oscillator
    {
        private float width = 0.0f;

        public PulseWave(float frequency, float amplitude, float sampleRate) : base (frequency, amplitude, sampleRate)
        {
            this.width = 2.0f;
        }

        /// <summary>
        /// Sets the pulse width of the pulse wave.
        /// </summary>
        /// <param name="width">the new pulse width, this will be constrained to 1 - 30</param>
        public void SetPulseWidth(float width)
        {
            this.width = width < 1.0f ? 1.0f : (width > 30.0f ? 30.0f : width);
        }

        /// <summary>
        /// Returns the current pulse width.
        /// </summary>
        /// <returns>the current pulse width</returns>
        public float GetPulseWidth()
        {
            return this.width;
        }

        protected override float Value(float step)
        {
            float v;
            if (step < 1.0f / (width + 1.0f))
                v = 1.0f;
            else
                v = -1.0f;
            return v;
        }
    }
}
