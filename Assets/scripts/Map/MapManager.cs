using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    MapBaseState currentState;
    [System.NonSerialized] public MapDestroyState onDestroyState = new MapDestroyState();
    [System.NonSerialized] public MapOnLoadState onLoadState = new MapOnLoadState();
    [System.NonSerialized] public MapOnRunState onRunState = new MapOnRunState();
    MapGenerator generator;
    public float loatdTime;
    public float RunTime;

    Vector2 Timer;

    private void Start()
    {
        Timer = new Vector2(loatdTime, RunTime);
        generator = GetComponent<MapGenerator>();
        currentState = onLoadState;
        currentState.OnStartEntreState(this, generator);
        currentState.OnEntreState(this, generator);
    }

    private void Update()
    {
        currentState.OnUpdateState(this, Timer, generator);
    }

    public void SwitchState(MapBaseState state)
    {
        currentState = state;
        currentState.OnEntreState(this, generator);
    }
}
