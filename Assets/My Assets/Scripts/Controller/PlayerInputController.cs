using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputController : MovementController
{


    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;


    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void LateUpdate() {
        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();

        Movement();
        Gravity();
    }



    public void Restart()
    {
        SceneManager.LoadSceneAsync("TestScene");
    }

}
