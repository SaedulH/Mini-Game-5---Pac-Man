using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour
    {
        public AudioData Data { get; private set; }
        public LinkedListNode<AudioEmitter> Node { get; set; }

        AudioSource audioSource;
        private Coroutine currentCoroutine;

        [field: SerializeField] private bool isFadingOut = false;
        [field: SerializeField] private bool isDynamic = false;
        [field: SerializeField] private float targetVolume = 0f;
        [field: SerializeField] private float targetPitch = 0f;

        void Awake()
        {
            audioSource = gameObject.GetOrAdd<AudioSource>();
        }

        private void Update()
        {
            if (currentCoroutine != null || !audioSource.isPlaying) return;

            if (isDynamic)
            {
                audioSource.volume = SetDynamicValue(audioSource.volume, targetVolume, Constants.DYNAMIC_VOLUME_LERP_SPEED);
                audioSource.pitch = SetDynamicValue(audioSource.pitch, targetPitch, Constants.DYNAMIC_PITCH_LERP_SPEED);
            }
        }

        private float SetDynamicValue(float current, float target, float lerpSpeed)
        {
            if (!Mathf.Approximately(current, target))
            {
                current = Mathf.MoveTowards(current, target, Time.deltaTime * lerpSpeed);
            }
            return current;
        }

        public void Initialize(AudioData data)
        {
            Data = data;
            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.mixerGroup;
            audioSource.loop = data.loop;
            audioSource.playOnAwake = data.playOnAwake;

            audioSource.mute = data.mute;
            audioSource.bypassEffects = data.bypassEffects;
            audioSource.bypassListenerEffects = data.bypassListenerEffects;
            audioSource.bypassReverbZones = data.bypassReverbZones;

            audioSource.priority = data.priority;
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
            audioSource.panStereo = data.panStereo;
            audioSource.spatialBlend = data.spatialBlend;
            audioSource.reverbZoneMix = data.reverbZoneMix;
            audioSource.dopplerLevel = data.dopplerLevel;
            audioSource.spread = data.spread;

            audioSource.minDistance = data.minDistance;
            audioSource.maxDistance = data.maxDistance;

            audioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            audioSource.ignoreListenerPause = data.ignoreListenerPause;

            audioSource.rolloffMode = data.rolloffMode;
        }

        #region Playback

        public void BeginNewCoroutine(IEnumerator coroutine)
        {
            StopCurrentCoroutine();
            currentCoroutine = StartCoroutine(coroutine);
        }

        public void Play(bool retain = false)
        {
            if (audioSource.isPlaying) return;

            audioSource.Play();
            if (!retain && !audioSource.loop)
            {
                StartCoroutine(WaitForEnd());
            }
        }

        //public void Resume(float fadeInDuration = 0f)
        //{
        //    if (audioSource.isPlaying) return;

        //    audioSource.Play();
        //    if (fadeInDuration > 0f)
        //    {
        //        BeginNewCoroutine(FadeVolume(0f, Data.volume, fadeInDuration));
        //    }
        //}

        public void Pause()
        {
            StopCurrentCoroutine();
            audioSource.Pause();
        }

        public void Stop(bool retain = false)
        {
            StopCurrentCoroutine();
            audioSource.Stop();
            if (!retain)
            {
                AudioManager.Instance.ReturnToPool(this);
            }
        }

        #endregion

        #region Fading

        public void DynamicVolume(float targetVolume)
        {
            if (!isDynamic) return;
            StopCurrentCoroutine();

            this.targetVolume = targetVolume;
        }

        public void DynamicPitch(float targetPitch)
        {
            if (!isDynamic) return;
            StopCurrentCoroutine();

            this.targetPitch = targetPitch;
        }

        public void FadeToPause(float duration = 0.5f)
        {
            if (isFadingOut) return;

            isFadingOut = true;
            BeginNewCoroutine(FadeOutThen(duration, Pause));
        }

        public void FadeToStop(float duration = 0.5f, bool retain = false)
        {
            if (isFadingOut) return;

            isFadingOut = true;
            BeginNewCoroutine(FadeOutThen(duration, () => Stop(retain)));
        }
         
        private IEnumerator FadeOutThen(float duration, System.Action onComplete)
        {
            yield return FadeVolume(audioSource.volume, 0f, duration);
            onComplete?.Invoke();
        }

        private IEnumerator FadeVolume(float start, float target, float duration)
        {
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, target, time / duration);
                yield return null;
            }

            audioSource.volume = target;
        }

        #endregion

        #region Helpers

        private IEnumerator WaitForEnd()
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            Stop();
        }

        private void StopCurrentCoroutine()
        {
            if (currentCoroutine == null) return;
            isFadingOut = false;
            //Debug.Log("Stopping current coroutine");
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        public bool IsPlaying() => audioSource.isPlaying;
        public float GetVolume() => audioSource.volume;

        #endregion

        #region Modifiers

        public void WithRandomPitch(float min = -0.1f, float max = 0.1f)
        {
            audioSource.pitch += Random.Range(min, max);
        }

        public void WithAdditivePitch(float pitch)
        {
            audioSource.pitch += pitch;
        }

        public void WithPitch(float pitch, bool fade = false, float duration = 0.1f)
        {
            audioSource.pitch = pitch;
        }

        public void WithLoop()
        {
            audioSource.loop = true;
        }

        public void WithReverb(float min = -0.1f, float max = 0.1f)
        {
            audioSource.reverbZoneMix += Random.Range(min, max);
        }

        public void WithDynamic(float targetVolume = 1f, float targetPitch = 1f)
        {
            isDynamic = true;
            this.targetVolume = targetVolume;
            this.targetPitch = targetPitch;
        }

        public void WithFadeIn(float duration = 0.2f)
        {
            BeginNewCoroutine(FadeVolume(0f, Data.volume, duration));
        }

        public void WithVolume(float target, bool fadeIn = false, float duration = 0.2f)
        {
            //Debug.Log($"targetVolume: {target}, fade: {fade}, duration: {duration}");
            if (!fadeIn)
            {
                audioSource.volume = target;
                return;
            }
            BeginNewCoroutine(FadeVolume(0f, target, duration));
        }

        #endregion
    }
}