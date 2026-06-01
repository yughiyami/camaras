# Project Overview: Mapa_3_Escena3

- **Game Title**: Stupid Simulator - Mapa 3 (Retro Horror Maze)
- **High-Level Concept**: A retro-styled horror experience where the player (a deer) must navigate a procedurally generated maze to reach an exit portal while avoiding a predatory tiger.
- **Players**: Single player.
- **Inspiration**: Classic Doom (visuals and movement), modern horror "stealth" games.
- **Tone / Art Direction**: Low-poly, retro-pixelated (90s FPS aesthetic), dark and claustrophobic.
- **Render Pipeline**: URP (Universal Render Pipeline) with custom retro post-processing.

# Game Mechanics

## 1. Procedural Maze Generation (`MazeGenerator.cs`)
- **Algorithm**: Recursive Backtracking (Depth-First Search).
- **Process**:
    1. A logical grid of cells is created.
    2. The algorithm carves a path by visiting unvisited neighbors and removing "walls" between them.
    3. Physically, `wallPrefab` and `floorPrefab` are instantiated at the grid positions.
    4. **Colliders**: Walls use `MeshColliders` to prevent passage.
- **Execution**: Runs on `Start()` to ensure every playthrough has a different layout.

## 2. Character Controls and Physics
- **Doom Movement (`DoomMovement.cs`)**:
    - **Physics**: Uses a `CharacterController` component (capsule shape) for smooth movement and wall sliding.
    - **Logic**: WASD for movement. Includes "Camera Bobbing" using a sine wave (`Mathf.Sin`) on the camera's local Y position to simulate walking.
- **Possession System (`PossessionManager.cs`)**:
    - Allows the player to "jump" into different entities.
    - Uses a `Raycast` from the center of the screen to identify objects with a `Fighter` or `Possessable` component.
    - Swaps the active camera and input controller to the target entity.

## 3. Enemy AI (`DoomTigerAI.cs`)
- **Navigation**: Uses Unity's **NavMesh** system. The `Tiger_001` object has a `NavMeshAgent`.
- **States**:
    - **Patrol**: Moves to random points within a `patrolRadius`.
    - **Chase**: Triggers when the player is within `chaseThreshold`. Increases speed and moves directly toward the player's position.
    - **Caught**: If distance < `catchDistance`, it triggers a jumpscare (camera shake, red flash) and restarts the player's position.
- **Colliders**: The tiger uses a `CharacterController` for movement and a `SphereCollider` as a trigger for detection/attack range.

## 4. Special Ability: Deer Vision (`DeerVision.cs`)
- **Mechanism**: Pressing `Tab` toggles an overlay map.
- **Time Dilation**: Sets `Time.timeScale = 0.3f` while the map is open, creating a slow-motion tactical view.
- **UI Interaction**: Scales the minimap UI element to fill the screen and switches rendering to the `MapCamera`.

# Cameras and Rendering

## 1. Main Camera (FPS View)
- Attached to the player's head.
- Contains the `DeerVision` and `PossessionManager` scripts.
- Uses a `CinemachineBrain` to handle transitions between different views.

## 2. Cinemachine Virtual Cameras
- **ThirdPersonCamera**: Used when not in first-person mode or during possession.
- **Framing Transposer**: Follows the target's position.
- **Cinemachine Decollider**: Prevents the camera from clipping through the generated maze walls.

## 3. Map Camera
- Positioned high above the maze (Y = 40+).
- Set to an Orthographic projection or fixed-perspective to provide a clear view of the maze layout.
- Uses a `RenderTexture` for the minimap UI.

# UI and Feedback
- **DoomHUD**: Displays health, cooldowns, and the minimap.
- **Retro Volume**: A Global Volume (URP) that applies a pixelation/bit-crushing shader to the final render to match the 90s aesthetic.

# Key Asset & Context

- **Scripts**:
    - `MazeGenerator.cs`: Maze logic.
    - `DoomTigerAI.cs`: Enemy behavior.
    - `DoomMovement.cs`: Player movement + bobbing.
    - `DeerVision.cs`: Ability logic.
    - `PossessionManager.cs`: Entity switching.
- **Prefabs**:
    - `Deer_001`: Player model with CharacterController.
    - `Tiger_001`: Enemy model with NavMeshAgent.
    - `Wall_Retro`: Prefab used by the generator for maze boundaries.

# Implementation Steps (To modify or extend)

1. **Modify Maze Size**:
    - **Description**: Adjust `width` and `height` properties in the `Maze_Controller` GameObject.
    - **Role**: developer
    - **Dependencies**: None
2. **Adjust AI Difficulty**:
    - **Description**: Edit `DoomTigerAI` parameters like `chaseSpeed`, `detectionRange`, and `patrolRadius`.
    - **Role**: developer
    - **Dependencies**: None
3. **Change Visual Style**:
    - **Description**: Modify the `Retro Volume` profile settings to change pixel density or color grading.
    - **Role**: developer
    - **Dependencies**: None

# Verification & Testing
- **Generation Test**: Enter Play Mode and verify that the maze is correctly generated and that there are no "unreachable" areas.
- **AI Pathfinding Test**: Move the player to different corners of the maze and ensure the Tiger can find its way using the NavMesh.
- **Possession Test**: Aim at a different possessable object and click to ensure camera and controls switch correctly.
