using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    private Rigidbody rb;
    private InputAction moveAction;

    private Vector2 moveVector;

    public float speed; 

    private Vector3 startPos;

    public float speedBoostTime;
    public float speedBoostMult;

    private bool isBoosted;

    public float jumpForce;

    public GameObject W;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("move");
        startPos = transform.position;
        Cursor.lockState = CursorLockMode.Confined;
        W.SetActive(false);
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
            W.SetActive(true);
        }

        if (other.CompareTag("Hazard")) 
        {
            Debug.Log("ow");
            transform.position = startPos;
        }
        if (other.CompareTag("Boost"))
        {
            Debug.Log("boost");
            //StartCoroutine(SpeedBoost());
            rb.AddForce(0, jumpForce, 0);
        }
    }

    IEnumerator SpeedBoost()
    {
        isBoosted = true;
        float originalSpeed = speed;
        speed *= speedBoostMult;
        Debug.Log(speed);
        yield return new WaitForSeconds(speedBoostTime);
        speed = originalSpeed;
        isBoosted = false;
        Debug.Log(speed);
    }
}
