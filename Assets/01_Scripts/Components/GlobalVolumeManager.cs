using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Utilities;

public class GlobalVolumeManager : NonPersistentSingleton<GlobalVolumeManager>
{
    private Volume volume;
    private Vignette vignette;

    //vignette pulse
    [field: SerializeField] public float StartIntensity { get; private set; } = 0.2f;
    [field: SerializeField] public float StartSmoothness { get; private set; } = 0.2f;
    private float targetIntensity;

    private float pulseTime;
    private bool isPulsing;

    protected override void Awake()
    {
        base.Awake();
        volume = GetComponent<Volume>();
        if (volume.profile.TryGet(out vignette)) // Get Vignette effect
        {
            vignette.intensity.overrideState = true;
            vignette.smoothness.overrideState = true;
            vignette.color.overrideState = true;
        }
    }

    public void Update()
    {
        if (!isPulsing || vignette == null) return;

        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetIntensity, 5f * Time.deltaTime);

        pulseTime -= Time.deltaTime;
        if (pulseTime <= 0f)
        {
            ResetVignette();
        }

        // Stop pulsing when close enough to target
        if (Mathf.Abs(vignette.intensity.value - targetIntensity) < 0.01f)
        {
            isPulsing = false;
        }
    }

    public void VignettePulse(float intensity, float smoothness, float duration, Color color)
    {
        if (vignette == null) return;

        targetIntensity = intensity;
        vignette.smoothness.value = smoothness;
        vignette.color.value = color;
        pulseTime = duration;
        isPulsing = true;
    }

    public void ResetVignette()
    {
        if (vignette == null) return;

        vignette.intensity.value = StartIntensity;
        vignette.smoothness.value = StartSmoothness;
        vignette.color.value = Color.black;
        isPulsing = false;
    }

    public void Blur()
    {

    }

    public void Unblur()
    {
    }

    public void ResetAll()
    {
        ResetVignette();
        Unblur();
    }
}
