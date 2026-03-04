using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float speed;
    private Vector2 moveDir;
    private bool canMove = true;
    private Vector2 moveVector;

    InputAction moveAction, interactAction;

    // Start is called before the first frame update
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
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        interactAction = InputSystem.actions.FindAction("Interact");

    }
    void Update()
    {
        moveVector = moveAction.ReadValue<Vector2>();
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (canMove)
            rb2d.linearVelocity = moveVector * speed;

        if (interactAction.IsPressed())
        {
            DialogManager.instance.StartDialog();

        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Diamond")
        {
            Destroy(collision.gameObject);
            DialogManager.instance.GotDiamond();
        }
    }

    private void OnDialogStart()
    {
        rb2d.linearVelocity = Vector2.zero;
        canMove = false;
    }
    private void OnDialogOver()
    {
        canMove = true;
    }
}
