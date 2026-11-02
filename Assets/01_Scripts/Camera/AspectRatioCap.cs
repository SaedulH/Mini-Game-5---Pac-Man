using CoreSystem;
using UnityEngine;
using Utilities;

/// <summary>
/// Caps the main camera to a maximum aspect ratio (16:9 by default) in map scenes, adding black borders left and right on wider screens.
/// </summary>
[RequireComponent(typeof(Camera))]
public class AspectRatioCap : MonoBehaviour
{
    [field: SerializeField] public float MaxAspect { get; private set; } = 16f / 9f;

    private Camera _camera;
    private Camera _borderCamera;
    private bool _isCapped;
    private Vector2Int _lastScreenSize;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        CreateBorderCamera();
        UpdateViewport();
    }

    private void LateUpdate()
    {
        // Only cap in map scenes, the main menu keeps the full screen
        LevelContext context = GameManager.HasInstance ? GameManager.Instance.CurrentLevelContext : null;
        bool shouldCap = context != null && !context.MapName.Equals(MapName.Menu);

        // Re-apply when entering/leaving a map, or when the window is resized
        if (shouldCap != _isCapped || _lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
        {
            _isCapped = shouldCap;
            UpdateViewport();
        }
    }

    private void UpdateViewport()
    {
        _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        if (Screen.height <= 0) return;

        float screenAspect = (float)Screen.width / Screen.height;
        bool needsBorders = _isCapped && screenAspect > MaxAspect;

        _borderCamera.enabled = needsBorders;
        if (!needsBorders)
        {
            _camera.rect = new Rect(0f, 0f, 1f, 1f);
            return;
        }

        float width = MaxAspect / screenAspect;
        _camera.rect = new Rect((1f - width) / 2f, 0f, width, 1f);
    }

    private void CreateBorderCamera()
    {
        // Renders nothing but black behind the main camera, so the borders are always cleared
        GameObject borderObject = new("BorderCamera");
        borderObject.transform.SetParent(transform, false);

        _borderCamera = borderObject.AddComponent<Camera>();
        _borderCamera.clearFlags = CameraClearFlags.SolidColor;
        _borderCamera.backgroundColor = Color.black;
        _borderCamera.cullingMask = 0;
        _borderCamera.depth = _camera.depth - 1;
        _borderCamera.enabled = false;
    }
}
