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


        // Contoh: SphereCast untuk mengecek apakah titik checkPoint berada di dalam wilayah
        Collider[] colliders = Physics.OverlapSphere(shooter.position, sphereRadius, layerMask);

        if (colliders.Length == 0)
        {
            target.position = baseTargetLocation.position;
            return;
        }

        foreach (var collider in colliders)
        {
            if (collider.gameObject.TryGetComponent(out targetable))
            {
                target.position = targetable.TargetParent().position;
                break;
            }
        }
    }
}