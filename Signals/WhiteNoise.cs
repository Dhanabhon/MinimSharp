using System;

namespace MinimSharp.Signals
{
    public class WhiteNoise : AudioSignal
    {
        protected float amp;
        protected float pan;
        protected float leftScale, rightScale;

        public WhiteNoise()
        {
            amp = 1;
            pan = 0;
            leftScale = rightScale = 1;
        }

        public WhiteNoise(float amp)
        {
            SetAmp(amp);
            pan = 0;
            leftScale = rightScale = 1;
        }

        public void SetAmp(float a)
        {
            amp = Constrain(a, 0, 1);
        }

        public void SetPan(float p)
        {
            pan = Constrain(p, -1, 1);
            CalcLRScale();
        }

        public void Generate(float[] signal)
        {
            Random rand = new Random();
            for (int i = 0; i < signal.Length; i++)
            {
                signal[i] = amp * (2 * (float)rand.NextDouble() - 1);
            }
        }

        public void Generate(float[] left, float[] right)
        {
            Random rand = new Random();
            for (int i = 0; i < left.Length; i++)
            {
                left[i] = leftScale * amp * (2 * (float)rand.NextDouble() - 1);
                right[i] = rightScale * amp * (2 * (float)rand.NextDouble() - 1);
            }
        }

        private void CalcLRScale()
        {
            if (pan <= 0)
            {
                // map -1, 0 to 0, 1
                rightScale = pan + 1;
                leftScale = 1;
            }
            if (pan >= 0)
            {
                // map 0, 1 to 1, 0;
                leftScale = 1 - pan;
                rightScale = 1;
            }
            if (pan == 0)
            {
                leftScale = rightScale = 1;
            }
        }

        float Constrain(float val, float min, float max)
        {
            return val < min ? min : (val > max ? max : val);
        }

    } // class
} // namespace
