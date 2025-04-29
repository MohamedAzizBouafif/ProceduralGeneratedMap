# 🗺️ Procedural Map Generator - Unity Project 🎮

## 🌟 Overview
A state-based procedural map generator with obstacle placement and smooth transitions. Perfect for endless runner or dungeon crawler games!

## 🚀 Features
- 🔄 Three-state system (Load → Run → Destroy)
- 🏗️ Customizable map generation
- ⛰️ Smart obstacle placement with path validation
- ✨ Smooth animations between states

## 🏗️ Project Structure
```plaintext
Assets/
├── Scripts/
│   ├── MapManager.cs          # 🎛️ Main controller
│   ├── MapGenerator.cs        # 🏗️ Generation logic
│   ├── States/
│   │   ├── MapBaseState.cs    # 📜 Abstract state
│   │   ├── MapOnLoadState.cs  # ⬇️ Loading phase
│   │   ├── MapOnRunState.cs   # ▶️ Active phase
│   │   └── MapDestroyState.cs # 💥 Cleanup phase
│   └── Utility.cs            # 🛠️ Helper functions
└── Prefabs/                  # 🧩 Tile and obstacle prefabs
```

# 💻 Code Highlights

## 🏗️ Map Generation Core
```csharp
// 📐 Convert grid coordinates to world position
Vector3 CoordToPosition(int x, int y) {
    return new Vector3(
        -MapSize.x/2 + 0.5f + x, 
        0f, 
        -MapSize.y/2 + 0.5f + y
    ) * MapScale;
}

// 🎲 Create obstacle height with randomness

float obsticulHeight = Mathf.Lerp(
    MinHeight, 
    MaxHeight, 
    (float)prng.NextDouble()
);
```
## 🔍 Accessibility Check (Flood Fill)
```csharp
bool MapFullyAccessible(bool[,] obsticulMap, int currentObsticulCount)
{
    bool[,] mapFlags = new bool[obsticulMap.GetLength(0), obsticulMap.GetLength(1)];
    Queue<Coord> coordToCheck = new Queue<Coord>();
    coordToCheck.Enqueue(mapCentre);
    mapFlags[mapCentre.x, mapCentre.y] = true;
    
    // ... flood fill implementation ...
    return accessibleTiles == expectedCount;
}
```


## 🔄 State Machine Flow
```csharp
// ⏱️ State transition in MapManager
void Update() {
    currentState.OnUpdateState(this, Timer, generator);
}

// 🔄 Switching states
public void SwitchState(MapBaseState state) {
    currentState = state;
    currentState.OnEntreState(this, generator);
}
```

## 🛠️ Setup Guide
Attach Components:

```csharp
[RequireComponent(typeof(MapGenerator))]
public class MapManager : MonoBehaviour
{
    // 🔄 State references
    public MapDestroyState onDestroyState = new MapDestroyState();
    public MapOnLoadState onLoadState = new MapOnLoadState();
    public MapOnRunState onRunState = new MapOnRunState();
}
```
Configure Parameters:

```csharp
public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int seed = 12345;
    public Coord MapSize = new Coord(10, 10);
    
    [Header("Obstacle Settings")]
    [Range(0,1)] public float ObsticulPercent = 0.3f;
    public float MinHeight = 0.5f;
    public float MaxHeight = 3f;
}
```

## ⚙️ Usage Example
```csharp
// 🕒 Configure state durations
public class MapManager : MonoBehaviour
{
    [Tooltip("Loading animation duration")]
    public float loadTime = 2f;  
    
    [Tooltip("Time before destruction starts")]
    public float runTime = 5f;    
}
```
📦 Dependencies
Unity 2019.4+ (LTS recommended)

Basic C# knowledge

Understanding of state machines

🌈 Visual Effects
Obstacles fall from sky during load ⬇️☁️

Smooth sinking during destruction ⬇️🌋

Color gradients across the map 🌈

🤝 Contributing
Feel free to fork and submit PRs for:

🧩 More obstacle types

🎨 Better visual effects

⚡ Performance optimizations

📜 License
MIT License - Free for personal and commercial use


This version includes:
1. 🎯 All content in one file
2. 🎨 Relevant emojis for visual organization
3. 🏗️ Complete code examples
4. 📊 Clear structure with sections
5. 🛠️ Practical setup instructions
6. 🌈 Visual effect descriptions
7. 🤝 Contribution guidelines

The emojis help visually categorize different sections while maintaining professional documentation standards. Each major component has its own visual indicator for quick scanning.
