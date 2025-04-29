using UnityEngine;

public abstract class MapBaseState : MonoBehaviour
{
    protected MapGenerator generator;

    public abstract void OnStartEntreState(MapManager mapManager, MapGenerator generator);

    public abstract void OnEntreState(MapManager mapManeger, MapGenerator generator);

    public abstract void OnUpdateState(MapManager mapManeger, Vector2 Timers, MapGenerator generator);

 
}
