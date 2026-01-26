using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class WalkerGenerator : MonoBehaviour
{
    private TerrainManager manager;

    public GameObject minimapFog;

    //Variables
    public GameObject MapTile;
    public GameObject EmptyTile;

    public Dictionary<string, TerrainData> FloorDatas;
    public TerrainData Water; //wall
    public TerrainData Empty; //empty

    public MonsterManager monsterManager;

    public TerrainData[,] gridHandler; //x,y `positions
    public List<WalkerObject> Walkers; //List of custom walker class

    private int[,,] extendedArray;

    private int floorNumber;

    public Dictionary<string, TerrainData> TerrainDatas;

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

    private void Start()
    {
        manager = GetComponent<TerrainManager>();
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        FloorDatas = new Dictionary<string, TerrainData>();

        MapDatas mapDatas = GetComponent<MapDatas>();
        for (int i = 0; i < mapDatas.Terrains.Count; i++)
        {
            TerrainData t = mapDatas.Terrains[i];
            FloorDatas.Add(t.Type, t);
        }

        floorNumber = FloorDatas.Count;
        gridHandler = new TerrainData[MapWidth, MapHeight];
        TerrainDatas = new Dictionary<string, TerrainData>();
        foreach (KeyValuePair<string, TerrainData> item in FloorDatas)
        {
            TerrainDatas.Add(item.Key, item.Value);
        }
        TerrainDatas.Add("Water", Water);
        TerrainDatas.Add("Empty", Empty);

        for (int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)
            {
                gridHandler[x, y] = Empty;
            }
        }

        Walkers = new List<WalkerObject>();

        Vector3Int TileCenter = new Vector3Int(gridHandler.GetLength(0) / 2, gridHandler.GetLength(1) / 2, 0);

        WalkerObject curWalker = new WalkerObject(new Vector2(TileCenter.x, TileCenter.y), GetDirection(), 0.5f, GetTerrainType());

        gridHandler[TileCenter.x, TileCenter.y] = curWalker.Type;
        Walkers.Add(curWalker);

        TileCount++;

        StartCoroutine(CreateFloors());
        //CreateFloors();
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
        int choice = Random.Range(0, floorNumber);

        int iteration = 0;
        foreach (TerrainData T in FloorDatas.Values)
        {
            if (choice == iteration) return T;
            iteration++;
        }
        return Empty;
    }

    TerrainData GetTerrainType(WalkerObject pre)
    {
        Dictionary<string, TerrainData> WeightedRandom = new Dictionary<string, TerrainData>();
        foreach (KeyValuePair<string, TerrainData> item in FloorDatas)
        {
            WeightedRandom.Add(item.Key, item.Value);
        }
        for (int i = 0; i < weight; i++)
        {
            WeightedRandom.Add(i.ToString(), pre.Type);
        }

        int choice = UnityEngine.Random.Range(0, WeightedRandom.Count);
        int iteration = 0;
        foreach (TerrainData T in WeightedRandom.Values)
        {
            if (choice == iteration) return T;
            iteration++;
        }
        return Empty;
    }

    IEnumerator CreateFloors()
    {
        while ((float)TileCount / (float)gridHandler.Length < FillPercentage)
        {
            bool hasCreatedFloor = false;
            foreach (WalkerObject curWalker in Walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                if (!gridHandler[curPos.x, curPos.y].Walkable)
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
                yield return Util.WaitForSecond(WaitTime);
            }
        }

        StartCoroutine(CreateWalls());
        //CreateWalls();
    }

    void ChanceToRemove()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChanceToChange && Walkers.Count > 1)
            {
                Walkers.RemoveAt(i);
                break;
            }
        }
    }

    void ChanceToRedirect()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (Random.value < Walkers[i].ChanceToChange)
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
        for (int i = 0; i < updatedCount; i++)
        {
            if (Random.value < Walkers[i].ChanceToChange && Walkers.Count < MaximumWalkers)
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
        for (int i = 0; i < Walkers.Count; i++)
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
        for (int x = 0; x < gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1) - 1; y++)
            {
                if (gridHandler[x, y].Walkable)
                {
                    bool hasCreatedWalls = false;

                    if (gridHandler[x + 1, y].isEmpty)
                    {
                        gridHandler[x + 1, y] = Water;
                        hasCreatedWalls = true;
                    }
                    if (gridHandler[x - 1, y].isEmpty)
                    {
                        gridHandler[x - 1, y] = Water;
                        hasCreatedWalls = true;
                    }
                    if (gridHandler[x, y + 1].isEmpty)
                    {
                        gridHandler[x, y + 1] = Water;
                        hasCreatedWalls = true;
                    }
                    if (gridHandler[x, y - 1].isEmpty)
                    {
                        gridHandler[x, y - 1] = Water;
                        hasCreatedWalls = true;
                    }

                    if (gridHandler[x - 1, y - 1].isEmpty)
                    {
                        gridHandler[x - 1, y - 1] = Water;
                        hasCreatedWalls = true;
                    }
                    if (gridHandler[x - 1, y + 1].isEmpty)
                    {
                        gridHandler[x - 1, y + 1] = Water;
                        hasCreatedWalls = true;
                    }
                    if (gridHandler[x + 1, y - 1].isEmpty)
                    {
                        gridHandler[x + 1, y - 1] = Water;
                        hasCreatedWalls = true;
                    }
                    if (gridHandler[x + 1, y + 1].isEmpty)
                    {
                        gridHandler[x + 1, y + 1] = Water;
                        hasCreatedWalls = true;
                    }

                    if (hasCreatedWalls)
                    {
                        yield return Util.WaitForSecond(WaitTime);
                    }
                }
            }
        }

        SetTiles();
    }

    void SetTiles()
    {
        extendedArray = new int[MapWidth, MapHeight, 4];
        BlendingTileSetter();

        for (int x = 0; x < extendedArray.GetLength(0); x++)
        {
            for (int y = 0; y < extendedArray.GetLength(1); y++)
            {
                GameObject tile;
                if (gridHandler[x, y].Type.Equals("Empty"))
                {
                    tile = Instantiate(EmptyTile, new Vector3(TileWidth * x - MapWidth * TileWidth / 2, 0, TileHeight * y - MapHeight * TileHeight / 2),
                                        Quaternion.identity, transform);
                }
                else
                {
                    tile = Instantiate(MapTile, new Vector3(TileWidth * x - MapWidth * TileWidth / 2, 0, TileHeight * y - MapHeight * TileHeight / 2),
                                            Quaternion.identity, transform);

                    tile.GetComponent<TerrainSetting>().terrainData = gridHandler[x, y];
                    tile.GetComponent<TileTextureSetter>().SetColors(extendedArray[x, y, 1], extendedArray[x, y, 3], extendedArray[x, y, 0], extendedArray[x, y, 2]);

                    int k = 0;
                    foreach (Transform child in tile.transform)
                    {
                        if (!gridHandler[x, y].Type.Equals("Empty") && extendedArray[x, y, k++] == 3) // water
                        {
                            child.gameObject.SetActive(true);
                        }
                    }
                }
                manager.addTile(tile, x, y);

                if (gridHandler[x, y].Walkable && Random.Range(0, 30) == 0)
                {
                    Instantiate(CS, tile.transform.position + CS.transform.position, Quaternion.identity);
                    monsterManager.SpawnCS();
                }
            }
        }
        minimapFog.transform.localScale = new Vector3(TileWidth * MapWidth, 1, TileHeight * MapHeight);

        manager.SettingCompleted();
    }

    void BlendingTileSetter() // 아름다운 타일 세팅
    {
        // 한 칸을 4개로 나눔
        extendedArray = new int[MapWidth, MapHeight, 4];

        for (int i = 0; i < extendedArray.GetLength(0); i++)
        {
            for (int j = 0; j < extendedArray.GetLength(1); j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    extendedArray[i, j, k] = gridHandler[i, j].TileIndex;
                }
            }
        }
        
        // 타일 가짓수는 그렇게 상관이 없음?
        // 아직까진 인스턴스 생성 안함.
        for (int i = 0; i < extendedArray.GetLength(0); i++)
        {
            for (int j = 0; j < extendedArray.GetLength(1); j++)
            {
                //주변 tile을 검사...
                
                // 0
                if (j - 1 >= 0)
                { //1
                    extendedArray[i, j - 1, 1] = extendedArray[i, j, 0];
                }
                if (i - 1 >= 0)
                { //2
                    extendedArray[i - 1, j, 2] = extendedArray[i, j, 0];
                }
                if (i - 1 >= 0 && j - 1 >= 0)
                { //3
                    extendedArray[i - 1, j - 1, 3] = extendedArray[i, j, 0];
                }
                
                // 1
                if (j + 1 < extendedArray.GetLength(1))
                { //0
                    extendedArray[i, j + 1, 0] = extendedArray[i, j, 1];
                }
                
                if (i - 1 >= 0 && j + 1 < extendedArray.GetLength(1))
                { //2
                    extendedArray[i - 1, j + 1, 2] = extendedArray[i, j, 1];
                }
                if (i - 1 >= 0)
                { //3
                    extendedArray[i - 1, j, 3] = extendedArray[i, j, 1];
                }
                

                // 2
                if (i + 1 < extendedArray.GetLength(0))
                { //0
                    extendedArray[i + 1, j, 0] = extendedArray[i, j, 2];
                }
                
                if (i + 1 < extendedArray.GetLength(0) && j - 1 >= 0)
                { //1
                    extendedArray[i + 1, j - 1, 1] = extendedArray[i, j, 2];
                }
                if (j - 1 >= 0)
                { //3
                    extendedArray[i, j - 1, 3] = extendedArray[i, j, 2];
                }
                

                // 3
                if (i + 1 < extendedArray.GetLength(0) && j + 1 < extendedArray.GetLength(1))
                { //0
                    extendedArray[i + 1, j + 1, 0] = extendedArray[i, j, 3];
                }
                if (i + 1 < extendedArray.GetLength(0))
                { //1
                    extendedArray[i + 1, j, 1] = extendedArray[i, j, 3];
                }
                if (j + 1 < extendedArray.GetLength(1))
                { //2
                    extendedArray[i, j + 1, 2] = extendedArray[i, j, 3];
                }
            }
        }
        // 이제 확장판 배열을 기준으로 실제 타일을 배치해줄 거임
        // 그리고 아직 해결되지 않은 <흐르는 물>...
    }
}