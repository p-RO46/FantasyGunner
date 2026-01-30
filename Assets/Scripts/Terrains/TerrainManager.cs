using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private GameManager gameManager;

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

    [SerializeField]
    private int CSCount = 5; //cs 개수는 정해놓자.
    private int obstacleLayer;
    private int obstacleLayer_forMonster;

    #endregion

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        CSCount = gameManager.stage * 4;

        obstacleLayer = LayerMask.GetMask("CS") & LayerMask.GetMask("Boxdetect");
        obstacleLayer_forMonster = obstacleLayer & LayerMask.GetMask("Obstacle");

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
        yield return Util.WaitForSecond(1f);
        LoadingSceneManager.OnSceneLoadedSignal();
        //Destroy(gameObject);
    }

    public void SetTerrainObejects()
    {
        for (int j = 0; j < 3; j++)
        {
            objectSpawnChance = 1;
            foreach (GameObject i in data.ObjectList1)
            {
                if (UnityEngine.Random.Range(0, objectSpawnChance++) == 0)
                {
                    float x = UnityEngine.Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.x * 5);
                    float z = UnityEngine.Random.Range(tileTransform.localScale.z * (-5), tileTransform.localScale.z * 5);
                    float y = 0;

                    for(int k = 0; k < 100; k++)
                    {
                        if(CanBuild(x, z))
                        {
                            float rotation = UnityEngine.Random.Range(0.0f, 180.0f);
                            Instantiate(i, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), objectParent);
                            break;
                        }
                        else
                        {
                            x = UnityEngine.Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.x * 5);
                            z = UnityEngine.Random.Range(tileTransform.localScale.z * (-5), tileTransform.localScale.z * 5);
                        }
                    }
                }
            }
        }

        objectSpawnChance = 2;
        foreach (GameObject i in data.ObjectList2)
        {
            if (UnityEngine.Random.Range(0, objectSpawnChance++) == 0)
            {
                float x = UnityEngine.Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.x * 5);
                float z = UnityEngine.Random.Range(tileTransform.localScale.z * (-5), tileTransform.localScale.z * 5);
                float y = 0;

                for (int k = 0; k < 100; k++)
                {
                    if (CanTree(x, z))
                    {
                        float rotation = UnityEngine.Random.Range(0.0f, 180.0f);
                        GameObject tree = Instantiate(i, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), objectParent);

                        float scale = UnityEngine.Random.Range(0.5f, 1.5f);
                        tree.transform.localScale = new Vector3(scale, scale, scale);
                        break;
                    }
                    else
                    {
                        x = UnityEngine.Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.x * 5);
                        z = UnityEngine.Random.Range(tileTransform.localScale.z * (-5), tileTransform.localScale.z * 5);
                    }
                }
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
            if (UnityEngine.Random.Range(0, monsterSpawnChance) == 0)
            {
                float x = UnityEngine.Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.x * 5);
                float z = UnityEngine.Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.z * 5);
                float y = 0;
                //if (data.Type.Equals("Lava")) { y = UnityEngine.Random.Range(15.0f, 20.0f); }
                //else { y = 0; }

                if (CanSpawnMonster(x, z))
                {
                    float rotation = UnityEngine.Random.Range(0.0f, 180.0f);
                    Instantiate(data.SpawnMonster, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), monsterParent);
                }
                else
                {
                    x = UnityEngine.Random.Range(tileTransform.localScale.x * (-5), tileTransform.localScale.x * 5);
                    z = UnityEngine.Random.Range(tileTransform.localScale.z * (-5), tileTransform.localScale.z * 5);
                }
                
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
        float RandomY = UnityEngine.Random.Range(0f, 360f);
        Quaternion yOnlyRot = Quaternion.Euler(0, RandomY, 0);

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
            int randomIndex = UnityEngine.Random.Range(i, allCoords.Count);
            Vector2Int temp = allCoords[i];
            allCoords[i] = allCoords[randomIndex];
            allCoords[randomIndex] = temp;
        }

        if(allCoords.Count < CSCount * 2)
        {
            CSCount = allCoords.Count / 2;
        }

        // 3. 앞의 10개만 사용하기
        for(int i = 0; i < CSCount; i++)
        {
            SpawnCS(allCoords[i].x, allCoords[i].y);
        }

        for (int i = CSCount; i < CSCount * 2; i++)
        {
            SpawnBoxes(allCoords[i].x, allCoords[i].y);
        }
    }

    bool CanBuild(float x, float y)
    {
        // 해당 위치에 'Obstacle' 레이어의 콜라이더가 있는지 확인
        if (!Physics.CheckSphere(new Vector3(x, 0, y), 5.0f, obstacleLayer))
        {
            return true;
        }
        return false;
    }

    bool CanTree(float x, float y)
    {
        // 해당 위치에 'Obstacle' 레이어의 콜라이더가 있는지 확인
        if (!Physics.CheckSphere(new Vector3(x, 0, y), 1.0f, obstacleLayer))
        {
            return true;
        }
        return false;
    }

    bool CanSpawnMonster(float x, float y)
    {
        // 해당 위치에 'Obstacle' 레이어의 콜라이더가 있는지 확인
        if (!Physics.CheckSphere(new Vector3(x, 0, y), 1.0f, obstacleLayer_forMonster))
        {
            return true;
        }
        return false;
    }
}
