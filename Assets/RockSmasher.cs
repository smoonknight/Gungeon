using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSmasher : MonoBehaviour
{
    MeshCollider meshCollider;
    private void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        GameObject fireball = ParticleManager.Instance.GetParticle(ParticleManager.ParticleType.fireball);
        Instantiate(fireball, transform);
        StartCoroutine(OnTriggerEnter());
    }
    private IEnumerator OnTriggerEnter()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() =>
        {
            RaycastHit hit;
            float raycastDistance = 1f;
            Ray ray = new Ray(meshCollider.bounds.center, Vector3.down);
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                return hit.distance <= raycastDistance;
            }
            return false;
        });
        CameraShakeManager.Instance.StartShake(1.5f, 0.4f);
        ParticleManager.Instance.PlayParticleOnTime(ParticleManager.ParticleType.explosion, transform.position);
    }
}
