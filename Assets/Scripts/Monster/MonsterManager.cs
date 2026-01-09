using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public List<GameObject> monsters;
    void Start()
    {
        monsters = new List<GameObject>();
    }

    public void SpawnMonster(GameObject m)
    {
        monsters.Add(m);
    }
}
