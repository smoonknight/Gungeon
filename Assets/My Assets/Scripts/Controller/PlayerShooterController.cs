using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerShooterControler : ShooterController
{
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    public GameObject bulletPrefab;
    public Transform bulletHole;

    PlayerInputController playerInputController;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Shoot.performed += ctx => TakeShoot();
    }

    private void Start()
    {
        playerInputController = GetComponent<PlayerInputController>();
    }

    void TakeShoot()
    {
        if (playerInputController.currentCharacterState == PlayerInputController.CharacterState.idle || playerInputController.currentCharacterState == PlayerInputController.CharacterState.running)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletHole.position, bulletHole.rotation);
            Shoot(bullet, bulletHole);
        }
    }



}