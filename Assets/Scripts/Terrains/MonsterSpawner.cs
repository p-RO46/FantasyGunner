using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private TerrainData terrain;
    public int SpawnChance; //높을수록 확률 낮아짐
    //private int[,] coord;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        terrain = GetComponent<TerrainSetting>().terrainData;
        for(int i = 0; i < terrain.Difficulty; i++)
        {
            if (UnityEngine.Random.Range(0, SpawnChance) == 0)
            {
                float x = UnityEngine.Random.Range(0.0f, transform.localScale.x * 5);
                float z = UnityEngine.Random.Range(0.0f, transform.localScale.z * 5);
                //coord = new int[transform.Scale.x * 10, trasnform.Scale.z * 10];
                float y;
                if (terrain.Type.Equals("Lava")) { y = UnityEngine.Random.Range(15.0f, 20.0f); }
                else { y = 0; }
                Instantiate(terrain.SpawnMonster, new Vector3(x, y, z) + transform.position, Quaternion.identity);
            }
        }
    }
}
