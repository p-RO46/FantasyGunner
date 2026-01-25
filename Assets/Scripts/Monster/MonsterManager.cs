using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterManager : MonoBehaviour
{
    public TextMeshProUGUI kill;
    public TextMeshProUGUI act;
    public List<GameObject> monsters;

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

                kill.text = "REMAIN MONSTERS : " + remainMonster.ToString();
                act.text = "REMAIN CS : " + remainCS.ToString();
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
            Destroy(m);

            kills++;
            remainMonster = monster - kills;
            kill.text = "REMAIN MONSTERS : " + remainMonster;
        }
    }

    public void AddActivation()
    {
        activated++;
        remainCS = cs - activated;
        act.text = "REMAIN CS : " + remainCS;
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
