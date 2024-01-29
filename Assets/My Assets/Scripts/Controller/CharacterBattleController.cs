using System;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterBattleController : MonoBehaviour, ITargetable, IDamageable
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform targetObject;
    [SerializeField] private Transform baseTargetLocation;

    [NonSerialized] public CharacterController characterController;
    [NonSerialized] public const float speed = 7f;
    [NonSerialized] public const float gravity = -9.81f;
    [NonSerialized] public const float jumpHeight = 3f;
    [NonSerialized] public const float wallRunVelocity = -1.1f;
    [NonSerialized] public const float wallRunModifierSpeed = 1.1f;
    [NonSerialized] public const float turnSmoothTime = 0.1f;
    [NonSerialized] public Vector2 inputVector;

    public CharacterType characterType;



    public Transform player;
    public Animator animator;

    public HealthAndMana currentPoint;
    public HealthAndMana maximumPoint;
    public void AddHealth(int add) => SetHealth(currentPoint.health + add);
    public void SetHealth(int value) => currentPoint.health = Mathf.Clamp(value, 0, maximumPoint.health);
    public CharacterData characterDataFromScriptableObject;
    public CharacterState currentCharacterState;

    float moveSpeed;
    Vector3 velocity;
    bool isGrounded;
    float turnSmoothVelocity;
    public float modifierSpeed = 1f;

    bool isTargetable;

    public int velocityZHash;

    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    protected virtual void Start()
    {
        velocityZHash = Animator.StringToHash("Velocity Z");
        currentPoint = characterDataFromScriptableObject.currentPoint;
        maximumPoint = characterDataFromScriptableObject.maximumPoint;
    }

    void Update()
    {
        if (currentCharacterState == CharacterState.dying)
        {
            return;
        }
        if (!CheckHealth())
        {
            if (characterType == CharacterType.player)
            {
                currentCharacterState = CharacterState.dying;
                GameManager.Instance.Lose();
            }
        }
    }

    bool CheckHealth()
    {
        if (currentPoint.health <= 0)
        {
            Debug.Log("TEWAS");
            return false;
        }
        return true;
    }

    public Transform TargetObject()
    {
        return targetObject;
    }
    public Transform TargetTransform()
    {
        return transform;
    }

    protected virtual void Movement()
    {
        // player.localPosition = Vector3.zero;

        moveSpeed = inputVector.magnitude != 0 ? Mathf.Clamp(moveSpeed + 5 * Time.deltaTime, 0, 1) : Mathf.Clamp(moveSpeed - 5 * Time.deltaTime, 0, 1);
        animator.SetFloat(velocityZHash, moveSpeed);
        SmoothRotate(inputVector);
        if (inputVector == Vector2.zero)
        {
            return;
        }

        characterController.Move(new Vector3(inputVector.x, 0, inputVector.y) * (speed * modifierSpeed) * Time.deltaTime);
    }


    public void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            return;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void SmoothRotate(Vector2 inputVector)
    {
        Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y).normalized;
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(player.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            player.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void Jump()
    {
        if (!isGrounded)
        {
            return;
        }
        JumpPlayer(jumpHeight);
    }

    private void JumpPlayer(float jumpHeight)
    {
        velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        characterController.Move(velocity * Time.deltaTime);
    }

    public void TrampolineJump()
    {
        velocity.y = -2f;
        JumpPlayer(10f);
    }

    public void TakeDamage(int value)
    {
        AddHealth(-value);
    }

    public bool IsTargetable()
    {
        return isTargetable;
    }

    public enum CharacterState
    {
        idle,
        running,
        rolling,
        shooting,
        dying,
    }

    public enum CharacterType
    {
        player,
        enemy
    }
}

internal interface ITargetable
{
    public bool IsTargetable();
    public Transform TargetObject();
    public Transform TargetTransform();
}

internal interface IDamageable
{
    public void TakeDamage(int value);
}