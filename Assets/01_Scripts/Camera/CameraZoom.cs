using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

public class CameraZoom : NonPersistentSingleton<CameraZoom>
{
    private CinemachineCamera _cinemachineCamera;
    private CinemachinePositionComposer _positionComposer;
    private CinemachineConfiner3D _confiner;
    [field: SerializeField] public BoxCollider ConfinerCollider { get; set; }
    [field: SerializeField] public GameObject CentrePosition { get; private set; }
    [field: SerializeField] public List<Transform> TrackingTargets { get; private set; } = new List<Transform>();
    [field: SerializeField] public GameObject Pacman { get; private set; }
    [field: SerializeField] public GameObject[] Ghosts { get; private set; } = new GameObject[4];

    private GameState _currentGameState;

    private float maxWidth;
    private float maxHeight;
    private float maxDistance;

    private float _defaultOrthoSize;
    private float _targetOrthoSize;
    private float _currentOrthoSize;
    private float _zoomTime;
    private bool _isTimedZooming;
    private bool _isDynamicTracking;

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
        if (_currentGameState.Equals(GameState.Playing) && _isDynamicTracking)
        {
            UpdateZoom();
        }
    }

    public void OnGameStateUpdated(GameState gameState)
    {
        _currentGameState = gameState;
    }

    private void UpdateZoom()
    {
        if (Pacman == null)
        {
            return;
        }

        Vector3 pacmanPosition = Pacman.transform.position;

        // Distance needed to fit every ghost on screen while following PacMan
        float followDistance = GetDistanceToFitTargets(pacmanPosition);
        float ratio = Mathf.InverseLerp(
            Constants.DYNAMIC_CAMERA_MIN_DISTANCE,
            Constants.DYNAMIC_CAMERA_MAX_DISTANCE,
            followDistance);

        // Close together: follow PacMan. Spread apart: recentre towards the static camera's view
        Vector3 position = Vector3.Lerp(pacmanPosition, GetStaticCentre(), ratio);

        // Re-check the fit around the blended position, so ghosts stay on screen mid-transition
        float distance = Mathf.Lerp(
            Constants.DYNAMIC_CAMERA_MIN_DISTANCE,
            Constants.DYNAMIC_CAMERA_MAX_DISTANCE,
            ratio);
        distance = Mathf.Min(
            Mathf.Max(distance, GetDistanceToFitTargets(position)),
            Constants.DYNAMIC_CAMERA_MAX_DISTANCE);

        if (_positionComposer != null)
        {
            _positionComposer.CameraDistance = Mathf.Lerp(
                _positionComposer.CameraDistance, distance, 5f * Time.deltaTime);
        }

        CentrePosition.transform.position = Vector3.Lerp(
            CentrePosition.transform.position,
            position,
            5f * Time.deltaTime);
    }

    private float GetDistanceToFitTargets(Vector3 centre)
    {
        float maxXOffset = 0f;
        float maxZOffset = 0f;

        foreach (Transform target in TrackingTargets)
        {
            if (target == null)
            {
                continue;
            }

            maxXOffset = Mathf.Max(maxXOffset, Mathf.Abs(target.position.x - centre.x));
            maxZOffset = Mathf.Max(maxZOffset, Mathf.Abs(target.position.z - centre.z));
        }

        float aspect = _cinemachineCamera.State.Lens.Aspect;
        if (aspect <= 0f)
        {
            aspect = 16f / 9f;
        }

        float tanHalfFov = Mathf.Tan(0.5f * Constants.DYNAMIC_CAMERA_FOV * Mathf.Deg2Rad);
        float pitch = Constants.CAMERA_ROTATION.x * Mathf.Deg2Rad;

        // Up/down the maze is foreshortened by the camera tilt, left/right uses the horizontal FOV
        float verticalDistance = (maxZOffset * Mathf.Sin(pitch) + Constants.DYNAMIC_CAMERA_PADDING) / tanHalfFov;
        float horizontalDistance = (maxXOffset + Constants.DYNAMIC_CAMERA_PADDING) / (tanHalfFov * aspect);

        return Mathf.Max(verticalDistance, horizontalDistance);
    }

    private Vector3 GetStaticCentre()
    {
        // The point the fixed camera looks at, so max zoom matches the static view
        Vector3 forward = Quaternion.Euler(Constants.CAMERA_ROTATION) * Vector3.forward;
        return Constants.CAMERA_POSITION + forward * Constants.DYNAMIC_CAMERA_MAX_DISTANCE;
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
        _isDynamicTracking = false;

        _cinemachineCamera.Target.TrackingTarget = null;
        _cinemachineCamera.Target.LookAtTarget = null;

        _cinemachineCamera.LookAt = null;
        _cinemachineCamera.Lens.FieldOfView = Constants.FIXED_CAMERA_FOV;

        transform.SetPositionAndRotation(
            Constants.CAMERA_POSITION,
            Quaternion.Euler(Constants.CAMERA_ROTATION));

        if (_positionComposer != null)
        {
            _positionComposer.CameraDistance = Constants.FIXED_CAMERA_DISTANCE;
            _positionComposer.enabled = false;
        }
        await Task.CompletedTask;
    }

    private async Task SetupDynamicCameraMode()
    {
        _isDynamicTracking = true;
        TrackingTargets.Clear();
        if (Pacman != null)
        {
            _cinemachineCamera.Target.TrackingTarget = CentrePosition.transform;
            _cinemachineCamera.Target.LookAtTarget = CentrePosition.transform;
            TrackingTargets.Add(Pacman.transform);
            CentrePosition.transform.position = Pacman.transform.position;
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

        _cinemachineCamera.Lens.FieldOfView = Constants.DYNAMIC_CAMERA_FOV;

        transform.SetPositionAndRotation(
            Constants.CAMERA_POSITION,
            Quaternion.Euler(Constants.CAMERA_ROTATION));

        if (_positionComposer != null)
        {
            _positionComposer.enabled = true;
            _positionComposer.CameraDistance = Constants.DYNAMIC_CAMERA_MAX_DISTANCE;
            // Lookahead would shift the view away from the fitted ghosts
            _positionComposer.Lookahead.Enabled = false;
        }
        if (_confiner != null)
        {
            _confiner.enabled = true;
            if (ConfinerCollider != null)
            {
                ConfinerCollider.center = Constants.DYNAMIC_CONFINER_COLLIDER_CENTRE;
                ConfinerCollider.size = Constants.DYNAMIC_CONFINER_COLLIDER_SIZE;
            }
        }
        await Task.CompletedTask;
    }

    private async Task SetupFollowCameraMode()
    {
        _isDynamicTracking = false;

        if (Pacman != null)
        {
            _cinemachineCamera.Target.TrackingTarget = Pacman.transform;
            _cinemachineCamera.Target.LookAtTarget = Pacman.transform;
        }
        _cinemachineCamera.Lens.FieldOfView = Constants.FOLLOW_CAMERA_FOV;

        transform.SetPositionAndRotation(
            Constants.FOLLOW_CAMERA_POSITION,
            Quaternion.Euler(Constants.FOLLOW_CAMERA_ROTATION));

        if (_positionComposer != null)
        {
            _positionComposer.enabled = true;
            _positionComposer.CameraDistance = Constants.FOLLOW_CAMERA_DISTANCE;
            _positionComposer.Lookahead.Enabled = true;
            _positionComposer.Lookahead.Time = Constants.FOLLOW_CAMERA_LOOK_AHEAD_TIME;
            _positionComposer.Lookahead.Smoothing = Constants.FOLLOW_CAMERA_LOOK_AHEAD_SMOOTHING;
        }
        if (_confiner != null)
        {
            _confiner.enabled = true;
            if (ConfinerCollider != null)
            {
                ConfinerCollider.center = Constants.FOLLOW_CONFINER_COLLIDER_CENTRE;
                ConfinerCollider.size = Constants.FOLLOW_CONFINER_COLLIDER_SIZE;
            }
        }

        await Task.CompletedTask;
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
