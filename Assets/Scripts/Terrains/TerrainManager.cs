using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

public class TerrainManager : MonoBehaviour
{
    public GameObject player;
    public NavMeshSurface surface;

    public GameObject CS;
    public GameObject Box;

    private WalkerGenerator wg;
    private GameObject[,] mapTiles;
    private bool _settingComplete = false;

    #region Terrain Setting Variables

    public Transform objectParent;
    public Transform csParent;
    public Transform boxParent;

    public Transform golemParent;
    public Transform skeletonParent;
    public MonsterManager monsterManager;

    private TerrainData data;
    private Transform tileTransform;
    private int objectSpawnChance;
    private int monsterSpawnChance = 10;

    public int CSCount = 5; //cs 개수는 정해놓자.

    #endregion

    private void Start()
    {
        wg = GetComponent<WalkerGenerator>();
        mapTiles = new GameObject[wg.MapWidth, wg.MapHeight];
    }

    private void Update()
    {
        if (_settingComplete)
        {
            surface.BuildNavMesh();
            SpawnCSAndBox();

            foreach (GameObject t in mapTiles)
            {
                data = t.GetComponent<TerrainSetting>().terrainData;
                tileTransform = t.transform;
                SpawnMonsters();
                SetTerrainObejects();
            }

            monsterManager.MonsterActivate();
            StartCoroutine(WaitStart());
            _settingComplete = false;

            Debug.Log("모든 초기화 로직 완료!");
        }
    }
    public void addTile(GameObject tile, int x, int y)
    {
        mapTiles[x, y] = tile;
    }

    public void SettingCompleted()
    {
        _settingComplete = true;
    }

    IEnumerator WaitStart()
    {
        player.transform.position = new Vector3(0, 100, 0);
        yield return Util.WaitForSecond(1f);
        LoadingSceneManager.OnSceneLoadedSignal();
        //Destroy(gameObject);
    }

    public void SetTerrainObejects()
    {
        for (int j = 0; j < 3; j++)
        {
            objectSpawnChance = 2;
            foreach (GameObject i in data.ObjectList1)
            {
                if (Random.Range(0, objectSpawnChance++) == 0)
                {
                    float x = Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.x * 5);
                    float z = Random.Range(tileTransform.localScale.z * (-5), tileTransform.localScale.z * 5);
                    float y = 0;

                    float rotation = Random.Range(0.0f, 180.0f);
                    Instantiate(i, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), objectParent);
                }
            }
        }

        objectSpawnChance = 3;
        foreach (GameObject i in data.ObjectList2)
        {
            if (Random.Range(0, objectSpawnChance++) == 0)
            {
                float x = Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.x * 5);
                float z = Random.Range(tileTransform.localScale.z * (-5), tileTransform.localScale.z * 5);
                float y = 0;

                float rotation = Random.Range(0.0f, 180.0f);
                GameObject tree = Instantiate(i, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), objectParent);

                float scale = Random.Range(0.5f, 1.5f);
                tree.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    public void SpawnMonsters()
    {
        string type = data.Type;
        Transform monsterParent;

        // 나중에 열거형으로 바꾸자
        if (type.Equals("Golem"))
        {
            monsterParent = golemParent;
        }
        else //if (type.Equals("Skeleton"))
        {
            monsterParent = skeletonParent;
        }

        for (int i = 0; i < data.Difficulty; i++)
        {
            if (Random.Range(0, monsterSpawnChance) == 0)
            {
                float x = Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.x * 5);
                float z = Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.z * 5);
                float y;
                if (data.Type.Equals("Lava")) { y = Random.Range(15.0f, 20.0f); }
                else { y = 0; }

                float rotation = Random.Range(0.0f, 180.0f);
                Instantiate(data.SpawnMonster, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), monsterParent);
                
                monsterManager.SpawnMonster(data);
            }
        }
    }

    void SpawnCS(int x, int y)
    {
        Instantiate(CS, mapTiles[x, y].transform.position + CS.transform.position, Quaternion.identity, csParent);
        monsterManager.SpawnCS();
    }
    void SpawnBoxes(int x, int y)
    {
        float randomY = Random.Range(0f, 360f);
        Quaternion yOnlyRot = Quaternion.Euler(0, randomY, 0);

        Instantiate(Box, mapTiles[x, y].transform.position + Box.transform.position, yOnlyRot, boxParent);
    }

    void SpawnCSAndBox()
    {
        int row = mapTiles.GetLength(0);
        int col = mapTiles.GetLength(1);

        // 1. 모든 가능한 좌표를 리스트에 담기
        List<Vector2Int> allCoords = new List<Vector2Int>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                data = mapTiles[i, j].GetComponent<TerrainSetting>().terrainData;
                if (data.Walkable)
                {
                    allCoords.Add(new Vector2Int(i, j));
                }
            }
        }

        // 2. 피셔-예이츠 셔플(Fisher-Yates Shuffle)로 리스트 섞기
        for (int i = 0; i < allCoords.Count; i++)
        {
            int randomIndex = Random.Range(i, allCoords.Count);
            Vector2Int temp = allCoords[i];
            allCoords[i] = allCoords[randomIndex];
            allCoords[randomIndex] = temp;
        }

        if(allCoords.Count < CSCount * 2)
        {
            CSCount = allCoords.Count / 2;
        }

        // 3. 앞의 10개만 사용하기
        for (int i = 0; i < CSCount; i++)
        {
            SpawnCS(allCoords[i].x, allCoords[i].y);
        }

        for (int i = CSCount; i < CSCount * 2; i++)
        {
            SpawnBoxes(allCoords[i].x, allCoords[i].y);
        }
    }
}
