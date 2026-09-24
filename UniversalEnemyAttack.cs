using UnityEngine;

namespace Doom_Dude.EnemyASI
{
    public enum AttackType
    {
        Melee,
        RangedRaycast,
        RangedProjectile
    }

    public class UniversalEnemyAttack : EnemyAttack
    {
        [Header("Universal Attack Settings")]
        public AttackType attackType = AttackType.Melee;
        [SerializeField] private LayerMask targetMask;

        [Header("Professional Animation Settings")]
        [Tooltip("Drop an Animator Override Controller here (e.g. Pistol, Rifle, Melee) to change this enemy's animations without changing the code!")]
        public AnimatorOverrideController weaponAnimatorOverride;
        [Tooltip("How many attack variations does this weapon have in the animator? (Starts at 0)")]
        public int numberOfAttackVariations = 1;

        [Header("Audio Settings")]
        [Tooltip("Optional. Attack sounds (gunshots, punches, sword swings)")]
        [SerializeField] private AudioClip[] attackSounds;
        [SerializeField] private float attackSoundVolume = 1f;
        [SerializeField] private float attackSoundRadius = 25f;
        [SerializeField] [Range(0.8f, 1.2f)] private float attackPitchMin = 0.95f;
        [SerializeField] [Range(0.8f, 1.2f)] private float attackPitchMax = 1.05f;

        [Header("Melee Settings")]
        [SerializeField] private Transform meleeHitPoint;
        [SerializeField] private float meleeHitRadius = 0.5f;

        [Header("Ranged Settings")]
        [SerializeField] private Transform gunBarrelPoint;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileSpeed = 20f;
        [SerializeField] private GameObject muzzleFlashPrefab;

        private Transform currentTarget;

        private void Awake()
        {
            if (targetMask.value == 0)
            {
                Debug.LogError($"<color=red><b>CRITICAL SETUP ERROR:</b></color> On '{gameObject.name}', the UniversalEnemyAttack script has its Target Mask set to 'Nothing'! You must set it to 'Player' or the attacks will pass straight through!", gameObject);
            }
        }

        protected override void ExecuteAttack(Transform target)
        {
            // Store target for when the animation event fires
            currentTarget = target;
            
            // The actual attack animation is triggered by EnemyAI.
            // We wait for the animation event to call TriggerAttackEvent().
        }

        public override void TriggerAttackEvent()
        {
            if (currentTarget == null) return;

            // Play Attack SFX if assigned
            if (attackSounds != null && attackSounds.Length > 0)
            {
                PlayProfessional3DSound(attackSounds, attackSoundVolume, attackSoundRadius, attackPitchMin, attackPitchMax);
            }

            switch (attackType)
            {
                case AttackType.Melee:
                    PerformMeleeAttack();
                    break;
                case AttackType.RangedRaycast:
                    PerformRaycastAttack();
                    break;
                case AttackType.RangedProjectile:
                    PerformProjectileAttack();
                    break;
            }
        }

        private void PerformMeleeAttack()
        {
            Vector3 hitCenter = meleeHitPoint != null ? meleeHitPoint.position : transform.position + transform.forward;
            Collider[] hitTargets = Physics.OverlapSphere(hitCenter, meleeHitRadius, targetMask);
            
            foreach (var hit in hitTargets)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log("Universal Enemy hit target with Melee!");
                }
            }
        }

        private void PerformRaycastAttack()
        {
            Vector3 spawnPos = gunBarrelPoint != null ? gunBarrelPoint.position : transform.position + transform.forward + Vector3.up;
            Vector3 aimDirection = (currentTarget.position - spawnPos).normalized;

            if (muzzleFlashPrefab != null)
            {
                Instantiate(muzzleFlashPrefab, spawnPos, Quaternion.LookRotation(aimDirection));
            }

            if (Physics.Raycast(spawnPos, aimDirection, out RaycastHit hit, attackRange, targetMask))
            {
                Debug.Log($"<color=cyan>Raycast Hit Object:</color> {hit.collider.name}"); // Tells us exactly what it hit!

                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log("Universal Enemy hit target with Raycast!");
                }
                else
                {
                    Debug.LogWarning($"<color=orange>Raycast hit {hit.collider.name}, but it doesn't have an IDamageable script attached!</color>");
                }
            }
            else
            {
                Debug.Log($"<color=yellow>Raycast Fired, but missed all colliders on the TargetMask layer!</color>");
            }
            Debug.DrawRay(spawnPos, aimDirection * attackRange, Color.red, 2f);
        }

        private void PerformProjectileAttack()
        {
            Vector3 spawnPos = gunBarrelPoint != null ? gunBarrelPoint.position : transform.position + transform.forward + Vector3.up;
            Vector3 aimDirection = (currentTarget.position - spawnPos).normalized;

            if (muzzleFlashPrefab != null)
            {
                Instantiate(muzzleFlashPrefab, spawnPos, Quaternion.LookRotation(aimDirection));
            }

            if (projectilePrefab != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(aimDirection));
                
                // Initialize the bullet damage and layers!
                EnemyProjectile projScript = projectile.GetComponent<EnemyProjectile>();
                if (projScript != null)
                {
                    projScript.Initialize(attackDamage, targetMask);
                }

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = aimDirection * projectileSpeed;
                }
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (attackType == AttackType.Melee)
            {
                Gizmos.color = Color.red;
                Vector3 hitCenter = meleeHitPoint != null ? meleeHitPoint.position : transform.position + transform.forward;
                Gizmos.DrawWireSphere(hitCenter, meleeHitRadius);
            }
        }

        // --- Utility ---
        private void PlayProfessional3DSound(AudioClip[] clips, float volume, float radius, float minPitch, float maxPitch)
        {
            if (clips == null || clips.Length == 0) return;
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (clip == null) return;

            GameObject audioObj = new GameObject("TempAttackAudio");
            audioObj.transform.position = transform.position;
            AudioSource source = audioObj.AddComponent<AudioSource>();
            
            source.clip = clip;
            source.volume = volume;
            source.pitch = Random.Range(minPitch, maxPitch);
            
            // Professional 3D Settings
            source.spatialBlend = 1f; 
            source.maxDistance = radius;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.dopplerLevel = 0f;
            
            source.Play();
            Destroy(audioObj, clip.length + 0.1f);
        }
    }
}
