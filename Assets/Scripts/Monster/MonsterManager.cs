using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterManager : MonoBehaviour
{
    public GameManager gameManager;
    public MissionManager missionManager;
    public PlayerInventory pi;
    public GameObject gameUI;

    public int golems = 0;
    public int skeletons = 0;
    public int cs = 0;
    public int resque = 0;

    public int kills = 0;
    public int activated = 0;

    public void SpawnMonster(TerrainData data)
    {
        if (data.Type.Equals("Ground"))
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
        gameManager.complete = true;
    }
}
