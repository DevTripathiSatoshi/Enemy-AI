<div align="center">
  
# 🤖 Universal Enemy AI System for Unity

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity](https://img.shields.io/badge/Unity-2022%2B-black?logo=unity)](https://unity.com/)
[![Code: C#](https://img.shields.io/badge/Code-C%23-blue.svg?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)

**A highly optimized, modular, and scalable Enemy AI framework for Unity.**<br>
Built for professional game development, this system uses 2D Blend Trees, Animator Overrides, and a state-machine-driven architecture to let you create anything from Snipers to Melee Brawlers in minutes.

</div>

---

## ✨ Key Features

*   **Universal Architecture**: Write the code once. Use `Animator Override Controllers` to instantly swap between Rifle, Pistol, or Melee animations without changing any code.
*   **Smart Crowd Control**: Advanced NavMesh setup prevents enemies from deadlocking, clustering, or sliding across the floor when swarming the player.
*   **Flawless Attack Synchronization**: Replaces fragile Animation Events with `AttackStateBehaviour`, a custom `StateMachineBehaviour` that mathematically syncs damage with animation frames (e.g., *exactly* halfway through a punch).
*   **Dynamic 3D Audio**: Built-in 3D audio system for perfectly overlapping footsteps and attack sounds without needing complex Object Pools.
*   **Plug-and-Play Projectiles**: Easily switch between Melee, Raycast (hitscan), and physical Projectiles with a single dropdown.

---

## 📁 Installation

> [!TIP]
> **The Fastest Way to Install**
> Download the `.unitypackage` from the **Releases** tab and import it directly into your project! 
> Alternatively, drop the `Enemy ASI` folder directly into your `Assets` directory.

---

## 🎮 Setup Guide (Zero-to-Hero in 3 Minutes)

### 1. The Base Animator
1. Create a Base Animator Controller (`UniversalEnemyController`) using a 2D Locomotion Blend Tree (`VelocityX` and `VelocityZ`).
2. Add triggers for `Attack`, `Hit`, and `Die`. Use an integer `AttackIndex` to randomize attacks.
3. Attach the included `AttackStateBehaviour` script to your Attack animation nodes.
4. *For specific enemies*: Create an **Animator Override Controller**, plug in your weapon-specific animations, and assign it to the enemy.

### 2. The Components
Attach the following components to a GameObject equipped with a `NavMeshAgent` and a `CapsuleCollider`:

| Component | Purpose |
| :--- | :--- |
| **`EnemyAI`** | The state-machine brain. Controls switching between Idle, Patrol, Chase, Attack, and Death. |
| **`EnemyVision`** | Controls Field of View. Includes Radius, Angle, and a close-quarters Proximity override. |
| **`EnemyHealth`** | Handles HP, auto-regeneration, and delayed despawning. |
| **`EnemyPatrol`** | Create empty GameObjects and assign them as patrol waypoints. |
| **`UniversalEnemyAttack`** | The core attack handler. Assign your Animator Override, choose the Attack Type, and set your `Target Mask`. |

---

## ⚔️ Player Integration (Dealing Damage)

Because the system is built cleanly on the `IDamageable` interface, integrating your player's weapons is effortless. No spaghetti code required!

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
            // Just look for the IDamageable interface!
            IDamageable enemy = hit.collider.GetComponent<IDamageable>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
```

---

## 🔧 Under-The-Hood Optimizations

> [!NOTE]
> We spent significant time debugging standard Unity AI quirks to make this system AAA-ready out of the box.

*   **NavMesh "Ice Skating" Fix**: The `EnemyAI` automatically zeroes out velocity when stopping, and dynamically adjusts stopping distance based on patrol/attack states to prevent floaty "ice-skating" visual bugs when transitioning between animations.
*   **Avoidance Deadlocks**: Standard Unity agents lock up when clustering at the exact same waypoint. `EnemyAI` randomizes avoidance priority on spawn, ensuring massive hordes naturally yield to each other instead of jamming.

---

## 📜 License
This project is licensed under the **MIT License** - completely free to use in your personal and commercial projects!
