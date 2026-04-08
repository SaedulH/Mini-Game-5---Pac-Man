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

    protected override void Awake()
    {
        base.Awake();
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    public async Task SetupScreenShake(string screenShakeSetting)
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
        Debug.Log($"Screen Shake Intensity Modifier set to: {intensityModifier}");
        await Task.CompletedTask;
    }

    public void ShakeCamera(float intensity, float duration)
    {
        if (cinemachineCamera == null) return;

        if(intensityModifier <= 0f) return;

        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            cinemachineCamera.transform.localPosition = new Vector3(0f, 0f, -5f);
        }
        StartCoroutine(ShakeCameraCoroutine((intensity * intensityModifier), duration));
    }

    private IEnumerator ShakeCameraCoroutine(float intensity, float duration)
    {
        Vector3 originalPosition = new Vector3(0f, 0f, -5f);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-0.5f, 0.5f) * intensity;
            float yOffset = Random.Range(-0.5f, 0.5f) * intensity;
            cinemachineCamera.transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cinemachineCamera.transform.localPosition = originalPosition;
    }
}