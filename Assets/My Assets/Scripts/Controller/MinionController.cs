using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MinionController : MonoBehaviour, ITargetable, IDamageable
{
    NavMeshAgent agent;
    Transform target;
    Animator animator;
    LayerMask playerMask;

    float lastAttack;
    void AddLastAttack(float value) => lastAttack += value;

    public Transform targetObject;
    public CharacterData characterDataFromScriptableObject;


    bool isTargetable = true;

    public HealthAndMana currentPoint;
    public HealthAndMana maximumPoint;

    public void AddHealth(int add) => SetHealth(currentPoint.health + add);
    public void SetHealth(int value) => currentPoint.health = Mathf.Clamp(value, 0, maximumPoint.health);

    int speedHash;
    readonly int detectRadius = 1;
    readonly int rangeAttackRadius = 2;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = GameManager.Instance.PlayerTransform;
        playerMask = GameManager.Instance.playerMask;

        speedHash = Animator.StringToHash("Speed");

        currentPoint = characterDataFromScriptableObject.currentPoint;
        maximumPoint = characterDataFromScriptableObject.maximumPoint;

        StartCoroutine(StartMinion());
    }

    IEnumerator StartMinion()
    {
        bool forceStop = false;
        while (currentPoint.health > 0)
        {
            agent.enabled = true;

            yield return new WaitUntil(() =>
            {
                if (currentPoint.health <= 0) {
                    agent.Warp(transform.position);
                    agent.enabled = false;
                    forceStop = true;
                    return true;
                }
                if (CheckingTarget() == true)
                {
                    agent.Warp(transform.position);
                    agent.enabled = false;
                    return true;
                };

                agent.SetDestination(target.position);
                float speed = agent.acceleration;
                animator.SetFloat(speedHash, speed);

                return false;
            });
            if (forceStop) {
                break;
            }

            lastAttack = Time.time;

            animator.SetTrigger("Punch");
            AudioManager.Instance.PlayOnShot("Minion Attack");

            yield return new WaitUntil(() => Time.time < lastAttack + 1 == false);
            AddLastAttack(1);

            AudioManager.Instance.PlayOnShot("Minion Punch");
            IDamageable damageable = CheckingDamage();
            if (damageable != null)
            {
                damageable.TakeDamage(10);
            }

            yield return new WaitUntil(() => Time.time < lastAttack + 1 == false);
            AddLastAttack(1);
        }

        AudioManager.Instance.PlayOnShot("Minion Dying");

        isTargetable = false;
        animator.SetTrigger("Dying");
        Destroy(gameObject, 3f);
    }

    bool CheckingTarget()
    {
        ITargetable targetable = null;


        Collider[] colliders = Physics.OverlapSphere(transform.position, detectRadius, playerMask);

        foreach (var collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out targetable))
            {
                return true;
            }
        }

        return false;
    }

    IDamageable CheckingDamage()
    {
        IDamageable damageable;
        Collider[] colliders = Physics.OverlapSphere(transform.position, rangeAttackRadius, playerMask);

        foreach (var collider in colliders)
        {
            Debug.Log(collider);
            if (collider.gameObject.TryGetComponent(out damageable))
            {
                Debug.Log("ADA");
                return damageable;
            }
        }

        return null;
    }

    public Transform TargetObject()
    {
        return targetObject;
    }

    public Transform TargetTransform()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int value)
    {
        AddHealth(-value);
    }

    public bool IsTargetable()
    {
        return isTargetable;
    }
}