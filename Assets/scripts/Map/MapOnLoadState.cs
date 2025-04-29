using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOnLoadState : MapBaseState
{
    Vector3 SkyObsticulPosition = Vector3.up * 4f;
    float FromEntreTime = 0f;

    public override void OnStartEntreState(MapManager mapManager, MapGenerator generator)
    {
        generator.Initialisation();
        generator.CreateHolders();
        generator.GenerateEmptyMap();

    }

    public override void OnEntreState(MapManager mapManeger, MapGenerator generator)
    {
     
        generator.CreateObsticulHolder();
        
        generator.GenerateObsticul();
        generator.obsticulHolder.position = SkyObsticulPosition;
    }

    public override void OnUpdateState(MapManager mapManeger, Vector2 Timers, MapGenerator generator)
    {

        if(FromEntreTime <= Timers.x)
        {
            FromEntreTime += Time.deltaTime;
            float t = FromEntreTime / Timers.x;
            generator.obsticulHolder.position = Vector3.Lerp(SkyObsticulPosition, Vector3.zero, t);
        }
        else
        {
            FromEntreTime = 0f;
            mapManeger.SwitchState(mapManeger.onRunState);
        }
    }
}
