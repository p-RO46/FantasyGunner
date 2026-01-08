using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerGenerator : MonoBehaviour
{
    //Variables
    public Dictionary<string, GameObject> Tiles;
    public Dictionary<string, TerrainData> FloorDatas;
    public TerrainData Water; //wall
    public TerrainData Empty; //empty

    public TerrainData[,] gridHandler; //x,y `positions
    public List<WalkerObject> Walkers; //List of custom walker class

    private int floorNumber;
    private Dictionary<string, TerrainData> TerrainDatas;

    public int MapWidth = 30;
    public int MapHeight = 30;

    public int MaximumWalkers = 10;
    public int TileCount = default;
    public float FillPercentage = 0.4f;
    public float WaitTime = 0.05f;
    public float weight = 1;

    private float TileWidth = 20;
    private float TileHeight = 20;

    public GameObject CS;

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        Tiles = new Dictionary<string, GameObject>();
        FloorDatas = new Dictionary<string, TerrainData>();

        MapDatas mapDatas = GetComponent<MapDatas>();
        for(int i = 0; i < mapDatas.Tiles.Count; i++)
        {
            GameObject t = mapDatas.Tiles[i];
            Tiles.Add(t.GetComponent<TerrainSetting>().terrainData.Type, t);
        }
        for(int i = 0; i < mapDatas.Terrains.Count; i++)
        {
            TerrainData t = mapDatas.Terrains[i];
            FloorDatas.Add(t.Type, t);
        }

        floorNumber = FloorDatas.Count;
        gridHandler = new TerrainData[MapWidth, MapHeight];
        TerrainDatas = new Dictionary<string, TerrainData>();
        foreach(KeyValuePair<string, TerrainData> item in FloorDatas)
        {
            TerrainDatas.Add(item.Key, item.Value);
        }
        TerrainDatas.Add("Water", Water);
        TerrainDatas.Add("Empty", Empty);

        for(int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)
            {
                gridHandler[x,y] = Empty;
            }
        }

        Walkers = new List<WalkerObject>();

        Vector3Int TileCenter = new Vector3Int(gridHandler.GetLength(0) / 2, gridHandler.GetLength(1) / 2, 0);

        WalkerObject curWalker = new WalkerObject(new Vector2(TileCenter.x, TileCenter.y), GetDirection(), 0.5f, GetTerrainType());

        gridHandler[TileCenter.x, TileCenter.y] = curWalker.Type;
        Walkers.Add(curWalker);

        TileCount++;

        StartCoroutine(CreateFloors());
    }

    Vector2 GetDirection()
    {
        int choice = Mathf.FloorToInt(UnityEngine.Random.value * 3.99f);

        switch (choice)
        {
            case 0:
            return Vector2.down;
            case 1:
            return Vector2.left;
            case 2:
            return Vector2.up;
            case 3:
            return Vector2.right;
            default:
            return Vector2.zero;
        }
    }

    TerrainData GetTerrainType()
    {
        int choice = UnityEngine.Random.Range(0, floorNumber);

        int iteration = 0;
        foreach(TerrainData T in FloorDatas.Values)
        {
            if(choice == iteration) return T;
            iteration++;
        }
        return Empty;
    }

    TerrainData GetTerrainType(WalkerObject pre)
    {
        Dictionary<string, TerrainData> WeightedRandom = new Dictionary<string, TerrainData>();
        foreach(KeyValuePair<string, TerrainData> item in FloorDatas)
        {
            WeightedRandom.Add(item.Key, item.Value);
        }
        for(int i = 0; i < weight; i++)
        {
            WeightedRandom.Add(i.ToString(), pre.Type);
        }

        int choice = UnityEngine.Random.Range(0, WeightedRandom.Count);
        int iteration = 0;
        foreach(TerrainData T in WeightedRandom.Values)
        {
            if(choice == iteration) return T;
            iteration++;
        }
        return Empty;
    }

    IEnumerator CreateFloors()
    {
        while((float)TileCount / (float)gridHandler.Length < FillPercentage)
        {
            bool hasCreatedFloor = false;
            foreach(WalkerObject curWalker in Walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                if(!gridHandler[curPos.x, curPos.y].Walkable)
                {
                    gridHandler[curPos.x, curPos.y] = curWalker.Type;
                    TileCount++;
                    hasCreatedFloor = true;
                }
            }   
            //Walker Methods
            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            if (hasCreatedFloor)
            {
                yield return new WaitForSeconds(WaitTime);
            }
        }

        StartCoroutine(CreateWalls());
    }

    void ChanceToRemove()
    {
        int updatedCount = Walkers.Count;
        for(int i = 0; i < updatedCount; i++)
        {
            if(UnityEngine.Random.value < Walkers[i].ChanceToChange && Walkers.Count > 1)
            {
                Walkers.RemoveAt(i);
                break;
            }
        }
    }

    void ChanceToRedirect()
    {
        for(int i = 0; i<Walkers.Count; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChanceToChange)
            {
                WalkerObject curWalker = Walkers[i];
                curWalker.Direction = GetDirection();
                Walkers[i] = curWalker;
            }
        }
    }

    void ChanceToCreate()
    {
        int updatedCount = Walkers.Count;
        for(int i= 0; i<updatedCount; i++)
        {
            if(UnityEngine.Random.value < Walkers[i].ChanceToChange && Walkers.Count < MaximumWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newPosition = Walkers[i].Position;

                WalkerObject newWalker = new WalkerObject(newPosition, newDirection, 0.5f, GetTerrainType(Walkers[i]));
                Walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {
        for(int i = 0; i<Walkers.Count; i++)
        {
            WalkerObject FoundWalker = Walkers[i];
            FoundWalker.Position += FoundWalker.Direction;
            FoundWalker.Position.x = Mathf.Clamp(FoundWalker.Position.x, 1, gridHandler.GetLength(0) - 2);
            FoundWalker.Position.y = Mathf.Clamp(FoundWalker.Position.y, 1, gridHandler.GetLength(1) - 2);
            Walkers[i] = FoundWalker;
        }
    }

    IEnumerator CreateWalls()
    {
        for(int x = 0; x < gridHandler.GetLength(0) - 1; x++)
        {
            for(int y = 0; y < gridHandler.GetLength(1) - 1; y++)
            {
                if(gridHandler[x,y].Walkable)
                {
                    bool hasCreatedWall = false;

                    if(gridHandler[x+1, y].isEmpty)
                    {
                        gridHandler[x+1, y] = Water;
                        hasCreatedWall = true;
                    }
                    if(gridHandler[x-1, y].isEmpty)
                    {
                        gridHandler[x-1, y] = Water;
                        hasCreatedWall = true;
                    }
                    if(gridHandler[x,y+1].isEmpty)
                    {
                        gridHandler[x, y+1] = Water;
                        hasCreatedWall = true;
                    }
                    if(gridHandler[x,y-1].isEmpty)
                    {
                        gridHandler[x, y-1] = Water;
                        hasCreatedWall = true;
                    }

                    if (hasCreatedWall)
                    {
                        yield return new WaitForSeconds(WaitTime);
                    }
                }
            }
        }
        SetTiles();
    }

    void SetTiles()
    {
        for(int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for(int y = 0; y < gridHandler.GetLength(1); y++)
            {
                GameObject tile = Instantiate(Tiles[gridHandler[x, y].Type], 
                new Vector3(TileWidth * x - MapWidth * TileWidth / 2, 0 , TileHeight * y - MapHeight * TileHeight / 2), 
                Quaternion.identity, transform);

                if(gridHandler[x, y].Walkable && Random.Range(0, 30) == 0)
                { 
                    Instantiate(CS, tile.transform.position + CS.transform.position, Quaternion.identity);
                }
            }
        }
    }
}
