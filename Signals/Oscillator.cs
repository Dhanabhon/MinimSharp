using System;

namespace MinimSharp.Signals
{
    public abstract class Oscillator : AudioSignal
    {
        /** The float value of 2*PI. Provided as a convenience for subclasses. */
        protected static readonly float TWO_PI = (float)(2 * Math.PI);
        /** The current frequency of the oscillator. */
        private float freq;
        /** The frequency to transition to. */
        private float newFreq;
        /** The sample rate of the oscillator. */
        private float srate;
        /** The current amplitude of the oscillator. */
        private float amp;
        /** The amplitude to transition to. */
        private float newAmp;
        /** The current position in the waveform's period. */
        private float step;
        private float stepSize;
        /** The portamento state. */
        private bool port;
        /** The portamento speed in milliseconds. */
        private float portSpeed; // in milliseconds
        /**
         * The amount to increment or decrement <code>freq</code> during the
         * transition to <code>newFreq</code>.
         */
        private float portStep;
        /** The current pan position. */
        private float pan;
        /** The pan position to transition to. */
        private float newPan;
        /**
         * The amount to scale the left channel's amplitude to achieve the current pan
         * setting.
         */
        private float leftScale;
        /**
         * The amount to scale the right channel's amplitude to achieve the current
         * pan setting.
         */
        private float rightScale;

        private AudioListener listener;

        private AudioSignal ampMod;
        private AudioSignal freqMod;

        private static readonly float panAmpStep = 0.0001f;

        protected abstract float Value(float step);

        public Oscillator(float frequency, float amplitude, float sampleRate)
        {
            freq = frequency;
            newFreq = freq;
            amp = amplitude;
            newAmp = amp;
            srate = sampleRate;
            step = 0;
            stepSize = freq / (sampleRate);
            port = false;
            portStep = 0.01f;
            pan = 0;
            newPan = 0;
            leftScale = rightScale = 1;
            listener = null;
            ampMod = null;
            freqMod = null;
        }

        public float SampleRate()
        {
            return srate;
        }

        public void SetFreq(float f)
        {
            newFreq = f;
            // we want to step from freq to new newFreq in portSpeed milliseconds
            // first off, we want to divide the difference between the two freqs
            // by the number of milliseconds it's supposed to take to get there
            float msStep = (newFreq - freq) / portSpeed;
            // but since freq is incremented at every sample, we need to divide
            // again by the number of samples per millisecond
            float spms = srate / 1000;
            portStep = msStep / spms;
        }

        public float Frequency()
        {
            return freq;
        }

        public void SetAmp(float a)
        {
            newAmp = Constrain(a, 0, 1);
        }

        public float Amplitude()
        {
            return amp;
        }

        public void SetPan(float p)
        {
            newPan = Constrain(p, -1, 1);
        }

        public void SetPanNoGlide(float p)
        {
            SetPan(p);
            pan = Constrain(p, -1, 1);
        }

        public float Pan()
        {
            return pan;
        }

        public void Portamento(int millis)
        {
            if (millis <= 0)
            {
                MinimSharp.Error("Oscillator.portamento: The portamento speed must be greater than zero.");
            }
            port = true;
            portSpeed = millis;
        }

        public void NoPortamento()
        {
            port = false;
        }

        private void UpdateFreq()
        {
            if (freq != newFreq)
            {
                if (port)
                {
                    if (Math.Abs(freq - newFreq) < 0.1f)
                    {
                        freq = newFreq;
                    }
                    else
                    {
                        freq += portStep;
                    }
                }
                else
                {
                    freq = newFreq;
                }
            }
            stepSize = freq / srate;
        }

        private float Generate(float fmod, float amod)
        {
            step += fmod;
            step = step - (float)Math.Floor(step);
            return amp * amod * Value(step);
        }

        public void Generate(float[] signal)
        {
            float[] fmod = new float[signal.Length];
            float[] amod = new float[signal.Length];
            if (freqMod != null)
            {
                freqMod.Generate(fmod);
            }
            if (ampMod != null)
            {
                ampMod.Generate(amod);
            }
            for (int i = 0; i < signal.Length; i++)
            {
                // do the portamento stuff / freq updating
                UpdateFreq();
                if (ampMod != null)
                {
                    signal[i] = Generate(fmod[i], amod[i]);
                }
                else
                {
                    signal[i] = Generate(fmod[i], 1);
                }
                MonoStep();
            }
            // broadcast to listener
            if (listener != null)
            {
                listener.Samples(signal);
            }
        }

        public void Generate(float[] left, float[] right)
        {
            float[] fmod = new float[left.Length];
            float[] amod = new float[right.Length];
            if (freqMod != null)
            {
                freqMod.Generate(fmod);
            }
            if (ampMod != null)
            {
                ampMod.Generate(amod);
            }
            for (int i = 0; i < left.Length; i++)
            {
                // do the portamento stuff / freq updating
                UpdateFreq();
                if (ampMod != null)
                {
                    left[i] = Generate(fmod[i], amod[i]);
                }
                else
                {
                    left[i] = Generate(fmod[i], 1);
                }
                right[i] = left[i];
                // scale amplitude to add pan
                left[i] *= leftScale;
                right[i] *= rightScale;
                StereoStep();
            }
            if (listener != null)
            {
                listener.Samples(left, right);
            }
        }

        public void SetAudioListener(AudioListener al)
        {
            listener = al;
        }

        void SetAmplitudeModulator(AudioSignal s)
        {
            ampMod = s;
        }

        void SetFrequencyModulator(AudioSignal s)
        {
            freqMod = s;
        }

        private void MonoStep()
        {
            StepStep();
            StepAmp();
        }

        private void StereoStep()
        {
            StepStep();
            StepAmp();
            CalcLRScale();
            StepPan();
        }

        private void StepStep()
        {
            step += stepSize;
            step -= (float)Math.Floor(step);
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

        private void StepPan()
        {
            if (pan != newPan)
            {
                if (pan < newPan)
                    pan += panAmpStep;
                else
                    pan -= panAmpStep;
                if (Math.Abs(pan - newPan) < panAmpStep) pan = newPan;
            }
        }

        private void StepAmp()
        {
            if (amp != newAmp)
            {
                if (amp < newAmp)
                    amp += panAmpStep;
                else
                    amp -= panAmpStep;
                if (Math.Abs(amp - newAmp) < panAmpStep) pan = newPan;
            }
        }

        public float Period()
        {
            return 1 / freq;
        }

        float Constrain(float val, float min, float max)
        {
            return val < min ? min : (val > max ? max : val);
        }
    }
}
