using UnityEngine;

namespace Doom_Dude.EnemyASI
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public float autoDestroyTime = 5f;
        public GameObject hitEffectPrefab;

        private float damageToDeal = 0f;
        private LayerMask hitMask;
        private bool isInitialized = false;

        // Called automatically by UniversalEnemyAttack when the projectile is spawned
        public void Initialize(float damage, LayerMask mask)
        {
            damageToDeal = damage;
            hitMask = mask;
            isInitialized = true;
            
            // Clean up bullet if it flies off into space
            Destroy(gameObject, autoDestroyTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isInitialized) return;

            // Check if the object we hit is in our Target Mask layer (using bitwise check)
            if (((1 << collision.gameObject.layer) & hitMask) != 0)
            {
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damageToDeal);
                    Debug.Log($"Projectile hit {collision.gameObject.name} for {damageToDeal} damage!");
                }
            }

            // Spawn sparks/blood if assigned
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // Destroy the bullet upon impact
            Destroy(gameObject);
        }
    }
}
