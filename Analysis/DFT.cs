using System;

namespace MinimSharp.Analysis
{
    public class DFT : FourierTransform
    {
        private float[] sinlookup;
        private float[] coslookup;

        public DFT(int timeSize, float sampleRate) : base(timeSize, sampleRate)
        {
            if (timeSize % 2 != 0)
                throw new ArgumentException("DFT: timeSize must be even.");
            BuildTrigTables();
        }

        protected override void AllocateArrays()
        {
            spectrum = new float[timeSize / 2 + 1];
            real = new float[timeSize / 2 + 1];
            imag = new float[timeSize / 2 + 1];
        }

        public override void ScaleBand(int i, float s)
        {
        }

        public override void SetBand(int i, float a)
        {
        }

        public override void Forward(float[] samples)
        {
            if (samples.Length != timeSize)
            {
                MinimSharp.Error("DFT.forward: The length of the passed sample buffer must be equal to DFT.timeSize().");
                return;
            }
            DoWindow(samples);
            int N = samples.Length;
            for (int f = 0; f <= N / 2; f++)
            {
                real[f] = 0.0f;
                imag[f] = 0.0f;
                for (int t = 0; t < N; t++)
                {
                    real[f] += samples[t] * Cos(t * f);
                    imag[f] += samples[t] * -Sin(t * f);
                }
            }
            FillSpectrum();
        }

        public override void Inverse(float[] buffer)
        {
            int N = buffer.Length;
            real[0] /= N;
            imag[0] = -imag[0] / (N / 2);
            real[N / 2] /= N;
            imag[N / 2] = -imag[0] / (N / 2);
            for (int i = 0; i < N / 2; i++)
            {
                real[i] /= (N / 2);
                imag[i] = -imag[i] / (N / 2);
            }
            for (int t = 0; t < N; t++)
            {
                buffer[t] = 0.0f;
                for (int f = 0; f < N / 2; f++)
                {
                    buffer[t] += real[f] * Cos(t * f) + imag[f] * Sin(t * f);
                }
            }
        }

        private void BuildTrigTables()
        {
            int n = spectrum.Length * timeSize;
            sinlookup = new float[n];
            coslookup = new float[n];
            for (int i = 0; i < n; i++)
            {
                sinlookup[i] = (float)Math.Sin(i * TWO_PI / timeSize);
                coslookup[i] = (float)Math.Cos(i * TWO_PI / timeSize);
            }
        }

        private float Sin(int i)
        {
            return sinlookup[i];
        }

        private float Cos(int i)
        {
            return coslookup[i];
        }

    }
}
