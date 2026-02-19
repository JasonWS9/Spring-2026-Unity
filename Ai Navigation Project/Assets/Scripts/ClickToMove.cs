using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]

public class ClickToMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Camera cam;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private UnityEngine.AI.NavMeshAgent m_Agent;
    
    void Start()
    {
        cam = Camera.main;
        targetPosition = transform.position; // Start at current position
        m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetTargetPosition();
        }

        if (isMoving)
        {
            MoveObject();
        }
    }

    void SetTargetPosition()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        
        // Create a Ray from the camera through the mouse position
        Ray ray = cam.ScreenPointToRay(mouseScreenPos);
        
        // Create a mathematical plane at y = 0 facing "up"
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // Check where the ray hits that plane
        if (groundPlane.Raycast(ray, out float distance))
        {
            targetPosition = ray.GetPoint(distance);
            isMoving = true;
        }
    }

    void MoveObject()
    {
        // Move strictly along X and Z, keeping the current Y height
        Vector3 destination = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        m_Agent.destination = destination;
        
        // transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            isMoving = false;
        }
    }
}