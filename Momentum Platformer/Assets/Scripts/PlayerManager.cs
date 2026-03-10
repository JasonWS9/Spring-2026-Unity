using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    private InputAction resetAction;
    private InputAction enterAction;
    private InputAction interactAction;

    private Rigidbody2D playerRb;

    private bool isLevelCompleted = false;

    public Vector2 spawnPoint;

    private bool canInteract;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        resetAction = InputSystem.actions.FindAction("Reset");
        enterAction = InputSystem.actions.FindAction("Enter");
        interactAction = InputSystem.actions.FindAction("Interact");
        spawnPoint = transform.position;
        isLevelCompleted = false;
    }

    private void OnEnable()
    {
        DialogManager.DialogStart += OnDialogStart;
        DialogManager.DialogOver += OnDialogOver;
    }
    private void OnDisable()
    {
        DialogManager.DialogStart -= OnDialogStart;
        DialogManager.DialogOver -= OnDialogOver;
    }

    void Update()
    {
        if (resetAction.WasPressedThisFrame())
        {
            SceneManagment.instance.LoadScene("TitleScene");
        }

        if (enterAction.WasPressedThisFrame())
        {
            if (isLevelCompleted)
            {
                SceneManagment.instance.LoadNextLevel();
            }
        }

        if (interactAction.WasPressedThisFrame())
        {
            DialogManager.instance.StartDialog();
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
            //PlayerDeath();
        }

        if (collision.CompareTag("SpawnPoint"))
        {
            spawnPoint = collision.transform.position;
        }
    
        if (collision.gameObject.tag == "Diamond")
        {
            Destroy(collision.gameObject);
            DialogManager.instance.GotDiamond();
        }
        
    }
#endregion

    void PlayerDeath()
    {
        transform.position = spawnPoint;
        playerRb.linearVelocity = new Vector2(0,0);
    }

#region Dialogue
    private void OnDialogStart()
    {

    }
    private void OnDialogOver()
    {
        
    }
#endregion

}
