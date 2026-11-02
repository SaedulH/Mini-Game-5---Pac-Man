using AudioSystem;
using System.Collections;
using UnityEngine;
using Utilities;

public class MusicManager : NonPersistentSingleton<MusicManager>
{
    [field: SerializeField] public float MusicFadeInTime { get; private set; } = 0.5f;
    [field: SerializeField] public AudioData DefaultBGM { get; private set; }
    [field: SerializeField] public AudioData GameOverBGM { get; private set; }

    private AudioEmitter _currentEmitter;
    private Coroutine _musicRoutine;

    private float _currentVolume;

    public void Initialise()
    {
        //PlayMusic(DefaultBGM);
    }

    public void PlayMusic(AudioData audioData)
    {
        // Only one music transition at a time - a new request replaces any in-flight one.
        if (_musicRoutine != null)
        {
            StopCoroutine(_musicRoutine);
        }
        _musicRoutine = StartCoroutine(PlayMusicCoroutine(audioData));
    }

    public void PlayGameOverMusic()
    {
        PlayMusic(GameOverBGM);
    }

    public void StopMusic()
    {
        if (_musicRoutine != null)
        {
            StopCoroutine(_musicRoutine);
        }
        _musicRoutine = StartCoroutine(StopMusicCoroutine());
    }

    private IEnumerator PlayMusicCoroutine(AudioData audioData)
    {
        // Hand the old track off so it fades out on its own, even if this routine gets replaced.
        AudioEmitter previous = _currentEmitter;
        _currentEmitter = null;

        if (previous != null && previous.isActiveAndEnabled)
        {
            Debug.Log($"Stopping current music (timeScale: {Time.timeScale})");
            previous.FadeToStop(MusicFadeInTime);

            // Realtime: music transitions must not depend on Time.timeScale (pause sets it to 0).
            yield return new WaitForSecondsRealtime(MusicFadeInTime);
            Debug.Log("Current music stopped");
        }

        if (audioData == null)
        {
            Debug.LogWarning("PlayMusic called with no AudioData");
            _musicRoutine = null;
            yield break;
        }

        Debug.Log($"Playing music: {audioData.name}");
        _currentEmitter = AudioManager.Instance.CreateAudioBuilder()
            .WithParent(transform)
            .WithLoop()
            .WithFadeIn()
            .Play(audioData);

        _musicRoutine = null;
    }

    private IEnumerator StopMusicCoroutine()
    {
        AudioEmitter previous = _currentEmitter;
        _currentEmitter = null;

        if (previous != null && previous.isActiveAndEnabled)
        {
            previous.FadeToStop(MusicFadeInTime);
            yield return new WaitForSecondsRealtime(MusicFadeInTime);
        }

        _musicRoutine = null;
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
