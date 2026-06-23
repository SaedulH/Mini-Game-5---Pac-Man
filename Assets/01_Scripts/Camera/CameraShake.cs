using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

public class CameraShake : NonPersistentSingleton<CameraShake>
{
    private CinemachineCamera cinemachineCamera;
    private Coroutine _shakeCoroutine = null;
    private float intensityModifier = 0f;
    private Vector3 initialPosition = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    public async Task SetupScreenShake(string screenShakeSetting)
    {
        UpdateScreenShakeIntensityModifier(screenShakeSetting);
        Debug.Log($"Screen Shake Intensity Modifier set to: {intensityModifier}");
        initialPosition = cinemachineCamera.transform.localPosition;
        await Task.CompletedTask;
    }

    public void UpdateScreenShakeIntensityModifier(string screenShakeSetting)
    {
        if (Enum.TryParse(screenShakeSetting, out ScreenShake parsedScreenShakeSetting))
        {
            intensityModifier = parsedScreenShakeSetting switch
            {
                ScreenShake.Off => 0f,
                ScreenShake.Low => 1f,
                ScreenShake.High => 1.5f,
                _ => 1f,
            };
        }
        else
        {
            intensityModifier = 1f;
        }
    }

    public void ShakeCamera(float intensity, float duration)
    {
        if (cinemachineCamera == null) return;

        if(intensityModifier <= 0f) return;

        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            cinemachineCamera.transform.localPosition = initialPosition;
        }
        StartCoroutine(ShakeCameraCoroutine((intensity * intensityModifier), duration));
    }

    private IEnumerator ShakeCameraCoroutine(float intensity, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.25f, 0.25f) * intensity;
            float yOffset = Random.Range(-0.25f, 0.25f) * intensity;
            cinemachineCamera.transform.localPosition = initialPosition + new Vector3(xOffset, yOffset, 0f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cinemachineCamera.transform.localPosition = initialPosition;
    }
}