using System;

namespace MinimSharp.Analysis
{
    public class FFT : FourierTransform
    {
        private int[] reverse;

        private float[] sinlookup;
        private float[] coslookup;

        public FFT(int timeSize, float sampleRate) : base(timeSize, sampleRate)
        {
            if ((timeSize & (timeSize - 1)) != 0)
            {
                throw new ArgumentException("FFT: timeSize must be a power of two.");
            }
            BuildReverseTable();
            BuildTrigTables();
        }

        protected override void AllocateArrays()
        {
            spectrum = new float[timeSize / 2 + 1];
            real = new float[timeSize];
            imag = new float[timeSize];
        }

        public override void ScaleBand(int i, float s)
        {
            if (s < 0)
            {
                //Minim.error("Can't scale a frequency band by a negative value.");
                return;
            }

            real[i] *= s;
            imag[i] *= s;
            spectrum[i] *= s;

            if (i != 0 && i != timeSize / 2)
            {
                real[timeSize - i] = real[i];
                imag[timeSize - i] = -imag[i];
            }
        }

        public override void SetBand(int i, float a)
        {
            if (a < 0)
            {
                //Minim.error("Can't set a frequency band to a negative value.");
                return;
            }
            if (real[i] == 0 && imag[i] == 0)
            {
                real[i] = a;
                spectrum[i] = a;
            }
            else
            {
                real[i] /= spectrum[i];
                imag[i] /= spectrum[i];
                spectrum[i] = a;
                real[i] *= spectrum[i];
                imag[i] *= spectrum[i];
            }
            if (i != 0 && i != timeSize / 2)
            {
                real[timeSize - i] = real[i];
                imag[timeSize - i] = -imag[i];
            }
        }

        private void Fft()
        {
            for (int halfSize = 1; halfSize < real.Length; halfSize *= 2)
            {
                // float k = -(float)Math.PI/halfSize;
                // phase shift step
                // float phaseShiftStepR = (float)Math.cos(k);
                // float phaseShiftStepI = (float)Math.sin(k);
                // using lookup table
                float phaseShiftStepR = Cos(halfSize);
                float phaseShiftStepI = Sin(halfSize);
                // current phase shift
                float currentPhaseShiftR = 1.0f;
                float currentPhaseShiftI = 0.0f;
                for (int fftStep = 0; fftStep < halfSize; fftStep++)
                {
                    for (int i = fftStep; i < real.Length; i += 2 * halfSize)
                    {
                        int off = i + halfSize;
                        float tr = (currentPhaseShiftR * real[off]) - (currentPhaseShiftI * imag[off]);
                        float ti = (currentPhaseShiftR * imag[off]) + (currentPhaseShiftI * real[off]);
                        real[off] = real[i] - tr;
                        imag[off] = imag[i] - ti;
                        real[i] += tr;
                        imag[i] += ti;
                    }
                    float tmpR = currentPhaseShiftR;
                    currentPhaseShiftR = (tmpR * phaseShiftStepR) - (currentPhaseShiftI * phaseShiftStepI);
                    currentPhaseShiftI = (tmpR * phaseShiftStepI) + (currentPhaseShiftI * phaseShiftStepR);
                }
            }
        }

        public override void Forward(float[] buffer)
        {
            if (buffer.Length != timeSize)
            {
                //Minim.error("FFT.forward: The length of the passed sample buffer must be equal to timeSize().");
                return;
            }
            DoWindow(buffer);
            // copy samples to real/imag in bit-reversed order
            BitReverseSamples(buffer, 0);
            // perform the fft
            Fft();
            // fill the spectrum buffer with amplitudes
            FillSpectrum();
        }

        //public override void Forward(float[] buffer, int startAt)
        //{
        //    if (buffer.Length - startAt < timeSize)
        //    {
        //        //Minim.error("FourierTransform.forward: not enough samples in the buffer between " + startAt + " and " + buffer.Length + " to perform a transform.");
        //        return;
        //    }

        //    currentWindow.Apply(buffer, startAt, timeSize);
        //    BitReverseSamples(buffer, startAt);
        //    Fft();
        //    FillSpectrum();
        //}

        //public override void Forward(float[] buffReal, float[] buffImag)
        //{
        //    if (buffReal.Length != timeSize || buffImag.Length != timeSize)
        //    {
        //        //Minim.error("FFT.forward: The length of the passed buffers must be equal to timeSize().");
        //        return;
        //    }
        //    SetComplex(buffReal, buffImag);
        //    BitReverseComplex();
        //    Fft();
        //    FillSpectrum();
        //}

        public override void Inverse(float[] buffer)
        {
            if (buffer.Length > real.Length)
            {
                //Minim.error("FFT.inverse: the passed array's length must equal FFT.timeSize().");
                return;
            }
            // conjugate
            for (int i = 0; i < timeSize; i++)
            {
                imag[i] *= -1;
            }
            BitReverseComplex();
            Fft();
            // copy the result in real into buffer, scaling as we do
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = real[i] / real.Length;
            }
        }

        private void BuildReverseTable()
        {
            int N = timeSize;
            reverse = new int[N];

            // set up the bit reversing table
            reverse[0] = 0;
            for (int limit = 1, bit = N / 2; limit < N; limit <<= 1, bit >>= 1)
                for (int i = 0; i < limit; i++)
                    reverse[i + limit] = reverse[i] + bit;
        }

        private void BitReverseSamples(float[] samples, int startAt)
        {
            for (int i = 0; i < timeSize; ++i)
            {
                real[i] = samples[startAt + reverse[i]];
                imag[i] = 0.0f;
            }
        }

        private void BitReverseComplex()
        {
            float[] revReal = new float[real.Length];
            float[] revImag = new float[imag.Length];
            for (int i = 0; i < real.Length; i++)
            {
                revReal[i] = real[reverse[i]];
                revImag[i] = imag[reverse[i]];
            }
            real = revReal;
            imag = revImag;
        }

        private float Sin(int i)
        {
            return sinlookup[i];
        }

        private float Cos(int i)
        {
            return coslookup[i];
        }

        private void BuildTrigTables()
        {
            int N = timeSize;
            sinlookup = new float[N];
            coslookup = new float[N];
            for (int i = 0; i < N; i++)
            {
                sinlookup[i] = (float)Math.Sin(-(float)Math.PI / i);
                coslookup[i] = (float)Math.Cos(-(float)Math.PI / i);
            }
        }
    } // class
} // namespace
