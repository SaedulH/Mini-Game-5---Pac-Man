using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using Utilities;

namespace SettingsSystem
{
    public class AudioSettings : SettingsTab
    {
        private Dictionary<AudioGroup, SliderInt> _sliders;
        private Dictionary<AudioGroup, Coroutine> _sliderRoutines = new();

        [field: SerializeField] public AudioMixer AudioMixer { get; set; }
        [field: SerializeField] public int DefaultVolume { get; private set; } = 50;
        [field: SerializeField] public float SliderLerpSpeed { get; private set; } = 50f;

        public override void InitialiseSettings(VisualElement root)
        {
            TabElement = root.Q<Tab>("Audio");

            _sliders = new Dictionary<AudioGroup, SliderInt>
            {
                { AudioGroup.Master, TabElement.Q<SliderInt>("MasterVolume") },
                { AudioGroup.Music, TabElement.Q<SliderInt>("MusicVolume") },
                { AudioGroup.UI, TabElement.Q<SliderInt>("UIVolume") },
                { AudioGroup.Effects, TabElement.Q<SliderInt>("EffectsVolume") }
            };

            foreach (var (group, slider) in _sliders)
            {
                if (slider == null)
                {
                    Debug.LogError($"Slider for {group} is missing!");
                    continue;
                }
                slider.RegisterValueChangedCallback(e =>
                {
                    OnVolumeChanged(group, e.newValue);
                });
            }

            TabElement.AddToClassList("hide");
        }

        protected override void GetSettings()
        {
            foreach (var (group, slider) in _sliders)
            {
                int value = GetVolumeSetting(group);
                //slider.SetValueWithoutNotify(value);
                StartSliderCoroutine(group, slider, value);
            }
        }

        private void OnVolumeChanged(AudioGroup group, int value)
        {
            SetVolumeSetting(group, value);
        }

        private void SetVolumeSetting(AudioGroup group, int value)
        {
            PlayerPrefs.SetInt(group.ToString(), value);
            SetMixerVolume(group.ToString(), value);
        }

        private int GetVolumeSetting(AudioGroup audioGroup)
        {
            return PlayerPrefs.GetInt(audioGroup.ToString(), DefaultVolume);
        }

        private void SetMixerVolume(string mixerGroup, int channelVolume)
        {
            if (AudioMixer == null) return;

            float channel = channelVolume / 100f;

            if (channel <= 0.0001f)
            {
                AudioMixer.SetFloat(mixerGroup, -80f);
                return;
            }

            float db = Mathf.Log10(channel) * 20f;
            AudioMixer.SetFloat(mixerGroup, db);
        }

        private IEnumerator SetSliderValue(SliderInt slider, int value)
        {
            float current = slider.value;

            while (!Mathf.Approximately(current, value))
            {
                current = Mathf.Lerp(current, value, Time.deltaTime * SliderLerpSpeed);
                slider.SetValueWithoutNotify(Mathf.RoundToInt(current));
                yield return null;
            }

            slider.SetValueWithoutNotify(value);
        }

        private void StartSliderCoroutine(AudioGroup group, SliderInt slider, int value)
        {
            if (_sliderRoutines.TryGetValue(group, out var routine) && routine != null)
            {
                StopCoroutine(routine);
                _sliderRoutines[group] = null;
            }

            _sliderRoutines[group] = StartCoroutine(SetSliderValue(slider, value));
        }

        public override void ResetToDefaults()
        {
            PlayResetAudio();

            foreach (var (group, slider) in _sliders)
            {
                SetVolumeSetting(group, DefaultVolume);

                StartSliderCoroutine(group, slider, DefaultVolume);
            }
        }
    }
}