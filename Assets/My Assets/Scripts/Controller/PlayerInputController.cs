using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputController : CharacterBattleController
{

    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    float lastRolling = 0;
    readonly float rollingInterval = 0.5f;
    readonly float rollingCooldown = 1f;
    int rollingHash;

    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Rolling.performed += ctx => Rolling();
    }
    protected override void Start()
    {
        base.Start();
        lastRolling -= rollingCooldown;
    }

    private void LateUpdate()
    {
        if (currentCharacterState == CharacterState.dying)
        {
            return;
        }
        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        inputVector = inputVector.normalized;

        if (currentCharacterState != CharacterState.rolling)
        {
            currentCharacterState = inputVector == Vector2.zero ? CharacterState.idle : CharacterState.running;
        }
        Movement();
        Gravity();
    }

    private void Rolling()
    {
        Debug.Log(Time.time);
        if (currentCharacterState == CharacterState.rolling)
        {
            Debug.Log("Sedang masa rolling");
            return;
        }

        if (Time.time < lastRolling + rollingCooldown)
        {
            Debug.Log("Sedang cooldown");
            return;
        }

        modifierSpeed = 3f;
        lastRolling = Time.time;
        currentCharacterState = CharacterState.rolling;

        StartCoroutine(StartRolling());

        animator.SetTrigger("Rolling");
    }

    IEnumerator StartRolling()
    {
        yield return new WaitUntil(() => Time.time > lastRolling + rollingInterval == true);
        currentCharacterState = CharacterState.idle;
        modifierSpeed = 1f;
    }

}
