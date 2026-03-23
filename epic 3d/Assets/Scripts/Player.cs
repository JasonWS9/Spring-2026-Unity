using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    private Rigidbody rb;
    private InputAction moveAction;

    private Vector2 moveVector;

    public float speed; 

    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("move");
        startPos = transform.position;
    }

    void Update()
    {
        moveVector = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        float inputX = moveVector.x;
        float inputZ = moveVector.y;

        Vector3 moveDir = cameraForward * inputZ + cameraRight * inputX;
        rb.AddForce(moveDir * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal")) 
        {
            Debug.Log("goal");
        }

        if (other.CompareTag("Hazard")) 
        {
            Debug.Log("ow");
            transform.position = startPos;
        }
    }
}
