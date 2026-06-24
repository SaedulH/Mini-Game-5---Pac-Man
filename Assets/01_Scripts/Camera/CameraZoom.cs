using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class CameraZoom : NonPersistentSingleton<CameraZoom>
{
    private CinemachineCamera _cinemachineCamera;
    private CinemachinePositionComposer _positionComposer;
    private CinemachineConfiner3D _confiner;
    [field: SerializeField] public BoxCollider ConfinerCollider { get; set; }
    [field: SerializeField] public List<Transform> TrackingTargets { get; private set; } = new List<Transform>();
    [field: SerializeField] public GameObject GroupCentre { get; private set; }
    [field: SerializeField] public GameObject Pacman { get; private set; }
    [field: SerializeField] public GameObject[] Ghosts { get; private set; } = new GameObject[4];

    private float maxWidth;
    private float maxHeight;
    private float maxDistance;

    private float _defaultOrthoSize;
    private float _targetOrthoSize;
    private float _currentOrthoSize;
    private float _zoomTime;
    private bool _isTimedZooming;
    private bool _isTrackGroupCentre;

    protected override void Awake()
    {
        base.Awake();
        _cinemachineCamera = GetComponent<CinemachineCamera>();
        _defaultOrthoSize = _cinemachineCamera.Lens.OrthographicSize;
        _positionComposer = gameObject.GetOrAdd<CinemachinePositionComposer>();
        _confiner = gameObject.GetOrAdd<CinemachineConfiner3D>();
        Ghosts = new GameObject[4];
    }

    private void Update()
    {
        if (_isTrackGroupCentre)
        {
            TrackTargetGroup();
        }
    }

    private void TrackTargetGroup()
    {
        if (Pacman == null)
            return;

        Bounds bounds = new Bounds(
            Pacman.transform.position,
            Vector3.zero);

        foreach (GameObject ghost in Ghosts)
        {
            if (ghost == null)
                continue;

            bounds.Encapsulate(ghost.transform.position);
        }

        UpdateZoom(bounds);
    }

    private void UpdateZoom(Bounds bounds)
    {
        float width = bounds.size.x;
        float height = bounds.size.z;

        float widthFactor =
            Constants.ZOOM_FACTOR_CONSTANT +
            Mathf.InverseLerp(0f, maxWidth, width);

        float heightFactor =
            Constants.ZOOM_FACTOR_CONSTANT +
            Mathf.InverseLerp(0f, maxHeight, height);

        float zoomFactor = Mathf.Max(
            widthFactor,
            heightFactor);

        float targetFOV = Mathf.Clamp(
            zoomFactor * Constants.MAX_CAMERA_SIZE,
            Constants.MIN_CAMERA_SIZE,
            Constants.MAX_CAMERA_SIZE);

        _cinemachineCamera.Lens.FieldOfView =
            Mathf.Lerp(
                _cinemachineCamera.Lens.FieldOfView,
                targetFOV,
                Time.deltaTime * 5f);
    }

    private void TrackTargetGroup1()
    {
        if (TrackingTargets == null || TrackingTargets.Count == 0)
        {
            return;
        }

        Vector3 centre = Vector3.zero;
        Bounds bounds = new Bounds(TrackingTargets[0].position, Vector3.zero);

        foreach (GameObject target in Ghosts)
        {
            centre += target.transform.position;
            bounds.Encapsulate(target.transform.position);
        }

        centre /= TrackingTargets.Count;

        GroupCentre.transform.position = centre;

        float spread = Mathf.Max(
            bounds.size.x,
            bounds.size.z);

        float targetFOV = Mathf.Lerp(
            Constants.MIN_CAMERA_SIZE,
            Constants.MAX_CAMERA_SIZE,
            Mathf.InverseLerp(Constants.MIN_CAMERA_SIZE, Constants.MAX_CAMERA_SIZE, spread));

        _cinemachineCamera.Lens.FieldOfView =
            Mathf.Lerp(
                _cinemachineCamera.Lens.FieldOfView,
                targetFOV,
                Time.deltaTime * 5f);
    }

    private void TimedZoom()
    {
        _cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(_cinemachineCamera.Lens.OrthographicSize, _targetOrthoSize, 5f * Time.deltaTime);
        if (_zoomTime > 0f)
        {
            _zoomTime -= Time.deltaTime;
            if (_zoomTime <= 0f)
            {
                ResetZoom();
            }
        }

        if (Mathf.Abs(_cinemachineCamera.Lens.OrthographicSize - _targetOrthoSize) < 0.1f)
        {
            _isTimedZooming = false;
        }
    }

    public void AddPacmanToCameraTarget(GameObject playerObject)
    {
        if (playerObject == null) return;
        Pacman = playerObject;
    }

    public void AddGhostToCameraTarget(int index, GameObject ghostObject)
    {
        if (ghostObject == null) return;

        Ghosts ??= new GameObject[4];
        Ghosts[index] = ghostObject;
    }

    public async Task SetupCameraMode(string cameraMode)
    {
        if (_cinemachineCamera == null) return;

        UpdateCameraMode(cameraMode);
        Debug.Log($"Camera Mode set to: {cameraMode}");
        await Task.CompletedTask;
    }

    public async void UpdateCameraMode(string cameraMode)
    {
        if (Enum.TryParse(cameraMode, out CameraMode parsedCameraMode))
        {
            switch (parsedCameraMode)
            {
                case CameraMode.Fixed:
                default:
                    await SetupFixedCameraMode();
                    break;
                case CameraMode.Dynamic:
                    await SetupDynamicCameraMode();
                    break;
                case CameraMode.Follow:
                    await SetupFollowCameraMode();
                    break;
            }
        }
        else
        {
            await SetupFixedCameraMode();
        }
    }

    private async Task SetupFixedCameraMode()
    {
        _isTrackGroupCentre = false;

        _cinemachineCamera.Target.TrackingTarget = null;
        _cinemachineCamera.Target.LookAtTarget = null;

        _cinemachineCamera.LookAt = null;
        _cinemachineCamera.Lens.FieldOfView = Constants.FIXED_CAMERA_FOV;

        transform.SetPositionAndRotation(
            Constants.FIXED_CAMERA_POSITION,
            Quaternion.Euler(Constants.FIXED_CAMERA_ROTATION));

        if (_positionComposer != null)
        {
            _positionComposer.CameraDistance = Constants.FIXED_CAMERA_DISTANCE;
            _positionComposer.enabled = false;
        }
        await Task.CompletedTask;
    }

    private async Task SetupDynamicCameraMode()
    {
        _isTrackGroupCentre = true;

        TrackingTargets.Clear();
        if (Pacman != null)
        {
            TrackingTargets.Add(Pacman.transform);
        }

        if (Ghosts != null)
        {
            foreach (var ghost in Ghosts)
            {
                if (ghost != null)
                {
                    TrackingTargets.Add(ghost.transform);
                }
            }
        }

        transform.SetPositionAndRotation(
            Constants.FIXED_CAMERA_POSITION,
            Quaternion.Euler(Constants.FIXED_CAMERA_ROTATION));

        if (Pacman != null)
        {
            _cinemachineCamera.Target.TrackingTarget = Pacman.transform;
            _cinemachineCamera.Target.LookAtTarget = Pacman.transform;
        }
        if (_positionComposer != null)
        {
            _positionComposer.enabled = true;
            _positionComposer.CameraDistance = Constants.FIXED_CAMERA_DISTANCE;
            _positionComposer.Lookahead.Enabled = true;
            _positionComposer.Lookahead.Time = Constants.FOLLOW_CAMERA_LOOK_AHEAD_TIME;
            _positionComposer.Lookahead.Smoothing = Constants.FOLLOW_CAMERA_LOOK_AHEAD_SMOOTHING;
        }
        if (_confiner != null)
        {
            _confiner.enabled = true;
            if (ConfinerCollider != null)
            {
                SetupConfinerCollider();
            }
        }
        await Task.CompletedTask;
    }

    private async Task SetupFollowCameraMode()
    {
        _isTrackGroupCentre = false;

        if (Pacman != null)
        {
            _cinemachineCamera.Target.TrackingTarget = Pacman.transform;
            _cinemachineCamera.Target.LookAtTarget = Pacman.transform;
        }
        _cinemachineCamera.Lens.FieldOfView = Constants.FIXED_CAMERA_FOV;

        transform.SetPositionAndRotation(
            Constants.FIXED_CAMERA_POSITION,
            Quaternion.Euler(Constants.FIXED_CAMERA_ROTATION));

        if (_positionComposer != null)
        {
            _positionComposer.enabled = true;
            _positionComposer.CameraDistance = Constants.FIXED_CAMERA_DISTANCE;
            _positionComposer.Lookahead.Enabled = true;
            _positionComposer.Lookahead.Time = Constants.FOLLOW_CAMERA_LOOK_AHEAD_TIME;
            _positionComposer.Lookahead.Smoothing = Constants.FOLLOW_CAMERA_LOOK_AHEAD_SMOOTHING;
        }
        if (_confiner != null)
        {
            _confiner.enabled = true;
            if (ConfinerCollider != null)
            {
                SetupConfinerCollider();
            }
        }

        await Task.CompletedTask;
    }

    private void SetupConfinerCollider()
    {
        //ConfinerCollider.isTrigger = true;
        //_cinemachineCamera.Lens.OrthographicSize = Constants.MIN_ORTHOGRAPHIC_CAMERA_SIZE;

        //maxHeight = Constants.MAX_CAMERA_SIZE * 2f;
        //maxWidth = maxHeight * mainCamera.aspect;
        //maxDistance = Mathf.Sqrt((maxHeight * maxHeight) + (maxWidth * maxWidth));

        //float halfHeight = (maxHeight / 2f) + 0.1f;
        //float halfWidth = (maxWidth / 2f) + 0.1f;

        //Vector2[] points = new Vector2[]
        //{
        //    new(-halfWidth, halfHeight),
        //    new(-halfWidth, -halfHeight),
        //    new(halfWidth, -halfHeight),
        //    new(halfWidth, halfHeight)
        //};

        //ConfinerCollider.SetPath(0, points);
    }

    public async Task ResetCameraZoom()
    {
        if (_cinemachineCamera == null)
        {
            GetComponent<CinemachineCamera>();
        }
        _defaultOrthoSize = _cinemachineCamera.Lens.OrthographicSize;
        await Task.CompletedTask;
    }

    public void ZoomWithTargetAndDuration(float distance, Transform target, float time)
    {
        _targetOrthoSize = _defaultOrthoSize - distance; // Zoom in by reducing FOV
        _zoomTime = time;
        _cinemachineCamera.LookAt = target;
        _isTimedZooming = true;
    }

    public void ResetZoom(float resetTime = 0.5f)
    {
        _targetOrthoSize = _defaultOrthoSize;

        _zoomTime = (resetTime > 0f) ? resetTime : _zoomTime; // Use last zoom time if not specified
        _isTimedZooming = true;
    }
}
