using System;

namespace MinimSharp.Signals
{
    public class SineWave : Oscillator
    {
		/// <summary>
		/// Constructs a sine wave with the given frequency, amplitude and sample rate.
		/// </summary>
		/// <param name="frequency">the frequency of the pulse wave</param>
		/// <param name="amplitude">the amplitude of the pulse wave</param>
		/// <param name="sampleRate">the sample rate of the pulse wave</param>
		public SineWave(float frequency, float amplitude, float sampleRate) : base(frequency, amplitude, sampleRate)
		{

		}

		protected override float Value(float step)
		{
			return (float)Math.Sin(TWO_PI * step);
		}
	}
}
