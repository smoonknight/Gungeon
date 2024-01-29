using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AIPlayerController : CharacterBattleController
{

    public NavMeshAgent agent;
    public LayerMask playerMask;
    public const float sphereRadius = 15f;


    protected override void Awake()
    {
        base.Awake();
    }

    private void LateUpdate()
    {
        Movement();
        //Gravity();
    }

    protected override void Movement()
    {
        float moveSpeed = Mathf.Clamp01(agent.velocity.magnitude);
        player.localPosition = Vector3.zero;

        animator.SetFloat(velocityZHash, moveSpeed);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius, playerMask);
        if (colliders.Length == 0)
        {
            agent.SetDestination(transform.forward * 3);
            return;
        }
        agent.SetDestination(colliders[0].transform.position);
    }

    private void UpdateLocation()
    {
        Vector3 random = Vector3.one;
        random.x = Random.Range(3, 10);
        random.y = Random.Range(3, 10);
        Debug.Log(random);

        agent.SetDestination(random);
        Debug.Log(agent.velocity.magnitude);
    }



    public void Restart()
    {
        SceneManager.LoadSceneAsync("TestScene");
    }

}
