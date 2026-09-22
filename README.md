<div align="center">

# 💀 DOOM DUDE'S ENEMY AI SYSTEM 💀

*The Ultimate, Modular, Plug-and-Play AI Framework for Unity!*

[![Unity](https://img.shields.io/badge/Unity-2021%2B-black?style=for-the-badge&logo=unity)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

</div>

Welcome to the **Enemy AI System**, a robust, highly modular, and professionally architected State Machine based Artificial Intelligence framework designed for Unity. Whether you need a simple melee zombie, a patrolling guard, or a sophisticated gun-wielding mercenary, this framework handles it all with pristine, decoupled C# scripts!

---

## 🌟 FEATURES

- **State Machine Architecture:** Clean `Idle`, `Walk` (Patrol), `Run` (Chase), `Attack`, and `Die` states.
- **Universal Attack System:** One script to rule them all! Easily switch between Melee, Raycast Shooting, and Projectile Shooting directly from the Inspector.
- **Animator Overrides:** Seamlessly swap attack animations (Pistol, Rifle, Melee) without touching a single line of code.
- **Advanced Vision Cone:** Built-in line-of-sight checking, field of view angles, and proximity detection so enemies never look stupid when you sneak right next to them.
- **Professional Gizmos:** Visual debugging in the Scene view lets you balance gameplay instantly without guessing ranges.
- **Highly Modular:** Don't want patrol behavior? Just remove the script. The AI automatically adapts!

---

## 🛠️ HOW TO SETUP AND USE

### 1. Basic Enemy Setup
1. Create an empty GameObject (or use your 3D model) and name it `Enemy`.
2. Add a **NavMeshAgent** component to it (and ensure your scene has a baked NavMesh).
3. Attach the `EnemyAI` script to your Enemy GameObject. 
   *(This will automatically require and link a NavMeshAgent)*.
4. Attach the core modular scripts: `EnemyVision`, `EnemyHealth`, `EnemyPatrol`, and `UniversalEnemyAttack` (or `NormalEnemyAttack`/`GunEnemyAttack`).
5. Ensure you have an **Animator** component assigned if you want animations to play.

### 2. Configuring the AI
- **Vision:** Set the `Target Mask` in `EnemyVision` to your Player's layer. Set the `Obstacle Mask` to walls/environment so the enemy can't see through walls.
- **Patrolling:** Create empty GameObjects in your scene to act as waypoints. Assign them to the `Patrol Points` array in the `EnemyPatrol` script.
- **Attacking:** Set the `Attack Range` in your chosen Attack script. If using `UniversalEnemyAttack`, select your `Attack Type` (Melee, Raycast, Projectile) and assign the required prefabs/points.

### 3. Layer Setup
Make sure your Player GameObject has a dedicated Layer (e.g., "Player") and that it has an `IDamageable` interface attached to receive damage.

---

## 📜 SCRIPTS AND THEIR FUNCTIONALITY

The system is built on the SOLID principles, keeping behaviors decoupled and extremely reusable.

### 🧠 Core Controller
- **`EnemyAI.cs`**: The brain of the operation. It manages the State Machine, handles transitions between patrolling, chasing, attacking, and dying. It communicates with all other scripts to decide what to do next based on sensory input.

### 👁️ Sensory & Movement
- **`EnemyVision.cs`**: Handles the sight of the enemy. It uses a view radius and a view angle to detect targets. It includes a "proximity radius" so players can't easily circle-strafe the AI at point-blank range without being noticed.
- **`EnemyPatrol.cs`**: Manages waypoints. It tells the AI where to go during the `Walk` state and handles waiting times at each node.

### ⚔️ Combat & Health
- **`EnemyHealth.cs`**: Manages the enemy's hit points, passive health regeneration, taking damage, and dying. It implements the `IDamageable` interface.
- **`EnemyAttack.cs`**: An abstract base class for all attacks. It handles attack cooldowns and base damage values.
- **`UniversalEnemyAttack.cs`**: The ultimate weapon script. Allows you to choose between Melee, Raycast (hitscan), and Projectile based attacks. It integrates with Unity's Animation Events to sync damage perfectly with your attack animations.
- **`NormalEnemyAttack.cs` / `GunEnemyAttack.cs`**: Simpler, specialized alternatives to the Universal script for basic melee or basic shooting.

---

## 🎨 THE GIZMO GUIDE (Visual Debugging)

To make level design and AI balancing a breeze, the scripts draw helpful, professional Gizmos in the Unity Scene view. Here is what every circle and line means:

| Gizmo Visual | Script Origin | What it represents |
| :--- | :--- | :--- |
| ⚪ **White Circle** | `EnemyVision.cs` | **View Radius:** The absolute maximum distance the enemy can see the player. |
| ⚪ **White Lines** | `EnemyVision.cs` | **View Cone:** The angle (Field of View) representing the enemy's peripheral vision. |
| 🟡 **Yellow Circle** | `EnemyVision.cs` | **Proximity Radius:** A "sixth sense" radius. If the player steps inside this circle, they are detected immediately, even if they are behind the enemy. |
| 🟣 **Purple (Magenta) Circle** | `EnemyAttack.cs` | **Attack Range:** The distance required for the AI to transition from Chase (Run) to Attack state. |
| 🔴 **Red Circle** | `EnemyAttack.cs` | **Melee Hit Radius:** The actual physical area where melee damage is applied when an attack connects. |
| 🔴 **Red Line** | `EnemyVision.cs` | **Target Lock:** Appears connecting the enemy to the player when the player is actively detected and in line of sight. |
| 🟢 **Green Spheres & Lines** | `EnemyPatrol.cs` | **Patrol Path:** Shows your assigned patrol waypoints and the path the enemy will take between them. |

---

<div align="center">
<i>"Design enemies efficiently. Let the AI do the heavy lifting."</i>
</div>
