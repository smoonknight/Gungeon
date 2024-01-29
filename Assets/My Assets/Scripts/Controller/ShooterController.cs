using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class ShooterController : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] private Transform baseTargetLocation;
    [SerializeField] private Rig shooterRig;
    [SerializeField] private Transform shooter;
    [SerializeField] private LayerMask layerMask;
    public const float sphereRadius = 15f;


    private void Update()
    {
        CheckingTarget();
    }

    void CheckingTarget()
    {
        ITargetable targetable = null;


        Collider[] colliders = Physics.OverlapSphere(shooter.position, sphereRadius, layerMask);


        foreach (var collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out targetable))
            {
                if (!targetable.IsTargetable())
                {
                    continue;
                }
                target.position = targetable.TargetObject().position;
                return;
            }
        }

        target.position = baseTargetLocation.position;
        return;
    }

    public void Shoot(GameObject bullet, Transform bulletSpawn)
    {

        AudioManager.Instance.PlayOnShot("Shot");
        Rigidbody rbPeluru = bullet.GetComponent<Rigidbody>();
        Vector3 direction = (target.position - bulletSpawn.position).normalized;
        rbPeluru.velocity = direction * 25;
        Destroy(bullet.gameObject, 3);
    }
}