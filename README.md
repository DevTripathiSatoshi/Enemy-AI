# Universal Enemy AI System for Unity 🤖

A highly optimized, modular, and scalable Enemy AI framework for Unity. Built for professional game development, this system uses 2D Blend Trees, Animator Overrides, and a state-machine-driven architecture to let you create anything from Snipers to Melee Brawlers in minutes.

## 🌟 Features

- **Universal Architecture**: Write the code once. Use `Animator Override Controllers` to swap between Rifle, Pistol, or Melee animations on the fly.
- **Smart Crowd Control**: Advanced NavMesh setup prevents enemies from deadlocking, clustering, or sliding across the floor when swarming the player.
- **Flawless Attack Synchronization**: Replaces fragile Animation Events with `AttackStateBehaviour`, a custom `StateMachineBehaviour` that mathematically syncs damage with animation frames (e.g., exactly halfway through a punch).
- **Dynamic 3D Audio**: Built-in 3D audio system for perfectly overlapping footsteps and attack sounds without needing complex Object Pools.
- **Plug-and-Play Projectiles**: Easily switch between Melee, Raycast (hitscan), and physical Projectiles.

## 📁 Installation

Simply drop this `Enemy ASI` folder into your Unity project's `Assets` directory. 

## 🎮 Setup Guide

### 1. The Animator
1. Create a Base Animator Controller (`UniversalEnemyController`) using a 2D Locomotion Blend Tree (`VelocityX` and `VelocityZ`).
2. Add triggers for `Attack`, `Hit`, and `Die`. Use an integer `AttackIndex` to randomize attacks.
3. Attach the included `AttackStateBehaviour` script to your Attack animation nodes in the Animator window.
4. *For specific enemies*: Create an **Animator Override Controller**, plug in your weapon-specific animations, and assign it to the enemy.

### 2. The Components
Add the following components to a GameObject equipped with a `NavMeshAgent` and a `CapsuleCollider`:
- **`EnemyAI`**: The brain. Set Walk/Run speed here.
- **`EnemyVision`**: Controls Field of View (Radius, Angle, and a close-quarters Proximity override).
- **`EnemyHealth`**: Handles HP, auto-regeneration, and delayed despawning.
- **`EnemyPatrol`**: Create empty GameObjects and assign them as waypoints.
- **`UniversalEnemyAttack`**: The core attack handler. Assign your Animator Override, choose the Attack Type (Melee/Raycast/Projectile), and set your `Target Mask` (e.g., Player).

### 3. Dealing Damage (Player Integration)
Because the system is built cleanly on the `IDamageable` interface, integrating your player's weapons is effortless.

```csharp
using Doom_Dude.EnemyASI;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float damage = 25f;

    // Example Raycast Attack
    void Shoot()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
        {
            IDamageable enemy = hit.collider.GetComponent<IDamageable>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
```

## 🔧 Optimization & Performance
- **NavMesh Sliding Fix**: The `EnemyAI` automatically zeroes out velocity when stopping, and dynamically adjusts stopping distance based on patrol/attack states to prevent floaty "ice-skating" visual bugs.
- **Avoidance Deadlocks**: `EnemyAI` randomizes avoidance priority on spawn, ensuring massive hordes naturally yield to each other instead of locking up.

## 📝 License
MIT License - Free to use in your personal and commercial projects!
