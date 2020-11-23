using System;
using System.Collections.Generic;
using System.Text;

namespace MinimSharp
{
    public interface AudioListener
    {
        /// <summary>
        /// Called by the audio object this AudioListener is attached to when that object has new samples.
        /// </summary>
        /// <param name="samp">a float[] buffer of samples from a MONO sound stream</param>
        void Samples(float[] samp);

        /// <summary>
        /// Called by the <code>Recordable</code> object this is attached to  when that object has new samples.
        /// </summary>
        /// <param name="sampL">a float[] buffer containing the left channel of a STEREO sound stream</param>
        /// <param name="sampR"><a float[] buffer containing the right channel of a STEREO sound stream/param>
        void Samples(float[] sampL, float[] sampR);
    }
}
