using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDestroyState : MapBaseState
{
    float FromEntreTime = 0f;
    Vector3 ToDesTroyPosition;

    public override void OnStartEntreState(MapManager mapManager, MapGenerator generator)
    {
      
    }

    public override void OnEntreState(MapManager mapManeger, MapGenerator generator)
    {
        ToDesTroyPosition = Vector3.down * (generator.MaxHeight + 0.5f);
    }

    public override void OnUpdateState(MapManager mapManeger, Vector2 Timers, MapGenerator generator)
    {

        if (FromEntreTime <= Timers.x)
        {
            FromEntreTime += Time.deltaTime;
            float t = FromEntreTime / Timers.x;
            Vector3 currentPosition = generator.obsticulHolder.position;
            generator.obsticulHolder.position = Vector3.Lerp(currentPosition, ToDesTroyPosition, t);
        }
        else
        {
            if (generator.mapHolder.Find(generator.ObsticulHolderName))
            {
                GameObject.Destroy(generator.mapHolder.Find(generator.ObsticulHolderName).gameObject);
            }

            FromEntreTime = 0f;
            mapManeger.SwitchState(mapManeger.onLoadState);
        }
    }
}
