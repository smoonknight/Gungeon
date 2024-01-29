using UnityEngine;

public class PropertyDamage : MonoBehaviour
{
    public LayerMask layerMask;
    public int layer;
    public bool destroyOnHit = true;

    public int minimumRangeDamage = 5;
    public int maximumRangeDamage = 20;

    bool takeDamage = false;



    private void OnTriggerEnter(Collider other)
    {
        if (takeDamage)
        {
            return;
        }

        if (other.gameObject.layer != layer)
        {
            Debug.Log(layer);
            return;
        }

        IDamageable damageable;
        if (other.TryGetComponent(out damageable))
        {
            AudioManager.Instance.PlayOnShot("Bullet Shot");
            takeDamage = true;
            int damage = Random.Range(minimumRangeDamage, maximumRangeDamage);

            damageable.TakeDamage(damage);
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
