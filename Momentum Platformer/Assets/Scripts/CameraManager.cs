using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("References")]
    private CinemachineCamera cam;
    private CinemachinePositionComposer composer;
    private CinemachineConfiner2D confiner;
    public Rigidbody2D playerRB;

    [Header("ZoomValues")]
    private float cameraDistance;
    public float maxCameraZoom = 20f; //The farthest zoom
    private float minCameraZoom; //The closest zoom (set at start)
    public float zoomSmoothTime; //How long it takes for the zoom to fully adjust (Higher = smoother)
    private float zoomVelocity; //Internal velocity value needed for smoothDamp to work

    [Header("SpeedValues")]
    public float minSpeed; //Minimum player speed for zoomout to start
    public float maxSpeed; //Player speed where the camera reaches max zoom
    private float playerSpeed;
    //public float horizontalSpeedZoomMult;

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        composer = GetComponent<CinemachinePositionComposer>();
        confiner = GetComponent<CinemachineConfiner2D>();

        minCameraZoom = cam.Lens.OrthographicSize;
    }

    void LateUpdate()
    {
        float playerSpeed = playerRB.linearVelocity.x;

        // Converts the speed into a zoom value: 0 = minSpeed & 1 = maxSpeed
        //EX: If minSpeed = 10 & maxSpeed = 20, if playerSpeed is 15 it returns 0.5
        float targetSpeed = Mathf.InverseLerp(minSpeed, maxSpeed, playerSpeed);

        //Lerp converts the 0-1 value into a zoom amount between minZoom and maxZoom.'
        //EX: 0.5 means halfway between the min & max zoom
        float targetZoom = Mathf.Lerp(minCameraZoom, maxCameraZoom, targetSpeed);

        float currentZoom = cam.Lens.OrthographicSize;

        //Smoothly changes the currentzoom to the targetzoom based on the smoothTime
        float smoothZoom = Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVelocity, zoomSmoothTime);

        cam.Lens.OrthographicSize = smoothZoom;
    }

    private void ZoomOut()
    {
        
    }
}
