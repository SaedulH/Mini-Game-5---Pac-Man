using System;
using System.Collections;
using AudioSystem;
using UnityEngine;
using Utilities;

public class MusicManager : NonPersistentSingleton<MusicManager>
{
    [field: SerializeField] public float MusicFadeInTime { get; private set; } = 0.5f;
    [field: SerializeField] public AudioData DefaultBGM { get; private set; }

    private AudioEmitter _currentEmitter;
    private AudioEmitter _nextEmitter;

    private float _currentVolume;

    public void Initialise()
    {
        PlayMusic(DefaultBGM);
    }

    public void PlayMusic(AudioData audioData)
    {
        StartCoroutine(PlayMusicCoroutine(audioData));
    }

    public void StopMusic()
    {
        StartCoroutine(StopMusicCoroutine());
    }

    public IEnumerator PlayMusicCoroutine(AudioData audioData)
    {
        if (_currentEmitter != null)
        {
            Debug.Log($"Stopping current music");

            _currentEmitter.FadeToStop(MusicFadeInTime);
            yield return new WaitForSeconds(MusicFadeInTime);
            Debug.Log($"current music Stopped");
        }
        Debug.Log($"Playing music: {audioData.name}");
        _nextEmitter = AudioManager.Instance.CreateAudioBuilder()
            .WithParent(transform)
            .WithLoop()
            .WithFadeIn()
            .Play(audioData);

        _currentEmitter = _nextEmitter;
        _nextEmitter = null;
    }

    public IEnumerator StopMusicCoroutine()
    {
        if (_currentEmitter != null)
        {
            _currentEmitter.FadeToStop();
            while (_currentEmitter.IsPlaying())
            {
                yield return null;
            }
        }

        _currentEmitter = null;
    }

    public void SetTempVolume(float tempVolume, bool isPercentage)
    {
        if (_currentEmitter != null)
        {
            _currentVolume = _currentEmitter.GetVolume();
            float newVolume = isPercentage ? _currentVolume * tempVolume : tempVolume;
            _currentEmitter.WithVolume(newVolume);
        }
    }

    public void ReturnFromTempVolume()
    {
        if (_currentEmitter != null)
        {
            float newVolume = _currentVolume > 0f ? _currentVolume : _currentEmitter.Data.volume;
            _currentEmitter.WithVolume(newVolume);
        }
    }
}
