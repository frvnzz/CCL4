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
The main inspiration of our project was a game called [Call of Mini: Zombies](https://call-of-mini-zombies.de.uptodown.com/android) that we used to play when we were kids. The game is no longer compatible or available in the App Store or on Google Play and the servers have been shut down. We wanted to create a simple third-person, arcade-like wave-shooter in a similar cartoony and blocky artstyle.

### Key Features and Implementation Detail

- 3D Modeling:
  1. Three different zombies that are fully textured and rigged. They all have an idle animation, a walking animation and an attack animation. (see [`UnityProject/Assets/Prefabs/Zombies`](../../UnityProject/Assets/Prefabs/Zombies))
  2. An explosive barrel model (see [`UnityProject/Assets/Models/explosive.fbx`](../../UnityProject/Models/explosive.fbx))
  3. An AK-47 model (see [`UnityProject/Assets/Models/ak.fbx`](../../UnityProject/Models/ak.fbx))
  4. A pistol model (see [`UnityProject/Assets/Models/pistol.fbx`](../../UnityProject/Models/pistol.fbx))
  5. A blaster model (see [`UnityProject/Assets/Models/blaster.fbx`](../../UnityProject/Models/blaster.fbx))
  6. A car model (see [`UnityProject/Assets/Models/car.fbx`](../../UnityProject/Models/car.fbx))
  7. A house model (see [`UnityProject/Assets/Models/house.fbx`](../../UnityProject/Models/house.fbx))

All other models that were not created by us are documented in [`ThirdPartyAssets.md`](../../ThirdPartyAssets.md)

- Game Audio:
  1. Enemy sounds  
Include a variety of sound files for zombie actions, like attack sounds, growling, footsteps and death sounds.
  2. Player sounds  
Include sounds like jumping, shooting with different weapons, reloading and footsteps.
  3. UI sounds  
Include sounds for button clicks, hitmarkers, background music and a sound effect to indicate a completed wave.
  4.  Environment sounds  
Include all other sounds like explosions and water.

- Unity Coding:
  1. Enemy AI that handles chasing and attacking the player ([`AIController.cs`](../../UnityProject/Assets/Scripts/AIController.cs))

- C# & Theory of CG&A:
  1. Item
  2. Item, and so forth

#### Implementation Logic Explanation:
(Explain how you implement the idea step by step compactly and clearly.)

#### Three Important Achievements:
(List down and explain 3 important achievements you are proud of (e.g., features, techniques, etc.) in the project. Please explain in detail.)
1. Item
2. Item, and so forth

### Learned Knowledge from the Project

#### Major Challenges and Solutions:
1. AI/NavMesh integration
2. Building the game without any errors
3. Version control with Wwise and Unity using Git

#### Minor Challenges and Solutions:
1. Item
2. Item, and so forth

### Reflections on the Own Project:
(List down and explain what you could improve and add if you have more time.)
1. Item
2. Item, and so forth
