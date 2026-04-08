using System;
using System.Collections;
using AudioSystem;
using UnityEngine;
using Utilities;

public class MusicManager : NonPersistentSingleton<MusicManager>
{
    [field: SerializeField] public AudioData DefaultBGM { get; set; }

    private AudioEmitter _currentEmitter;
    private AudioEmitter _nextEmitter;
    private void Start()
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

            _currentEmitter.FadeToStop(Constants.AUDIO_MUSIC_FADE_IN_TIME);
            yield return new WaitForSeconds(Constants.AUDIO_MUSIC_FADE_IN_TIME);
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
}
