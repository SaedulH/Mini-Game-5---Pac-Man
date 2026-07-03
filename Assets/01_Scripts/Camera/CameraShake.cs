using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraShake : NonPersistentSingleton<CameraShake>
{
    private CinemachineCamera _cinemachineCamera;
    private Coroutine _shakeCamera2DCoroutine = null;
    private float _intensityModifier = 0f;
    private Vector3 _initialPosition = Vector3.zero;

    private float _shakeTimer3D = 0f;

    protected override void Awake()
    {
        base.Awake();
        _cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        _shakeTimer3D -= Time.deltaTime;
        if (_shakeTimer3D < 0f)
        {
            CinemachineBasicMultiChannelPerlin m_MultiChannelPerlin =
                (CinemachineBasicMultiChannelPerlin)_cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise);
            if (m_MultiChannelPerlin != null)
            {
                m_MultiChannelPerlin.AmplitudeGain = 0f;
            }
        }
    }

    public async Task SetupScreenShake(string screenShakeSetting)
    {
        UpdateScreenShakeIntensityModifier(screenShakeSetting);
        Debug.Log($"Screen Shake Intensity Modifier set to: {_intensityModifier}");
        _initialPosition = _cinemachineCamera.transform.localPosition;
        await Task.CompletedTask;
    }

    public void UpdateScreenShakeIntensityModifier(string screenShakeSetting)
    {
        if (Enum.TryParse(screenShakeSetting, out ScreenShake parsedScreenShakeSetting))
        {
            _intensityModifier = parsedScreenShakeSetting switch
            {
                ScreenShake.Off => 0f,
                ScreenShake.Low => 1f,
                ScreenShake.High => 1.5f,
                _ => 1f,
            };
        }
        else
        {
            _intensityModifier = 1f;
        }
    }

    public void ShakeCamera3D(float intensity, float duration)
    {
        if (_intensityModifier <= 0f) return;

        CinemachineBasicMultiChannelPerlin m_MultiChannelPerlin =
            (CinemachineBasicMultiChannelPerlin)_cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise);
        if (m_MultiChannelPerlin != null)
        {
            m_MultiChannelPerlin.AmplitudeGain = intensity * _intensityModifier;
            _shakeTimer3D = duration;
        }
    }

    public void ShakeCamera2D(float intensity, float duration)
    {
        if (_cinemachineCamera == null) return;

        if(_intensityModifier <= 0f) return;

        if (_shakeCamera2DCoroutine != null)
        {
            StopCoroutine(_shakeCamera2DCoroutine);
            _cinemachineCamera.transform.localPosition = _initialPosition;
        }
        StartCoroutine(ShakeCamera2DCoroutine((intensity * _intensityModifier), duration));
    }

    private IEnumerator ShakeCamera2DCoroutine(float intensity, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.25f, 0.25f) * intensity;
            float yOffset = Random.Range(-0.25f, 0.25f) * intensity;
            _cinemachineCamera.transform.localPosition = _initialPosition + new Vector3(xOffset, yOffset, 0f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _cinemachineCamera.transform.localPosition = _initialPosition;
    }
}