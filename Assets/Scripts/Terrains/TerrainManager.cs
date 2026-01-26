using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject player;
    public NavMeshSurface surface;

    private WalkerGenerator wg;
    private GameObject[,] mapTiles;
    private bool _settingComplete = false;

    #region Terrain Setting Variables

    public Transform mapObjectParent;
    public Transform monsterManagerTransform;

    private MonsterManager monsterManager;
    private TileTextureSetter textureSetter;
    private Transform tileTransform;
    private int objectSpawnChance;
    private int monsterSpawnChance = 40;

    private Dictionary<string, TerrainData> TerrainDatas;

    #endregion

    private void Start()
    {
        wg = GetComponent<WalkerGenerator>();
        mapTiles = new GameObject[wg.MapWidth, wg.MapHeight];

        monsterManager = monsterManagerTransform.gameObject.GetComponent<MonsterManager>();
    }

    private void Update()
    {
        if (_settingComplete)
        {
            surface.BuildNavMesh();
            TerrainDatas = GetComponent<WalkerGenerator>().TerrainDatas;

            foreach (GameObject t in mapTiles)
            {
                if (!t.GetComponent<TerrainSetting>().terrainData.Type.Equals("Empty"))
                {
                    textureSetter = t.GetComponent<TileTextureSetter>();
                    tileTransform = t.transform;

                    SpawnByRange();
                }
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
        player.transform.position = new Vector3(0, 10, 0);
        yield return Util.WaitForSecond(1f);
        LoadingSceneManager.OnSceneLoadedSignal();
        //Destroy(gameObject);
    }

    public void SetTerrainObejects(Vector2 range, TerrainData tData)
    {
        objectSpawnChance = 8;
        foreach (GameObject i in tData.ObjectList1)
        {
            if (Random.Range(0, objectSpawnChance++) == 0)
            {
                float x = Random.Range(0, range.x);
                float z = Random.Range(0, range.y);
                float y = 0;

                float rotation = Random.Range(0.0f, 180.0f);
                Instantiate(i, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), mapObjectParent);
            }
        }

        objectSpawnChance = 12;
        foreach (GameObject i in tData.ObjectList2)
        {
            if (Random.Range(0, objectSpawnChance++) == 0)
            {
                float x = Random.Range(0, range.x);
                float z = Random.Range(0, range.y);
                float y = 0;

                float rotation = Random.Range(0.0f, 180.0f);
                GameObject tree = Instantiate(i, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), mapObjectParent);

                float scale = Random.Range(0.5f, 1.5f);
                tree.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    public void SpawnMonsters(Vector2 range, TerrainData tData)
    {
        for (int i = 0; i < tData.Difficulty; i++)
        {
            if (Random.Range(0, monsterSpawnChance) == 0)
            {
                float x = Random.Range(0, range.x);
                float z = Random.Range(0, range.y);
                float y;
                if (tData.Type.Equals("Lava")) { y = Random.Range(15.0f, 20.0f); }
                else { y = 0; }

                float rotation = Random.Range(0.0f, 180.0f);
                Instantiate(tData.SpawnMonster, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), monsterManagerTransform);
                monsterManager.SpawnMonster();
            }
        }
    }

    void SpawnByRange()
    {
        int[] tex = { textureSetter.lt, textureSetter.rt, textureSetter.lb, textureSetter.rb }; 
        Vector2[] range = { new Vector2(5, 5), new Vector2(-5, 5), new Vector2(5, -5), new Vector2(-5, -5) };

        TerrainData tData;

        for(int i = 0; i < 4; i++)
        { 
            if (tex[i] != 3)
            {
                switch (tex[i])
                {
                    case 0:
                        tData = TerrainDatas["Grass"]; break;
                    case 1:
                        tData = TerrainDatas["Ground"]; break;
                    case 2:
                        tData = TerrainDatas["Lava"]; break;
                    default:
                        tData = TerrainDatas["Grass"]; break;
                }

                SetTerrainObejects(range[i], tData);
                SpawnMonsters(range[i], tData);
            }
        }
    }
}
