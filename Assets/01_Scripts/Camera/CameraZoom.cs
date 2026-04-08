using CoreSystem;
using System;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

public class CameraZoom : NonPersistentSingleton<CameraZoom>
{
    private Camera mainCamera;
    private CinemachineCamera cinemachineCamera;
    private CinemachinePositionComposer positionComposer;
    private CinemachineConfiner2D confiner2D;
    [field: SerializeField] public PolygonCollider2D ConfinerCollider {  get; set; }
    [field: SerializeField] public Transform TrackingTarget { get; private set; }
    private bool trackGroupCentre = false;
    [field: SerializeField] public GameObject GroupCentre { get; private set; }
    [field: SerializeField] public GameObject PlayerOne { get; private set; }
    [field: SerializeField] public GameObject PlayerTwo { get; private set; }

    private float maxWidth;
    private float maxHeight;
    private float maxDistance;

    private float defaultOrthoSize;
    private float targetOrthoSize;
    private float currentOrthoSize;
    private float zoomTime;
    private bool isTimedZooming;

    protected override void Awake()
    {
        base.Awake();
        cinemachineCamera = GetComponent<CinemachineCamera>();
        defaultOrthoSize = cinemachineCamera.Lens.OrthographicSize;
        positionComposer = gameObject.GetOrAdd<CinemachinePositionComposer>();
        confiner2D = gameObject.GetOrAdd<CinemachineConfiner2D>();
    }

     private void Update()
    {
        //if (isTimedZooming)
        //{
        //    TimedZoom();
        //}
        //else 
        if (trackGroupCentre)
        {
            TrackTargetGroup();
        }
    }

    private void TrackTargetGroup()
    {
        if (PlayerOne == null) return;

        if (PlayerTwo == null)
        {
            GroupCentre.transform.position = PlayerOne.transform.position;
            return;
        }
        Vector3 midpoint = PlayerOne.transform.position + ((PlayerTwo.transform.position - PlayerOne.transform.position) / 2);
        GroupCentre.transform.position = midpoint;
        float xDistanceBetweenPlayers = Mathf.Abs(PlayerOne.transform.position.x - PlayerTwo.transform.position.x);
        float yDistanceBetweenPlayers = Mathf.Abs(PlayerOne.transform.position.y - PlayerTwo.transform.position.y);
        float heightDistFactor = Constants.ZOOM_FACTOR_CONSTANT + Mathf.InverseLerp(
            0,
            maxWidth,
            xDistanceBetweenPlayers
        );

        float widthDistFactor = Constants.ZOOM_FACTOR_CONSTANT + Mathf.InverseLerp(
            0,
            maxHeight,
            yDistanceBetweenPlayers
        );

        float zoomFactor = Mathf.Max(heightDistFactor, widthDistFactor);
        targetOrthoSize = Mathf.Clamp((zoomFactor * Constants.MAX_ORTHOGRAPHIC_CAMERA_SIZE), 
            Constants.MIN_ORTHOGRAPHIC_CAMERA_SIZE, Constants.MAX_ORTHOGRAPHIC_CAMERA_SIZE);
        if(targetOrthoSize != currentOrthoSize)
        {
            cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetOrthoSize, 5f * Time.deltaTime);
            if (Mathf.Approximately(cinemachineCamera.Lens.OrthographicSize, targetOrthoSize))
            {
                cinemachineCamera.Lens.OrthographicSize = targetOrthoSize;
                currentOrthoSize = targetOrthoSize;
            }
            confiner2D.InvalidateLensCache();
        }
    }

    private void TimedZoom()
    {
        cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetOrthoSize, 5f * Time.deltaTime);
        if (zoomTime > 0f)
        {
            zoomTime -= Time.deltaTime;
            if (zoomTime <= 0f)
            {
                ResetZoom();
            }
        }

        if (Mathf.Abs(cinemachineCamera.Lens.OrthographicSize - targetOrthoSize) < 0.1f)
        {
            isTimedZooming = false;
        }
    }

    public void AddPlayerToCameraTarget(int playerIndex, GameObject playerObject)
    {
        if (playerObject == null) return;

        if (playerIndex == 1)
        {
            PlayerOne = playerObject;
        }
        else
        {
            PlayerTwo = playerObject;
        }
    }

    public async Task SetupCameraMode(LevelContext context, string cameraMode)
    {
        mainCamera = Camera.main;
        if (cinemachineCamera == null) return;

        if (Enum.TryParse(cameraMode, out CameraMode parsedCameraMode))
        {
            switch (parsedCameraMode)
            {
                case CameraMode.Fixed:
                default:
                    await SetupFixedCameraMode();
                    break;
                case CameraMode.Dynamic:
                    await SetupDynamicCameraMode(context.PlayerCount);
                    break;
            }
        }
        Debug.Log($"Camera Mode set to: {cameraMode}");
    }

    private async Task SetupFixedCameraMode()
    {
        cinemachineCamera.Lens.OrthographicSize = Constants.MAX_ORTHOGRAPHIC_CAMERA_SIZE;
        cinemachineCamera.LookAt = null;
        trackGroupCentre = false;
        if (positionComposer != null)
        {
            positionComposer.enabled = false;
        }
        if (confiner2D != null)
        {
            confiner2D.enabled = false;
        }
        await Task.CompletedTask;
    }

    private async Task SetupDynamicCameraMode(int playerCount)
    {
        if (positionComposer != null)
        {
            positionComposer.enabled = true;
            positionComposer.Lookahead.Enabled = true;
            positionComposer.Lookahead.Time = Constants.DYNAMIC_CAMERA_LOOK_AHEAD_TIME;
            positionComposer.Lookahead.Smoothing = Constants.DYNAMIC_CAMERA_LOOK_AHEAD_SMOOTHING;
        }
        if (confiner2D != null)
        {
            confiner2D.enabled = true;
            if (ConfinerCollider != null)
            {
                SetupConfinerCollider();
            }
        }
        if (playerCount == 1)
        {
            if (PlayerOne != null)
            {
                TrackingTarget = PlayerOne.transform;
            }
        }
        else
        {
            TrackingTarget = GroupCentre.transform;
            trackGroupCentre = true;
        }
        TrackingTarget.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        cinemachineCamera.Target.TrackingTarget = TrackingTarget;
        cinemachineCamera.Target.LookAtTarget = TrackingTarget;
        confiner2D.InvalidateBoundingShapeCache();
        await Task.CompletedTask;
    }

    public async Task ResetCameraZoom()
    {
        if (cinemachineCamera == null)
        {
            GetComponent<CinemachineCamera>();
        }
        defaultOrthoSize = cinemachineCamera.Lens.OrthographicSize;
        await Task.CompletedTask;
    }

    public void ZoomWithTargetAndDuration(float distance, Transform target, float time)
    {
        targetOrthoSize = defaultOrthoSize - distance; // Zoom in by reducing FOV
        zoomTime = time;
        cinemachineCamera.LookAt = target;
        isTimedZooming = true;
    }

    private void SetupConfinerCollider()
    {
        ConfinerCollider.isTrigger = true;
        cinemachineCamera.Lens.OrthographicSize = Constants.MIN_ORTHOGRAPHIC_CAMERA_SIZE;

        maxHeight = Constants.MAX_ORTHOGRAPHIC_CAMERA_SIZE * 2f;
        maxWidth = maxHeight * mainCamera.aspect;
        maxDistance = Mathf.Sqrt((maxHeight * maxHeight) + (maxWidth * maxWidth));

        float halfHeight = (maxHeight / 2f) + 0.1f;
        float halfWidth = (maxWidth / 2f) + 0.1f;

        Vector2[] points = new Vector2[]
        {
            new(-halfWidth, halfHeight),
            new(-halfWidth, -halfHeight),
            new(halfWidth, -halfHeight),
            new(halfWidth, halfHeight)
        };

        ConfinerCollider.SetPath(0, points);
    }

    public void ResetZoom(float resetTime = 0.5f)
    {
        targetOrthoSize = defaultOrthoSize;

        zoomTime = (resetTime > 0f) ? resetTime : zoomTime; // Use last zoom time if not specified
        isTimedZooming = true;
    }
}
