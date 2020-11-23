using System;

namespace MinimSharp
{
    public abstract class WindowFunction
    {
        protected static readonly float TWO_PI = (float)(2 * Math.PI);
        protected int length;

        protected abstract float Value(int length, int index);

        public WindowFunction()
        {

        }

        /// <summary>
        /// Apply the window function to a sample buffer.
        /// </summary>
        /// <param name="samples">samples a sample buffer</param>
        public void Apply(float[] samples)
        {
            this.length = samples.Length;

            for (int i = 0; i < samples.Length; i++)
                samples[i] *= this.Value(samples.Length, i);
        }

        /// <summary>
        /// Apply the window to a portion of this sample buffer,
        /// given an offset from the beginning of the buffer
        /// and the number of samples to be widowed.
        /// </summary>
        /// <param name="samples">the array of samples to apply the window to</param>
        /// <param name="offset">the index in the array to begin windowing</param>
        /// <param name="length">how many samples to apply the window to</param>
        public void Apply(float[] samples, int offset, int length)
        {
            this.length = length;
            for (int i = offset; i < offset + length; ++i)
                samples[i] *= this.Value(length, i - offset);
        }

        /// <summary>
        /// Generates the curve of the window function.
        /// </summary>
        /// <param name="length">length the length of the window</param>
        /// <returns>the shape of the window function</returns>
        public float[] GenerateCurve(int length)
        {
            float[] samples = new float[length];
            for (int i = 0; i < length; i++)
                samples[i] = 1.0f * this.Value(length, i);
            return samples;
        }
    } // calss
} // namespace
