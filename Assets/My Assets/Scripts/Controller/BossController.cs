using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour, ITargetable, IDamageable
{
    public Transform bossBody;
    public CharacterData characterData;
    public Animator bossAnimator;
    public BossState currentBossState;
    public new Rigidbody rigidbody;
    public BoxCollider boxCollider;
    public LayerMask targetMask;
    public Transform targetObject;
    public NavMeshAgent agent;
    public GameObject minionPrefab;
    public GameObject finalBossFase;

    Transform player;
    float lastSkill;
    void AddLastSkill(float value) => lastSkill += value;
    float cooldownSkill;
    float cooldownSkillModifier;
    int dyingHash;
    int jumpHash;
    int speedHash;
    float moveSpeed;

    bool targetable = true;

    bool rageState = false;

    private void Awake()
    {
        characterData.ResetCurrentPoint();

    }

    private void Start()
    {
        player = GameManager.Instance.PlayerTransform;

        dyingHash = Animator.StringToHash("Dying");
        jumpHash = Animator.StringToHash("Jump");
        speedHash = Animator.StringToHash("Speed");

        MinionSpawner();
        StartCoroutine(StartBoss());
        StartCoroutine(WaitingRage());
        StartCoroutine(WaitingFinalRage());
    }

    IEnumerator WaitingFinalRage()
    {
        yield return new WaitUntil(() => characterData.currentPoint.health < 500);
        AudioManager.Instance.PitchModifier("Music", 0.48f);
        finalBossFase.SetActive(true);
    }

    IEnumerator WaitingRage()
    {
        yield return new WaitUntil(() => characterData.currentPoint.health < 1000);
        yield return new WaitUntil(() => agent.isActiveAndEnabled);
        AudioManager.Instance.PitchModifier("Music", 0.7f);
        cooldownSkillModifier = 2f;
        bossAnimator.speed = 1.5f;
        agent.acceleration = 10;
        AudioManager.Instance.PlayOnShot("Boss Rage");
        GameObject fire = ParticleManager.Instance.GetParticle(ParticleManager.ParticleType.fire);
        Instantiate(fire, transform);
        rageState = true;
    }

    IEnumerator StartBoss()
    {
        bool forceStop = false;
        cooldownSkillModifier = 1f;
        while (characterData.currentPoint.health > 0)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            agent.enabled = true;

            AudioManager.Instance.Play("Boss Walk");

            yield return new WaitUntil(() =>
            {
                if (characterData.currentPoint.health <= 0)
                {
                    agent.Warp(transform.position);
                    agent.enabled = false;
                    forceStop = true;
                    return true;
                }
                if (Time.time < lastSkill + cooldownSkill == false)
                {
                    agent.Warp(transform.position);
                    agent.enabled = false;
                    return true;
                };

                agent.SetDestination(player.position);
                moveSpeed = agent.acceleration != 0 ? Mathf.Clamp(moveSpeed + 5 * Time.deltaTime, 0, 1) : Mathf.Clamp(moveSpeed - 5 * Time.deltaTime, 0, 1);
                bossAnimator.SetFloat(speedHash, moveSpeed);

                return false;
            });

            yield return new WaitUntil(() =>
            {
                moveSpeed = Mathf.Clamp(moveSpeed - 5 * Time.deltaTime, 0, 1);
                if (moveSpeed <= 0)
                {
                    return true;
                }
                return false;
            });

            bossAnimator.SetFloat(speedHash, 0);

            AudioManager.Instance.Stop("Boss Walk");

            if (forceStop)
            {
                break;
            }

            AudioManager.Instance.PlayOnShot("Boss Skill");

            lastSkill = Time.time;
            cooldownSkill = UnityEngine.Random.Range(6, 10) / cooldownSkillModifier;
            int random = new System.Random().Next(3);
            Debug.Log(random);

            switch (random)
            {
                case 0:
                    if (rageState)
                    {
                        StartCoroutine(ThrowRock());
                        yield return new WaitUntil(() => Time.time < lastSkill + 1.0 == false);
                        lastSkill = Time.time;
                        StartCoroutine(ThrowRock());
                        yield return new WaitUntil(() => Time.time < lastSkill + 1.0 == false);
                        lastSkill = Time.time;
                    }
                    yield return StartCoroutine(ThrowRock());
                    break;
                case 1:
                    yield return StartCoroutine(JumpSmash());
                    break;
                case 2:
                    yield return StartCoroutine(SpawnMinion());
                    break;
                default:
                    if (rageState)
                    {
                        yield return StartCoroutine(ThrowRock());
                        yield return StartCoroutine(ThrowRock());
                    }
                    yield return StartCoroutine(ThrowRock());
                    break;
            }
        }

        bossAnimator.SetTrigger(dyingHash);
        GameManager.Instance.Win();
    }

    IEnumerator ThrowRock()
    {
        bossAnimator.SetTrigger("Skill");
        yield return new WaitUntil(() => Time.time < lastSkill + 1 == false);
        AddLastSkill(1f);

        CameraShakeManager.Instance.StartShake(0.4f, 0.4f);
        GameObject rockPrefab = PropertyManager.Instance.GetPorperty(PropertyManager.PropertyType.rock);
        ParticleManager.Instance.PlayParticleOnTime(ParticleManager.ParticleType.rocket, transform.position);

        Vector3 rockLocation = transform.position;
        GameObject rock = Instantiate(rockPrefab, rockLocation, Quaternion.identity);
        AudioManager.Instance.PlayOnShot("Rock Yeet");
        StartCoroutine(YeetRock(rock));
    }

    IEnumerator YeetRock(GameObject rock)
    {
        Rigidbody rigidbodyRock = rock.GetComponent<Rigidbody>();
        rigidbodyRock.velocity = Vector3.up * 25;
        yield return new WaitUntil(() => rigidbodyRock.velocity.y < 0);

        Vector3 targetPosition = GetPlayerPosition();
        rock.transform.position = new Vector3(targetPosition.x, rock.transform.position.y, targetPosition.z);

        rigidbodyRock.velocity = Vector3.down * (25 * cooldownSkillModifier);

        Destroy(rock.gameObject, 5f);
    }

    IEnumerator SpawnMinion()
    {
        bossAnimator.SetTrigger("Ultimate");

        AudioManager.Instance.PlayOnShot("Boss Summon");
        CameraShakeManager.Instance.StartShake(1f, 0.4f);

        yield return new WaitUntil(() => Time.time < lastSkill + 1 == false);
        AddLastSkill(1);

        MinionSpawner();

        yield return new WaitUntil(() => Time.time < lastSkill + 2 == false);
        AddLastSkill(2);
    }

    void MinionSpawner()
    {
        Vector3 minionLocation = transform.position;
        Instantiate(minionPrefab, minionLocation + transform.forward * 2, transform.rotation);
        Instantiate(minionPrefab, minionLocation + transform.right * 2, transform.rotation);
        Instantiate(minionPrefab, minionLocation + transform.forward * -2, transform.rotation);
        Instantiate(minionPrefab, minionLocation + transform.right * -2, transform.rotation);
    }

    IEnumerator JumpSmash()
    {

        bossAnimator.SetBool(jumpHash, true);
        yield return new WaitUntil(() => Time.time < lastSkill + 1 == false);
        AddLastSkill(1f);

        ParticleManager.Instance.PlayParticleOnTime(ParticleManager.ParticleType.plasma, transform.position);
        yield return new WaitUntil(() => Time.time < lastSkill + 0.5f == false);
        AddLastSkill(0.5f);
        transform.position = new Vector3(transform.position.x, transform.position.y + 20, transform.position.z);

        yield return new WaitUntil(() => rigidbody.velocity.y < 0);

        Vector3 targetPosition = GetPlayerPosition();

        transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        rigidbody.velocity = Vector3.down * 20;

        Ray ray = new Ray(boxCollider.bounds.center, Vector3.down);
        yield return new WaitUntil(() =>
        {
            RaycastHit hit;
            float raycastDistance = 1f;
            Ray ray = new Ray(boxCollider.bounds.center, Vector3.down);
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                return hit.distance <= raycastDistance;
            }
            return false;
        });
        lastSkill = Time.time;
        bossAnimator.SetBool(jumpHash, false);
        CameraShakeManager.Instance.StartShake(1f, 0.4f);
        ParticleManager.Instance.PlayParticleOnTime(ParticleManager.ParticleType.dust, transform.position);
        AudioManager.Instance.PlayOnShot("Rock Smash");

        yield return new WaitUntil(() => Time.time < lastSkill + 1.5 == false);
        AddLastSkill(1.5f);
    }

    Vector3 GetPlayerPosition()
    {
        return player.position;
    }

    public Transform TargetObject()
    {
        return targetObject;
    }

    public Transform TargetTransform()
    {
        throw new NotImplementedException();
    }

    public bool IsTargetable()
    {
        return targetable;
    }

    public void TakeDamage(int value)
    {
        characterData.AddHealth(-value);
    }

    public enum BossState
    {
        idle,
        skill,
        dying
    }
}
