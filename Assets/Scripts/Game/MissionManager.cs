using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
public class MissionManager : MonoBehaviour
{
    public Transform MissionList;
    public GameManager gm;
    public MonsterManager mm;
    public List<Mission> Missions;

    public GameObject missionPrefab;

    private bool missionCompleted = true;

    private void Start()
    {
        Missions = new List<Mission>();
    }

    public void MakeMissions(int count)
    {
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
            m.UpdateProgress(type);
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
        }
    }

    void UpdateMissionUI()
    {
        int index = 0;
        foreach(Transform child in MissionList)
        {
            TextMeshProUGUI progressText = child.GetChild(1).GetComponent<TextMeshProUGUI>();
            progressText.text = $"{Missions[index].progress} / {Missions[index].max}";
            
            if (Missions[index].isCompleted)
            {
                progressText.color = Color.green;
            }
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
}
