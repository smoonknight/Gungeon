using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooterControler : ShooterController
{
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    public GameObject bulletPrefab;
    public Transform bulletHole;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Shoot.performed += ctx => Shoot();
    }

    void Shoot() {
        GameObject peluru = Instantiate(bulletPrefab, bulletHole.position, bulletHole.rotation);

        Rigidbody rbPeluru = peluru.GetComponent<Rigidbody>();
        Vector3 direction = (target.position - bulletHole.position).normalized;

        rbPeluru.velocity = direction * 25 ;
        Destroy(peluru.gameObject, 3);
    }


}