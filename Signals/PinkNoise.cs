using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimSharp.Signals
{
    public class PinkNoise : AudioSignal
    {
        protected float amp;
        protected float pan;
        protected float leftScale, rightScale;

        private int maxKey, key, range;
        private float [] whiteValues;
        private float maxSumEver;

        public PinkNoise()
        {
            amp = 1;
            pan = 0;
            leftScale = rightScale = 1;
            InitPink();
        }

        public PinkNoise(float amp)
        {
            SetAmp(amp);
            pan = 0;
            leftScale = rightScale = 1;
            InitPink();
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
            for (int i = 0; i < signal.Length; i++)
            {
                signal[i] = amp * Pink();
            }
        }

        public void Generate(float[] left, float[] right)
        {
            for (int i = 0; i < left.Length; i++)
            {
                left[i] = leftScale * amp * Pink();
                right[i] = rightScale * amp * Pink();
            }
        }

        private void InitPink()
        {
            Random rand = new Random();
            maxKey = 0x1f;
            range = 128;
            maxSumEver = 90;
            key = 0;
            whiteValues = new float[6];

            for (int i = 0; i < 6; i++)
                whiteValues[i] = ((float)rand.NextDouble() * long.MaxValue) % (range / 6);
        }

        // return a pink noise value
        private float Pink()
        {
            Random rand = new Random();

            int last_key = key;
            float sum;

            key++;
            if (key > maxKey) key = 0;
            // Exclusive-Or previous value with current value. This gives
            // a list of bits that have changed.
            int diff = last_key ^ key;
            sum = 0;
            for (int i = 0; i < 6; i++)
            {
                // If bit changed get new random number for corresponding
                // white_value
                if ((diff & (1 << i)) != 0)
                {
                    whiteValues[i] = ((float)rand.NextDouble() * long.MaxValue) % (range / 6);
                }
                sum += whiteValues[i];
            }
            if (sum > maxSumEver) maxSumEver = sum;
            sum = 2f * (sum / maxSumEver) - 1f;
            return sum;
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
