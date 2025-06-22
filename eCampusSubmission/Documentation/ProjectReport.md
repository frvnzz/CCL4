# Code Review Report
Course: CCL4 SS 2025 (5 ECTS, 3 SWS)

CCL Group: 8

Names: 
- Selina Hacker - cc231072
- Alikhan Manat - cc231008
- Marcus Fichtinger - cc231016
- Franz-Aurel Huber - cc231014

Your Project Name: Deadwood

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
All fonts we used are documented in [`FontSources.md`](../../UnityProject/Assets/Fonts/FontSources.md)

- Game Audio:
  1. Enemy sounds  
Include a variety of sound files for zombie actions, like attack sounds, growling, footsteps and death sounds
  2. Player sounds  
Include sounds like jumping, shooting with different weapons, reloading and footsteps
  3. UI sounds  
Include sounds for button clicks, hitmarkers, background music and a sound effect to indicate a completed wave
  4.  Environment sounds  
Include all other sounds like explosions and water
  5. Conversion
Convert the audio files for smaller file sizes with Auto Medium Sample Rate and Vorbis Format with Quality Level 2.

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
##### Player Scripts
  - PlayerCamera.cs
    Uses the Vector2 from Unitys new Input System to move the camera using the mouse. The rotation is limited, so the player can only look up and down so far.
  - PlayerMovement.cs
    The player is being moved by the physics engine using the rigidbody and linear velocity. Because physics are being used, the movement needs to happen in the fixed updated method. For the jump method a simple ground check is done, by sending out a raycast to the bottom. When the player is a allowed to jump, a force is added to the rigidbody. There is also the isSprinting boolean. Depending on its value the speed of the player is set accordingly.
  - WeaponManager.cs
    There is a lot happening in this script, but to keep it short: there are multiple methods for different purposes, such as: Fire(), Reloading(), HandleGunSwitch(), EquipGun(), AddAmmo(), etc.
    Fire() does what it says it does. It handles all of the raycasts and depending on the gun, the fire rate if it is automatic. The stats of a weapon are defined by the GunStats class that is applied to each prefab of a weapon.
    HandleGunSwitch and EquipGun handle all of the changing of the current gun stats and keep track of everything. For the purpose of storing the state of each gun, when switching, a List of type WeaponInstance is used, which store all of the current states.
    AddAmmo() is used when the player picks up an ammo crate and is called by the individual crate on collision enter.
  - WeaponWobble.cs
    Moves the weapon depending on the input vectors of the player. So it moves depending on the movement of the player and when shooting it adds knockback to the weapon.
    When reloading the gun is being pulled to the bottom of the view to visualize the reloading process.
  - WeaponInstance.cs
    This is class for storing the current state of a weapon. This helps keeping track of everything for switching between different weapons.
  - GunStats.cs
    This script is applied to the prefabs of the weapons beforehand. Here adjustments can be made for different properties of the weapons, such as fire rate, total ammo it can have, ammo per magazine, if it is automatic, the damage, the knockback amount, the muzzle flash and much more. There are tooltips for most characteristics to help with filling out the stats for a new weapon. The name of the audio event for each weapon is also set in the stats script.
  
##### AI
  - AIController.cs
    The basic enemy navigation is done by the NavMeshAgent attached to every enemy. The destination is set to the position of the player in the update method.
    When the player is in reach, the enemy will attack the player and play the according animation. Because of the animation playing, a coroutine is used to delay the damage being applied to the player until the animation has played and the player is still in reach after that time. This script also handles the death of the enemy and enables the ragdoll once an enemy has been defeated.

##### Interactables
  - AmmoBoxPickup.cs
    The pickup of the ammo box is triggered by entering the collision box of the ammo crate. This call the respective method in the weapon manager.
  - Explosive.cs
    When the player raycast hits an explosive, the explode() method is triggered. This will play the particle effect, play the sound, deal damage to the enemies in range and then destroy the instance.

##### UI
  - HUD.cs
    This script has all the methods to set the UI text for things like score, ammunition, health, game over screen, etc.
  - SceneTransition.cs
    Has the method to change the scene, when a button is clicked.
  - ScorePopup.cs
    Handles the text that pops up when the player hits an enemy and a score is added to the total score.
  - DamageVignette.cs
    Handles the fading of the damage vignette/red screen, when the player has been hit.
  - HandleToggle.cs
    Handles setting the boolean of the checkbox in the main menu settings for activating "unlimited enemies", which essentially makes the enemies spawn amount increase faster and the limit of concurrent enemies alive is increased.
  - PauseManager.cs
    Handles the pause screen when escape is pressed during gameplay. It pauses the game and handles the menu that appeared.
  - UIButtonSound.cs
    Plays the button sound when a button is clicked in the main menu.


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