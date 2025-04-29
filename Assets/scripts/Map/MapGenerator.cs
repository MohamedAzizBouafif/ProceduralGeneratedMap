using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //maps

    public int MapCount;

    //prefabs
    public  Transform NavMeshFloor;
    public  Transform NavMeshMask;
    public  Transform TilePrefab;
    public  Transform ObsticulPrefab;


    //holders
    [System.NonSerialized] public Transform mapHolder;
    [System.NonSerialized] public Transform obsticulHolder;
    protected Transform tileHolder;
    protected Transform NavmeshHolder;
    [System.NonSerialized] public string ObsticulHolderName = "Obsticul Holder";
    
    //map size and seed
    public int seed;
    public Coord MapSize;
    public float MapScale;

    //obsticull initialisation
    [Range(0,1)]
    public float outLinePercent;
    [Range(0, 1)]
    public  float ObsticulPercent;
    public float MinHeight;
    public float MaxHeight;
    
    public Vector2 MaxMapSize;
    private Coord mapCentre;



    [System.NonSerialized] public Vector3 toDestoryPosition;

  

    public void CreateObsticulHolder()
    {
        obsticulHolder = new GameObject(ObsticulHolderName).transform;
        obsticulHolder.parent = mapHolder;
        obsticulHolder.tag = ObsticulHolderName;
    }

    //initialisation
    public void Initialisation()
    {
        MeshMAskGEnerarot();
        mapCentre = new Coord(MapSize.x / 2, MapSize.y / 2);
        
    }

    //THE OBSTICUL GENERATO
    public void GenerateObsticul()
    {


        System.Random prng = new System.Random(seed);
        bool[,] obsticulMap = Generate2DArrayOfBoolsForObsticulCoord(prng.Next(), MapSize, ObsticulPercent);


        Color randomForwardColor = GenerateRandomColor();
        Color RandomBackgroundColor = GenerateRandomColor();

        obsticulHolder.parent = obsticulHolder;

        for (int x = 0; x < obsticulMap.GetLength(0); x++)
        {
            for (int y = 0; y < obsticulMap.GetLength(1); y++)
            {
                if (obsticulMap[x, y])
                {
                    //set scale
                    float obsticulHeight = Mathf.Lerp(MinHeight, MaxHeight, (float)prng.NextDouble());
                    //Debug.Log(obsticulHeight);
                    Vector3 obsticulScale = new Vector3((1 - outLinePercent) * MapScale, obsticulHeight, (1 - outLinePercent) * MapScale);

                    Vector3 OBSPosition = CoordToPosition(x, y) + (Vector3.up * obsticulScale.y / 2f);
                    Transform newObsticul = Instantiate(ObsticulPrefab, OBSPosition, Quaternion.identity) as Transform;
                    newObsticul.parent = obsticulHolder;
                    newObsticul.localScale = obsticulScale;

                    //obsticul color
                    float colorPercent = y / (float)MapSize.y;
                    Color obsticulColor = Color.Lerp(randomForwardColor, RandomBackgroundColor, colorPercent);

                    Renderer ObsticulRanderer = newObsticul.GetComponent<Renderer>();
                    Material tempMaterial = new Material(ObsticulRanderer.sharedMaterial);
                    tempMaterial.color = obsticulColor;
                    ObsticulRanderer.sharedMaterial = tempMaterial;
                }
            }
        }
    }

    public bool[,] Generate2DArrayOfBoolsForObsticulCoord(int seed, Coord newObsticulMapSize, float newObsticulobsticulPercent)
    {
        List<Coord> AllTileCoords = new List<Coord>();

        for (int x = 0; x < newObsticulMapSize.x; x++)
        {
            for (int y = 0; y < newObsticulMapSize.y; y++)
            {
                //Debug.Log(x + " + " + y);
                AllTileCoords.Add(new Coord(x, y));
            }
        }

        Queue<Coord> ShuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(AllTileCoords.ToArray(), seed));

        bool[,] obsticulMap = new bool[newObsticulMapSize.x, newObsticulMapSize.y];
        int obsticulCount = (int)(newObsticulMapSize.x * newObsticulMapSize.y * newObsticulobsticulPercent);

        int currentObsticulCount = 0;
        for (int i = 0; i < obsticulCount; i++)
        {
            Coord randomCoord = ShuffledTileCoords.Dequeue();
            ShuffledTileCoords.Enqueue(randomCoord);

            obsticulMap[randomCoord.x, randomCoord.y] = true;
            currentObsticulCount++;

            if (randomCoord != mapCentre && MapFullyAccessible(obsticulMap, currentObsticulCount))
            {
                continue;
            }
            else
            {
                obsticulMap[randomCoord.x, randomCoord.y] = false;
                currentObsticulCount--;
            }
        }

        return obsticulMap;
    }

    bool MapFullyAccessible(bool[,] obsticulMap, int currnetObsticulCount)
    {
        bool[,] mapFlags = new bool[obsticulMap.GetLength(0), obsticulMap.GetLength(1)];
        Queue<Coord> coordToCheck = new Queue<Coord>();
        coordToCheck.Enqueue(mapCentre);
        mapFlags[mapCentre.x, mapCentre.y] = true;
        int accessibleTiles = 1;

        while (coordToCheck.Count > 0)
        {
            Coord tile = coordToCheck.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;

                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obsticulMap.GetLength(0) && neighbourY >= 0 && neighbourY < obsticulMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obsticulMap[neighbourX, neighbourY])
                            {
                                coordToCheck.Enqueue(new Coord(neighbourX, neighbourY));
                                mapFlags[neighbourX, neighbourY] = true;
                                accessibleTiles++;
                            }
                        }
                    }
                }
            }
        }
        int targetObsticulCount = MapSize.x * MapSize.y - accessibleTiles;

        return currnetObsticulCount == targetObsticulCount;
    }

    //UTILITIS
    Color GenerateRandomColor()
    {
        // Generate a random color
        Color randomColor = new Color(
            Random.Range(0f, 1f),    // red component
            Random.Range(0f, 1f),    // green component
            Random.Range(0f, 1f),    // blue component
            1.0f                     // alpha component
        );

        return randomColor;
    }

     Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-MapSize.x / 2 + 0.5f + x, 0f, -MapSize.y / 2 + 0.5f + y) * MapScale;
    }

   

    //THE GROUND GENERATOR
    public void CreateHolders()
    {
        string HolderName = "Generated Map";

        if (transform.Find(HolderName))
        {
            GameObject.Destroy(transform.Find(HolderName).gameObject);
        }

        //creating holder

        mapHolder = new GameObject(HolderName).transform;
        mapHolder.parent = transform;

        string tileHolderName = "tile Holder";
        tileHolder = new GameObject(tileHolderName).transform;
        tileHolder.parent = mapHolder;

        string NavmeshHolderName = "NavMesh Holder";
        NavmeshHolder = new GameObject(NavmeshHolderName).transform;
        NavmeshHolder.parent = mapHolder;
    }

    public void MeshMAskGEnerarot()
    {
        Transform maskLeft = Instantiate(NavMeshMask, Vector3.left * (MapSize.x + MaxMapSize.x) / 4f * MapScale, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((MaxMapSize.x - MapSize.x) / 2f, 1, MapSize.y) * MapScale;

        Transform maskRight = Instantiate(NavMeshMask, Vector3.right * (MapSize.x + MaxMapSize.x) / 4f * MapScale, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((MaxMapSize.x - MapSize.x) / 2f, 1, MapSize.y) * MapScale;

        Transform maskTop = Instantiate(NavMeshMask, Vector3.forward * (MapSize.y + MaxMapSize.y) / 4f * MapScale, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(MaxMapSize.x, 1, (MaxMapSize.y - MapSize.y) / 2f) * MapScale;

        Transform maskBottom = Instantiate(NavMeshMask, Vector3.back * (MapSize.y + MaxMapSize.y) / 4f * MapScale, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(MaxMapSize.x, 1, (MaxMapSize.y - MapSize.y) / 2f) * MapScale;

        NavMeshFloor.localScale = new Vector3(MaxMapSize.x, MaxMapSize.y) * MapScale;
    }

    public void GenerateEmptyMap()
    {
        //instiate tile
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(TilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.parent = tileHolder;
                newTile.localScale = Vector3.one * (1 - outLinePercent) * MapScale;
            }
        }
    }
}
