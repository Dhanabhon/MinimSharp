using System;
using System.Collections.Generic;
using System.Text;

namespace MinimSharp.Analysis
{
    public abstract class FourierTransform
    {
        public static readonly WindowFunction NONE = new RectangularWindow();

        public static readonly WindowFunction HAMMING = new HammingWindow();

        public static readonly WindowFunction HANN = new HammingWindow();

        public static readonly WindowFunction COSINE = new CosineWindow();

        public static readonly WindowFunction TRIANGULAR = new TriangularWindow();

        public static readonly WindowFunction BARTLETT = new BartlettWindow();

        public static readonly WindowFunction BARTLETTHANN = new BartlettHannWindow();

        public static readonly WindowFunction LANCZOS = new LanczosWindow();

        public static readonly WindowFunction BLACKMAN = new BlackmanWindow();

        public static readonly WindowFunction GAUSS = new GaussWindow();

        protected static readonly int LINAVG = 1;
        protected static readonly int LOGAVG = 2;
        protected static readonly int NOAVG = 3;

        protected static readonly float TWO_PI = (float)(2.0 * Math.PI);
        protected int timeSize;
        protected int sampleRate;
        protected float bandWidth;
        protected WindowFunction currentWindow;
        protected float[] real;
        protected float[] imag;
        protected float[] spectrum;
        protected float[] averages;
        protected int whichAverage;
        protected int octaves;
        protected int avgPerOctave;

        protected abstract void AllocateArrays();

        public abstract void SetBand(int i, float a);

        public abstract void ScaleBand(int i, float s);

        public abstract void Forward(float[] buffer);

        public abstract void Inverse(float[] buffer);

        /// <summary>
        /// Construct a FourierTransform that will analyze sample buffers that are
        /// <code>ts</code> samples long and contain samples with a <code>sr</code>
        /// sample rate.
        /// </summary>
        /// <param name="ts">the length of the buffers that will be analyzed</param>
        /// <param name="sr">the sample rate of the samples that will be analyzed</param>
        public FourierTransform(int ts, float sr)
        {
            timeSize = ts;
            sampleRate = (int)sr;
            bandWidth = (2.0f / timeSize) * ((float)sampleRate / 2.0f);
            NoAverages();
            AllocateArrays();
            currentWindow = new RectangularWindow(); // a Rectangular window is analogous to using no window. 
        }

        protected void SetComplex(float[] r, float[] i)
        {
            if (real.Length != r.Length && imag.Length != i.Length)
            {
                //Minim.error("FourierTransform.setComplex: the two arrays must be the same length as their member counterparts.");
            }
            else
            {
                Array.Copy(r, 0, real, 0, r.Length);
                Array.Copy(i, 0, imag, 0, i.Length);
            }
        }

        protected void FillSpectrum()
        {
            for (int i = 0; i < spectrum.Length; i++)
            {
                spectrum[i] = (float)Math.Sqrt(real[i] * real[i] + imag[i] * imag[i]);
            }

            if (whichAverage == LINAVG)
            {
                int avgWidth = (int)spectrum.Length / averages.Length;
                for (int i = 0; i < averages.Length; i++)
                {
                    float avg = 0;
                    int j;
                    for (j = 0; j < avgWidth; j++)
                    {
                        int offset = j + i * avgWidth;
                        if (offset < spectrum.Length)
                        {
                            avg += spectrum[offset];
                        }
                        else
                        {
                            break;
                        }
                    }
                    avg /= j + 1;
                    averages[i] = avg;
                }
            }
            else if (whichAverage == LOGAVG)
            {
                for (int i = 0; i < octaves; i++)
                {
                    float lowFreq, hiFreq, freqStep;
                    if (i == 0)
                    {
                        lowFreq = 0;
                    }
                    else
                    {
                        lowFreq = (sampleRate / 2) / (float)Math.Pow(2, octaves - i);
                    }
                    hiFreq = (sampleRate / 2) / (float)Math.Pow(2, octaves - i - 1);
                    freqStep = (hiFreq - lowFreq) / avgPerOctave;
                    float f = lowFreq;
                    for (int j = 0; j < avgPerOctave; j++)
                    {
                        int offset = j + i * avgPerOctave;
                        averages[offset] = CalcAvg(f, f + freqStep);
                        f += freqStep;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the object to not compute averages.
        /// </summary>
        public void NoAverages()
        {
            averages = new float[0];
            whichAverage = NOAVG;
        }

        public void LinAverages(int numAvg)
        {
            if (numAvg > spectrum.Length / 2)
            {
                // Minim.error("The number of averages for this transform can be at most " + spectrum.Length / 2 + ".");
                return;
            }
            else
            {
                averages = new float[numAvg];
            }
            whichAverage = LINAVG;
        }

        public void LogAverages(int minBandwidth, int bandsPerOctave)
        {
            float nyq = (float)sampleRate / 2f;
            octaves = 1;
            while ((nyq /= 2) > minBandwidth)
            {
                octaves++;
            }
            // Minim.debug("Number of octaves = " + octaves);
            avgPerOctave = bandsPerOctave;
            averages = new float[octaves * bandsPerOctave];
            whichAverage = LOGAVG;
        }

        public void Window(WindowFunction windowFunction)
        {
            this.currentWindow = windowFunction;
        }

        protected void DoWindow(float[] samples)
        {
            currentWindow.Apply(samples);
        }

        public int TimeSize()
        {
            return timeSize;
        }

        public int SpecSize()
        {
            return spectrum.Length;
        }

        public float GetBand(int i)
        {
            if (i < 0) i = 0;
            if (i > spectrum.Length - 1) i = spectrum.Length - 1;
            return spectrum[i];
        }

        public float GetBandWidth()
        {
            return bandWidth;
        }

        public float GetAverageBandWidth(int averageIndex)
        {
            if (whichAverage == LINAVG)
            {
                // an average represents a certain number of bands in the spectrum
                int avgWidth = (int)spectrum.Length / averages.Length;
                return avgWidth * GetBandWidth();

            }
            else if (whichAverage == LOGAVG)
            {
                // which "octave" is this index in?
                int octave = averageIndex / avgPerOctave;
                float lowFreq, hiFreq, freqStep;
                // figure out the low frequency for this octave
                if (octave == 0)
                {
                    lowFreq = 0;
                }
                else
                {
                    lowFreq = (sampleRate / 2) / (float)Math.Pow(2, octaves - octave);
                }
                // and the high frequency for this octave
                hiFreq = (sampleRate / 2) / (float)Math.Pow(2, octaves - octave - 1);
                // each average band within the octave will be this big
                freqStep = (hiFreq - lowFreq) / avgPerOctave;

                return freqStep;
            }

            return 0;
        }

        public int FreqToIndex(float freq)
        {
            // special case: freq is lower than the bandwidth of spectrum[0]
            if (freq < GetBandWidth() / 2) return 0;
            // special case: freq is within the bandwidth of spectrum[spectrum.length - 1]
            if (freq > sampleRate / 2 - GetBandWidth() / 2) return spectrum.Length - 1;
            // all other cases
            float fraction = freq / (float)sampleRate;
            int i = (int)Math.Round(timeSize * fraction);
            return i;
        }

        public float IndexToFreq(int i)
        {
            float bw = GetBandWidth();
            // special case: the width of the first bin is half that of the others.
            //               so the center frequency is a quarter of the way.
            if (i == 0) return bw * 0.25f;
            // special case: the width of the last bin is half that of the others.
            if (i == spectrum.Length - 1)
            {
                float lastBinBeginFreq = (sampleRate / 2) - (bw / 2);
                float binHalfWidth = bw * 0.25f;
                return lastBinBeginFreq + binHalfWidth;
            }
            // the center frequency of the ith band is simply i*bw
            // because the first band is half the width of all others.
            // treating it as if it wasn't offsets us to the middle 
            // of the band.
            return i * bw;
        }

        public float GetAverageCenterFrequency(int i)
        {
            if (whichAverage == LINAVG)
            {
                // an average represents a certain number of bands in the spectrum
                int avgWidth = (int)spectrum.Length / averages.Length;
                // the "center" bin of the average, this is fudgy.
                int centerBinIndex = i * avgWidth + avgWidth / 2;
                return IndexToFreq(centerBinIndex);

            }
            else if (whichAverage == LOGAVG)
            {
                // which "octave" is this index in?
                int octave = i / avgPerOctave;
                // which band within that octave is this?
                int offset = i % avgPerOctave;
                float lowFreq, hiFreq, freqStep;
                // figure out the low frequency for this octave
                if (octave == 0)
                {
                    lowFreq = 0;
                }
                else
                {
                    lowFreq = (sampleRate / 2) / (float)Math.Pow(2, octaves - octave);
                }
                // and the high frequency for this octave
                hiFreq = (sampleRate / 2) / (float)Math.Pow(2, octaves - octave - 1);
                // each average band within the octave will be this big
                freqStep = (hiFreq - lowFreq) / avgPerOctave;
                // figure out the low frequency of the band we care about
                float f = lowFreq + offset * freqStep;
                // the center of the band will be the low plus half the width
                return f + freqStep / 2;
            }

            return 0;
        }

        public float GetFreq(float freq)
        {
            return GetBand(FreqToIndex(freq));
        }

        public void SetFreq(float freq, float a)
        {
            SetBand(FreqToIndex(freq), a);
        }

        public void ScaleFreq(float freq, float s)
        {
            ScaleBand(FreqToIndex(freq), s);
        }

        public int AvgSize()
        {
            return averages.Length;
        }

        public float GetAvg(int i)
        {
            float ret;
            if (averages.Length > 0)
                ret = averages[i];
            else
                ret = 0;
            return ret;
        }

        public float CalcAvg(float lowFreq, float hiFreq)
        {
            int lowBound = FreqToIndex(lowFreq);
            int hiBound = FreqToIndex(hiFreq);
            float avg = 0;
            for (int i = lowBound; i <= hiBound; i++)
            {
                avg += spectrum[i];
            }
            avg /= (hiBound - lowBound + 1);
            return avg;
        }

        public float[] GetSpectrumReal()
        {
            return real;
        }

        public float[] GetSpectrumImaginary()
        {
            return imag;
        }

        public void Forward(float[] buffer, int startAt)
        {
            if (buffer.Length - startAt < timeSize)
            {
                // Minim.error("FourierTransform.forward: not enough samples in the buffer between " + startAt + " and " + buffer.Length + " to perform a transform.");
                return;
            }

            // copy the section of samples we want to analyze
            float[] section = new float[timeSize];
            Array.Copy(buffer, startAt, section, 0, section.Length);
            Forward(section);
        }

        public void Inverse(float[] freqReal, float[] freqImag, float[] buffer)
        {
            SetComplex(freqReal, freqImag);
            Inverse(buffer);
        }
    } // class
} // namespace
