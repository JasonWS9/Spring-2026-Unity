using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    private InputAction resetAction;
    private InputAction enterAction;

    private Rigidbody2D playerRb;

    private bool isLevelCompleted = false;

    public Vector2 spawnPoint;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        resetAction = InputSystem.actions.FindAction("Reset");
        enterAction = InputSystem.actions.FindAction("Enter");
        spawnPoint = transform.position;
        isLevelCompleted = false;
    }

    void Update()
    {
        if (resetAction.WasPressedThisFrame())
        {
            SceneManagment.instance.ReloadCurrentScene();
        }

        if (enterAction.WasPressedThisFrame())
        {
            if (isLevelCompleted)
            {
                SceneManagment.instance.LoadNextLevel();
            }
        }

    }

#region Collisions
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            TimerManager.instance.CompleteLevel();
            isLevelCompleted = true;
        }

        if (collision.CompareTag("Hazard"))
        {
            PlayerDeath();
        }

        if (collision.CompareTag("SpawnPoint"))
        {
            spawnPoint = collision.transform.position;
        }
        
    }
#endregion

    void PlayerDeath()
    {
        transform.position = spawnPoint;
        playerRb.linearVelocity = new Vector2(0,0);
    }

}
