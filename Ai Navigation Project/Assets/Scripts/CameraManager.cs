using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbit;

    private InputAction moveAction;

    public float cameraRotationSpeed = 10f;

    void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        orbit = GetComponent<CinemachineOrbitalFollow>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 moveinput = moveAction.ReadValue<Vector2>();
        float moveX = moveinput.x;
        float moveY = moveinput.y;

        orbit.HorizontalAxis.Value -= moveX * cameraRotationSpeed * Time.deltaTime;

        if (orbit.HorizontalAxis.Value >= 360 || orbit.HorizontalAxis.Value <= -360)
        {
            orbit.HorizontalAxis.Value = 0;
        }
    }

}
