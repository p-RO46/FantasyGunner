using UnityEngine;

public class TerrainSetting : MonoBehaviour
{
    private Transform MonsterManager;
    private Transform MapObjectParent; //맵에 배치되는 오브젝트들의 부모
    public TerrainData terrainData;

    private int ObjectSpawnChance;
    private int MonsterSpawnChance = 10;

    void Start()
    {
        MonsterManager = GameObject.FindWithTag("MonsterManager").transform;
        MapObjectParent = GameObject.FindWithTag("MapObjectParent").transform;
        SetTerrainObejects();
        SpawnMonsters();
    }

    public void SetTerrainObejects()
    {
        for(int j = 0; j < 3; j++)
        {
            ObjectSpawnChance = 2;
            foreach(GameObject i in terrainData.ObjectList1)
            {
                if (UnityEngine.Random.Range(0, ObjectSpawnChance ++) == 0)
                {
                    float x = UnityEngine.Random.Range(transform.localScale.x * (-5), transform.localScale.x * 5);
                    float z = UnityEngine.Random.Range(transform.localScale.z * (-5), transform.localScale.z * 5);
                    //coord = new int[transform.Scale.x * 10, trasnform.Scale.z * 10];
                    float y = 0;

                    float rotation = UnityEngine.Random.Range(0.0f, 180.0f);
                    Instantiate(i, new Vector3(x, y, z) + transform.position, Quaternion.Euler(0, rotation, 0), MapObjectParent);
                }
            }
        }

        ObjectSpawnChance = 3;
        foreach(GameObject i in terrainData.ObjectList2)
        {
            if (UnityEngine.Random.Range(0, ObjectSpawnChance ++) == 0)
            {
                float x = UnityEngine.Random.Range(transform.localScale.x * (-5), transform.localScale.x * 5);
                float z = UnityEngine.Random.Range(transform.localScale.z * (-5), transform.localScale.z * 5);
                //coord = new int[transform.Scale.x * 10, trasnform.Scale.z * 10];
                float y = 0;

                float rotation = UnityEngine.Random.Range(0.0f, 180.0f);
                GameObject tree = Instantiate(i, new Vector3(x, y, z) + transform.position, Quaternion.Euler(0, rotation, 0), MapObjectParent);
                
                float scale = UnityEngine.Random.Range(0.5f, 1.5f);
                tree.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    public void SpawnMonsters()
    {
        for(int i = 0; i < terrainData.Difficulty; i++)
        {
            if (UnityEngine.Random.Range(0, MonsterSpawnChance) == 0)
            {
                float x = UnityEngine.Random.Range(transform.localScale.x * (-5), transform.localScale.x * 5);
                float z = UnityEngine.Random.Range(transform.localScale.x * (-5), transform.localScale.z * 5);
                //coord = new int[transform.Scale.x * 10, trasnform.Scale.z * 10];
                float y;
                if (terrainData.Type.Equals("Lava")) { y = UnityEngine.Random.Range(15.0f, 20.0f); }
                else { y = 0; }

                float rotation = UnityEngine.Random.Range(0.0f, 180.0f);
                Instantiate(terrainData.SpawnMonster, new Vector3(x, y, z) + transform.position, Quaternion.Euler(0, rotation, 0), MonsterManager);
            }
        }
    }
}
