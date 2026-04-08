using UnityEngine;

namespace AudioSystem
{
    public class AudioBuilder
    {
        readonly AudioManager audioManager;
        Transform parent;
        Vector3 position = Vector3.zero;
        float targetVolume = 1f;
        bool isOverrideVolume = false;

        float newPitch = 1f;
        bool additivePitch = false;
        bool randomPitch = false;
        float minRandomPitchRange = -0.1f;
        float maxRandomPitchRange = 0.1f;

        bool reverb = false;
        bool loop = false;

        bool fadeIn = false;
        float fadeDuration = 0.2f;
        bool dynamic = false;

        public AudioBuilder(AudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        public AudioBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }

        public AudioBuilder WithParent(Transform parent)
        {
            this.parent = parent;
            return this;
        }

        public AudioBuilder WithRandomPitch(float min = -0.1f, float max = 0.1f)
        {
            this.randomPitch = true;
            this.minRandomPitchRange = min;
            this.maxRandomPitchRange = max;
            return this;
        }

        public AudioBuilder WithAdditivePitch(float pitch)
        {
            this.additivePitch = true;
            this.newPitch = pitch;
            return this;
        }

        public AudioBuilder WithReverb()
        {
            this.reverb = true;
            return this;
        }

        public AudioBuilder WithLoop()
        {
            this.loop = true;
            return this;
        }

        public AudioBuilder WithDynamic()
        {
            this.dynamic = true;
            return this;
        }

        public AudioBuilder WithFadeIn(float fadeDuration = 0.2f)
        {
            this.fadeIn = true;
            this.fadeDuration = fadeDuration;
            return this;
        }

        public AudioBuilder WithVolume(float volume)
        {
            this.targetVolume = volume;
            this.isOverrideVolume = true;
            return this;
        }

        public AudioEmitter Play(AudioData audioData, bool retain = false)
        {
            if (audioData == null)
            {
                //Debug.LogWarning("SoundData is null");
                return null;
            }

            if (!audioManager.CanPlaySound(audioData)) return null;

            AudioEmitter audioEmitter = audioManager.Get();
            audioEmitter.Initialize(audioData);
            audioEmitter.transform.position = position;
            if (parent != null)
            {
                audioEmitter.transform.parent = parent;
            }
            else
            {
                audioEmitter.transform.parent = audioManager.transform;
            }

            if (randomPitch)
            {
                audioEmitter.WithRandomPitch(minRandomPitchRange, maxRandomPitchRange);
            } 
            else if (additivePitch)
            {
                audioEmitter.WithAdditivePitch(newPitch);
            }

            if (loop)
            {
                audioEmitter.WithLoop();
            }

            if (reverb)
            {
                audioEmitter.WithReverb();
            }

            float volume = isOverrideVolume ? targetVolume : audioData.volume;
            audioEmitter.WithVolume(volume, fadeIn, fadeDuration);

            if(dynamic)
            {
                audioEmitter.WithDynamic(volume, audioData.pitch);
            }

            if (audioData.frequentSound)
            {
                audioEmitter.Node = audioManager.FrequentAudioEmitters.AddLast(audioEmitter);
            }

            audioEmitter.Play(retain);

            return audioEmitter;
        }
    }
}