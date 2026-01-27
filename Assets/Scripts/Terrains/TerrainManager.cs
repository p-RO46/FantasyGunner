using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

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
    private TerrainData data;
    private Transform tileTransform;
    private int objectSpawnChance;
    private int monsterSpawnChance = 10;

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
                    Instantiate(i, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), mapObjectParent);
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
                GameObject tree = Instantiate(i, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), mapObjectParent);

                float scale = Random.Range(0.5f, 1.5f);
                tree.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    public void SpawnMonsters()
    {
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
                Instantiate(data.SpawnMonster, new Vector3(x, y, z) + tileTransform.position, Quaternion.Euler(0, rotation, 0), monsterManagerTransform);
                monsterManager.SpawnMonster();
            }
        }
    }
}
