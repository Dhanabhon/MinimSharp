using System;
using System.Collections.Generic;
using System.Text;

namespace MinimSharp
{
    public interface AudioSignal
    {
        /// <summary>
        /// Fills <code>signal</code> with values in the range of [-1, 1].
        /// <code>signal</code> represents a mono audio signal.
        /// </summary>
        /// <param name="signal">the float array to fill</param>
        void Generate(float[] signal);

        /// <summary>
        /// Fills <code>left</code> and <code>right</code> with values in the range
        /// of [-1, 1]. <code>left</code> represents the left channel of a stereo
        /// signal, <code>right</code> represents the right channel of that same
        /// stereo signal.
        /// </summary>
        /// <param name="left">the left channel</param>
        /// <param name="right">the right channel</param>
        void Generate(float[] left, float[] right);
    }
}
