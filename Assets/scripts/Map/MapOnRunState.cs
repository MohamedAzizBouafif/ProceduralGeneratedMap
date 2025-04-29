using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOnRunState : MapBaseState
{
    float FromEntreTime = 0f;
    public override void OnStartEntreState(MapManager mapManager, MapGenerator generator)
    {

    }

    public override void OnEntreState(MapManager mapManeger, MapGenerator generator)
    {
        
    }

    public override void OnUpdateState(MapManager mapManeger, Vector2 Timers, MapGenerator generator)
    {

        if (FromEntreTime <= Timers.y)
        {
            FromEntreTime += Time.deltaTime;
          
        }
        else
        {
            FromEntreTime = 0f;
            mapManeger.SwitchState(mapManeger.onDestroyState);
        }
    }
}
