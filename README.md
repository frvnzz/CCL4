# CCL4 - Group 8 Portfolio

Not final

### How to Run the Program (User Guideline)
1. **Download and Install Unity 6**  
   Ensure you have Unity installed on your system. The project was developed using Unity version `6000.0.50f1`.  

2. **Clone the Repository**  
   Clone the project repository to your local machine with:
   ```bash
   https://github.com/frvnzz/CCL4.git
   ```

   Alternatively, you can download the repository as a .zip file from 
   [here](https://github.com/frvnzz/CCL4/archive/refs/heads/main.zip)  

3. **Open the Project**  
   Open Unity Hub, add the cloned repository folder, and open the project.  

4. **Import Third-Party Assets**  
   This project uses third-party assets that are not included in the repository due to license restrictions.
   Please refer to [ThirdPartyAssets.md](./ThirdPartyAssets.md) for a list of the required assets and download instructions. 

5. **Run the Scene**  

### Mockup
The test scene shows a player capsule that is followed by the enemy capsule that shoots out a raycast for the player hit. The player will be followed around by the enemy and the player needs to fight off the waves of enemies, which are displayed in the UI.
![Mockup](./img/mockup.png)

### Feature Description
- Explore the map by running, sprinting and jumping
- Defeat Enemy Waves
- Use various weapons with different stats
- Reload your weapons and pick up ammo crates once you run out of bullets
- Enemies have idle, walking, and attack animations and will always navigate to the player using NavMesh.  

### System Design
#### Player:
- Shoots enemies with weapons.
- Deals damage to enemies when hitting them with the weapons. Different weapons have different settings for the raycast.
- Takes damage from enemies within range.
- Increases score by shooting enemies.

#### Spawn Controller:
- Once the player has defeated all enemies the next wave will start.
- The waves are defined beforehand with the enemies stored in an array, to select different types of enemies, and the delay between spawns for each wave.
- The Controller stores locations for the spawn points of the enemies.
- One spawn location is randomly picked from all locations.
- The wave text is updated.

#### AI Controller:
- Navigates to the player with obstacle avoidance.
- Deals damage to the player when in range.
- Takes damage from the players weapon.

#### Environment:
- Explosive barrels scattered around the level for damage to multiple enemies.

### System infrastructure
#### Class diagram (NOT FINISHED, still needs to be consolidated - still a lot of test scripts that will not be used in the final version):
![Class diagram (NOT FINISHED, still needs to be consolidated)](./img/include.png)