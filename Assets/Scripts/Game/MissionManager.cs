using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MissionManager : MonoBehaviour
{
    public Transform MissionList;
    private MonsterManager mm;

    public GameManager gm;
    public List<Mission> Missions;
    public List<Transform> Mission_Prefabs;
    public GameObject missionPrefab;

    private bool missionCompleted = true;
    private void Start()
    {
        MissionList = GameObject.Find("GameUIParent")?.transform.Find("Mission UI").Find("Mission UI").Find("Mission_List");
        mm = GameObject.Find("MonsterManager")?.GetComponent<MonsterManager>();
    }

    public void MakeMissions(int count)
    {
        Missions = new List<Mission>();
        Mission_Prefabs = new List<Transform>();

        int[] counts = { mm.cs, mm.golems, mm.skeletons, mm.resque };

        for (int i = 0; i < count; i++)
        {
            Mission m = new Mission(this, (MissionType)i, counts[i]);
            Missions.Add(m);
        }

        InitMissionUI();
    }
    public void UpdateProgresses(MissionType type)
    {
        foreach(Mission m in Missions)
        {
            if(m != null)
            {
                m.UpdateProgress(type);
            }
        }

        UpdateMissionUI();
    }

    void InitMissionUI()
    {
        for(int i = 0; i< Missions.Count; i++)
        {
            Transform mission = Instantiate(missionPrefab, MissionList).transform;
            mission.GetChild(0).GetComponent<TextMeshProUGUI>().text = Missions[i].text;
            mission.GetChild(1).GetComponent<TextMeshProUGUI>().text
                = $"{Missions[i].progress} / {Missions[i].max}";

            Mission_Prefabs.Add(mission);
        }
    }

    void UpdateMissionUI()
    {
        int index = 0;
        foreach(Transform child in Mission_Prefabs)
        {
            if(child != null)
            {
                TextMeshProUGUI progressText = child.GetChild(1).GetComponent<TextMeshProUGUI>();

                if (progressText == null)
                {
                    Debug.Log("progressText가 없습니다!");
                    return;
                }
                progressText.text = $"{Missions[index].progress} / {Missions[index].max}";

                if (Missions[index].isCompleted)
                {
                    progressText.color = Color.green;
                }
            }
            index++;
        }
    }

    public void Completed()
    {
        foreach (Mission m in Missions)
        {
            missionCompleted = missionCompleted && m.isCompleted;
        }

        if (missionCompleted)
        {
            gm.OnMissionSuccess();
        }
        else
        {
            missionCompleted = true;
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "FantasyGunner") // 실제 인게임 씬 이름
        {
            ReInitialize();
        }
    }
    void ReInitialize()
    {
        // 1. 새로운 씬에서 UI와 매니저 다시 찾기
        GameObject uiParent = GameObject.Find("GameUIParent");
        if (uiParent != null)
        {
            MissionList = uiParent.transform.Find("Mission UI/Mission UI/Mission_List");
        }

        mm = GameObject.Find("MonsterManager")?.GetComponent<MonsterManager>();
        gm = GetComponent<GameManager>();

        // 2. 리스트와 실제 게임 오브젝트 청소 (가장 중요)
        if (Mission_Prefabs != null)
        {
            foreach (Transform prefab in Mission_Prefabs)
            {
                if (prefab != null) Destroy(prefab.gameObject);
            }
            Mission_Prefabs.Clear();
        }

        if (Missions != null) Missions.Clear();
    }

    #region Notice
    public void Notice_MissionList(Transform missionList)
    {
        MissionList = missionList;
    }
    public void Notice_MonsterManager(MonsterManager monsterManager)
    {
        mm = monsterManager;
    }
    #endregion
}
