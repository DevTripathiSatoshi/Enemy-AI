using UnityEngine;
using UnityEngine.AI;

namespace Doom_Dude.EnemyASI
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimationHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private EnemyAI enemyAI;
        [SerializeField] private EnemyHealth enemyHealth;
        [SerializeField] private EnemyVision enemyVision;
        [SerializeField] private EnemyAttack enemyAttack;
        [SerializeField] private NavMeshAgent agent;

        [Header("Animation Settings")]
        [SerializeField] private bool useAimingState = true;
        [SerializeField] private bool useHitReactions = true;

        [Header("Footstep Audio Settings")]
        [Tooltip("Add footstep clips here. It will randomly pick one to avoid sounding repetitive.")]
        [SerializeField] private AudioClip[] footstepSounds;
        [SerializeField] private float footstepVolume = 0.5f;
        [SerializeField] private float footstepRadius = 15f;
        [SerializeField] [Range(0.8f, 1.2f)] private float footstepPitchMin = 0.9f;
        [SerializeField] [Range(0.8f, 1.2f)] private float footstepPitchMax = 1.1f;

        // Animator Hashes for Kevin Iglesias pack mapping
        private readonly int hashIsAiming = Animator.StringToHash("IsAiming");
        private readonly int hashVelocityX = Animator.StringToHash("VelocityX");
        private readonly int hashVelocityZ = Animator.StringToHash("VelocityZ");
        private readonly int hashHit = Animator.StringToHash("Hit");

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (enemyAI == null) enemyAI = GetComponentInParent<EnemyAI>();
            if (enemyHealth == null) enemyHealth = GetComponentInParent<EnemyHealth>();
            if (enemyVision == null) enemyVision = GetComponentInParent<EnemyVision>();
            if (enemyAttack == null) enemyAttack = GetComponentInParent<EnemyAttack>();
            if (agent == null) agent = GetComponentInParent<NavMeshAgent>();
        }

        private void Start()
        {
            // Apply Weapon Animation Override if provided
            UniversalEnemyAttack universalAttack = enemyAttack as UniversalEnemyAttack;
            if (animator != null && universalAttack != null && universalAttack.weaponAnimatorOverride != null)
            {
                animator.runtimeAnimatorController = universalAttack.weaponAnimatorOverride;
            }
        }

        private void OnEnable()
        {
            if (enemyHealth != null && useHitReactions)
            {
                enemyHealth.OnTakeDamage.AddListener(PlayHitReaction);
            }
        }

        private void OnDisable()
        {
            if (enemyHealth != null && useHitReactions)
            {
                enemyHealth.OnTakeDamage.RemoveListener(PlayHitReaction);
            }
        }

        private void Update()
        {
            if (enemyHealth != null && enemyHealth.IsDead) return;

            UpdateMovementAnimation();
            UpdateAimingState();
        }

        private void UpdateMovementAnimation()
        {
            if (agent != null && animator != null)
            {
                // Convert world velocity to local velocity for strafing support (2D Blend Tree)
                Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
                
                // localVelocity.x is left/right (- for left, + for right)
                // localVelocity.z is forward/backward (- for back, + for forward)
                
                animator.SetFloat(hashVelocityX, localVelocity.x, 0.1f, Time.deltaTime);
                animator.SetFloat(hashVelocityZ, localVelocity.z, 0.1f, Time.deltaTime);
            }
        }

        private void UpdateAimingState()
        {
            if (!useAimingState || animator == null || enemyVision == null) return;

            // If the enemy sees the player, switch to "Aiming" posture
            bool shouldAim = enemyVision.IsPlayerDetected;
            animator.SetBool(hashIsAiming, shouldAim);
        }

        private void PlayHitReaction()
        {
            if (animator != null)
            {
                animator.SetTrigger(hashHit);
            }
        }

        // --- Animation Events ---
        // These can be called by Animation Events on the Kevin Iglesias animations
        
        public void AnimEvent_Footstep()
        {
            if (footstepSounds != null && footstepSounds.Length > 0)
            {
                PlayProfessional3DSound(footstepSounds, footstepVolume, footstepRadius, footstepPitchMin, footstepPitchMax);
            }
        }

        public void AnimEvent_Shoot()
        {
            TriggerAttack();
        }

        public void AnimEvent_MeleeHit()
        {
            TriggerAttack();
        }

        // Called by StateMachineBehaviour as an alternative to Animation Events
        public void TriggerAttack()
        {
            if (enemyAttack != null)
            {
                enemyAttack.TriggerAttackEvent();
            }
        }

        // --- Utility ---
        private void PlayProfessional3DSound(AudioClip[] clips, float volume, float radius, float minPitch, float maxPitch)
        {
            if (clips == null || clips.Length == 0) return;
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (clip == null) return;

            GameObject audioObj = new GameObject("TempFootstepAudio");
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
