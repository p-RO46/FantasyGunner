using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterManager : MonoBehaviour
{
    public TextMeshProUGUI kill;
    public TextMeshProUGUI act;
    public List<GameObject> monsters;
    public int kills = 0;
    public int activated = 0;

    void Start()
    {
        monsters = new List<GameObject>();
        kill.text = "KILL : " + kills.ToString();
        act.text = "CS : " + activated.ToString();
    }

    public void SpawnMonster(GameObject m)
    {
        monsters.Add(m);
    }

    public void AddKills()
    {
        kills ++;
        kill.text = "KILL : " + kills.ToString();
    }

    public void AddActivation()
    {
        activated ++;
        act.text = "CS : " + activated.ToString();
    }
}
