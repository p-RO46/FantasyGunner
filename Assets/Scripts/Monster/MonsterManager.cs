using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterManager : MonoBehaviour
{
    private GameManager gameManager;
    private MissionManager missionManager;
    private PlayerInventory pi;

    public GameObject gameUI;

    public int golems = 0;
    public int skeletons = 0;
    public int cs = 0;
    public int resque = 0;

    public int kills = 0;
    public int activated = 0;
    private void Awake()
    {
        GameObject OB_gameManager = GameObject.Find("GameManager");
        gameManager = OB_gameManager.GetComponent<GameManager>();
        missionManager = OB_gameManager.GetComponent<MissionManager>();
        pi = OB_gameManager.transform.Find("SupplyManager").GetComponent<PlayerInventory>();
    }

    private void Start()
    {
        missionManager.Notice_MonsterManager(this);
    }

    public void SpawnMonster(TerrainData data)
    {
        if (data.Type.Equals("Grass"))
        {
            golems++;
        }
        else //if(data.CharacterName.Equals("Skeleton"))
        {
            skeletons++;
        }
    }

    public void SpawnCS()
    {
        cs++;
    }

    public void AddKills(GameObject m)
    {
        if(m != null)
        {
            CharacterData data = m.GetComponent<Monster>().monsterData;
            int maxScraps = data.DropCredits;
            int scraps = Random.Range(maxScraps - 5, maxScraps);
            pi.AddScrap(scraps);

            if (data.CharacterName.Equals("Golem"))
            {
                missionManager.UpdateProgresses(MissionType.GOLEM);
            }
            else if (data.CharacterName.Equals("Skeleton"))
            {
                missionManager.UpdateProgresses(MissionType.SKEL);
            }

            Destroy(m);
            kills++;
        }
    }

    public void AddActivation()
    {
        activated++;
        missionManager.UpdateProgresses(MissionType.CS);
    }

    public void MonsterActivate()
    {
        foreach (Transform child in transform)
        {
            foreach(Transform monster in child)
            {
                MonsterMove m = monster.gameObject.GetComponent<MonsterMove>();
                m.InitState();
                m.ToWalk();
            }
        }
        gameManager.MakeMissions();
    }
}
