# Code Review Report
Course: CCL4 SS 2025 (5 ECTS, 3 SWS)

CCL Group: 8

Names: 
- Selina Hacker - cc231072
- Alikhan Manat - cc231008
- Marcus Fichtinger - cc231016
- Franz-Aurel Huber - cc231014

Your Project Name: 

#

### A Short Summary to Promote the Project (What are the Background and the Motivation of the project?): 
The main inspiration of our project was a game called [Call of Mini: Zombies](https://call-of-mini-zombies.de.uptodown.com/android) that we used to play when we were kids. The game is no longer compatible or available in the App Store or on Google Play and the servers have been shut down a long time ago. We wanted to create a simple first-person, arcade-like wave-shooter in a similar cartoony and blocky artstyle.

### Key Features and Implementation Detail

- 3D Modeling:
  1. Three different zombies that are fully textured and rigged. They all have an idle animation, a walking animation and an attack animation. (see [`UnityProject/Assets/Prefabs/Zombies`](../../UnityProject/Assets/Prefabs/Zombies))
  2. An explosive barrel model (see [`UnityProject/Assets/Models/explosive.fbx`](../../UnityProject/Assets/Models/explosive.fbx))
  3. An AK-47 model (see [`UnityProject/Assets/Models/ak.fbx`](../../UnityProject/Assets/Models/ak.fbx))
  4. A pistol model (see [`UnityProject/Assets/Models/pistol.fbx`](../../UnityProject/Assets/Models/pistol.fbx))
  5. A blaster model (see [`UnityProject/Assets/Models/blaster.fbx`](../../UnityProject/Assets/Models/blaster.fbx))
  6. A car model (see [`UnityProject/Assets/Models/car.fbx`](../../UnityProject/Assets/Models/car.fbx))
  7. A house model (see [`UnityProject/Assets/Models/house.fbx`](../../UnityProject/Assets/Models/house.fbx))

All other assets that were not created by us have been documented in [`ThirdPartyAssets.md`](../../ThirdPartyAssets.md)

- Game Audio:
  1. Enemy sounds  
Include a variety of sound files for zombie actions, like attack sounds, growling, footsteps and death sounds
  2. Player sounds  
Include sounds like jumping, shooting with different weapons, reloading and footsteps
  3. UI sounds  
Include sounds for button clicks, hitmarkers, background music and a sound effect to indicate a completed wave
  4.  Environment sounds  
Include all other sounds like explosions and water

- Unity Coding:
  1. Enemy AI that handles chasing and attacking the player ([`AIController.cs`](../../UnityProject/Assets/Scripts/AIController.cs))
  2. Player movement with jumping ([`PlayerMovement.cs`](../../UnityProject/Assets/Scripts/Player%20Scripts/PlayerMovement.cs))
  3. Weapon system with raycast ([`WeaponManager.cs`](../../UnityProject/Assets/Scripts/Player%20Scripts/WeaponManager.cs))
  4. Switching between different weapons ([`WeaponManager.cs`](../../UnityProject/Assets/Scripts/Player%20Scripts/WeaponManager.cs))
  5. Reload system ([`WeaponManager.cs`](../../UnityProject/Assets/Scripts/Player%20Scripts/WeaponManager.cs))
  6. Ammunition system with interactables ([`AmmoBoxPickup.cs`](../../UnityProject/Assets/Scripts/AmmoBoxPickup.cs))
  7. Interactable explosive barrels that deal aera damage ([`Explosive.cs`](../../UnityProject/Assets/Scripts/Explosive.cs))
  8. Pause menu ([`PauseManager.cs`](../../UnityProject/Assets/Scripts/PauseManager.cs))
  9. Persistent volume and sensitivity settings ([`SliderSettings.cs`](../../UnityProject/Assets/Scripts/SliderSettings.cs), [`SliderVolume.cs`](../../UnityProject/Assets/Scripts/SliderVolume.cs))

- C# & Theory of CG&A:
  1. Wave management system with increasing amounts of enemies per wave and cooldowns between them ([`SpawnController.cs`](../../UnityProject/Assets/Scripts/SpawnController.cs))
  2. Ammunition box spawn system ([`SpawnController.cs`](../../UnityProject/Assets/Scripts/SpawnController.cs))

#### Implementation Logic Explanation:
(Explain how you implement the idea step by step compactly and clearly.)

#### Three Important Achievements:
1. Interactable systems like the explosive barrels, ammo pickups, and persistent settings for a polished feel
2. High quality 3D models that go beyond the minimum requirements of the CCL
3. Enemy AI with pathfinding and enemy attacks with corresponding animations

### Learned Knowledge from the Project

#### Major Challenges and Solutions:
1. AI/NavMesh integration
2. Building the game threw many errors and we had lots of pathing issues when it comes to Wwise. Fixing this needed a lot of troubleshooting
3. Version control with Wwise and Unity using Git was very inconvenient and constantly required major conflict management

#### Minor Challenges and Solutions:
1. Wave and Spawn system of enemies
2. Unity particle system for things like muzzle flash and blood splatter

### Reflections on the Own Project:
Things we could add to the game it we had more time include
1. More variety in enemies
2. Score points shop
3. Score leaderboard
4. Improve animation quality and accuracy
5. Higher quality audio
6. Improve the level to increase player engagement
7. A boss enemy every X waves