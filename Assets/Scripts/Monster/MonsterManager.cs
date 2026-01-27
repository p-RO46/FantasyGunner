using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterManager : MonoBehaviour
{
    public List<GameObject> monsters;

    public PlayerInventory pi;
    public GameObject gameUI;

    private int kills = 0;
    private int activated = 0;
    private int monster = 0;
    private int cs = 0;

    private int remainMonster = 0;
    private int remainCS = 0;

    private bool complete = false;
    private bool gameStarted = false;

    public string gameClear;

    void Start()
    {
        monsters = new List<GameObject>();
    }

    private void Update()
    {
        if (gameStarted)
        {
            if (monster == kills && cs == activated)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                SceneManager.LoadScene(gameClear);
            }
        }
        else
        {
            if (complete)
            {
                gameStarted = true;

                remainMonster = monster - kills;
                remainCS = cs - activated;
            }
        }
    }

    public void SpawnMonster()
    {
        //monsters.Add(m);
        monster++;
    }
    public void SpawnCS()
    {
        cs++;
    }

    public void AddKills(GameObject m)
    {
        if(m != null)
        {
            int maxScraps = m.GetComponent<Monster>().monsterData.DropCredits;
            int scraps = Random.Range(maxScraps - 5, maxScraps);
            pi.AddScrap(scraps);

            Destroy(m);
            kills++;
            remainMonster = monster - kills;
        }
    }

    public void AddActivation()
    {
        activated++;
        remainCS = cs - activated;
    }

    public void MonsterActivate()
    {
        foreach (Transform child in transform)
        {
            MonsterMove m = child.gameObject.GetComponent<MonsterMove>();
            m.InitState();
            m.ToWalk();
        }
        complete = true;
    }
}
