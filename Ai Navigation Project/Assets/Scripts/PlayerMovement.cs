using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public CharacterController controller;
    public Transform cameraTransform;

    public float speed = 6f;
    public float gravity = -9.81f;

    private int collectableCount;
    public int totalCollectables;

    float verticalVelocity;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * v;

        if (move.magnitude > 1)
            move.Normalize();

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
            collectableCount++;
            Debug.Log(collectableCount);
        }

        if (other.CompareTag("Goal") || other.name == "Goal")
        {
            if (collectableCount >= totalCollectables)
            {
                GameManager.instance.ReloadScene();
                Debug.Log("You Win");

            } else
            {
                Debug.Log("Havent Gotten All Collectables");
            }
        }

        if (other.CompareTag("Enemy") || other.name == "Enemy")
        {
            GameManager.instance.ReloadScene();
        }

    }

    private void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.name == "Enemy")
        {
            GameManager.instance.ReloadScene();
        }

        if (hit.gameObject.CompareTag("Goal") || hit.gameObject.name == "Goal")
        {
            if (collectableCount >= totalCollectables)
            {
                GameManager.instance.ReloadScene();
                Debug.Log("You Win");

            } else
            {
                Debug.Log("Havent Gotten All Collectables");
            }
        }
    }
}
