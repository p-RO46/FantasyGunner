using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Mission
{
    // Mission 종류
    // 1. Activate All CS
    // 2. Kill {n} {Monster}
    // 3. Rescue All missing people 

    // Monster, CS 연동
    // 저지른 일이 있으니 MonsterManager에서 가져오도록 하자
    public MissionType type;
    public string text;
    public int max;
    public int progress;
    public bool isCompleted;

    private MissionManager manager;

    public Mission(MissionManager m, MissionType t, int max)
    {
        manager = m;

        type = t;
        text = type switch
        {
            MissionType.CS => $"Activate {max} CS",
            MissionType.GOLEM => $"Kill {max} Golems",
            MissionType.SKEL => $"Kill {max} Skeletons",
            MissionType.RESQUE => $"Rescue {max} missing",
            _ => " "
        };

        this.max = max;
        progress = 0;
        isCompleted = false;
    }

    public void UpdateProgress(MissionType t)
    {
        if(t == type)
        {
            progress++;
            if (progress >= max)
            {
                isCompleted = true;
                manager.Completed();
            }
        }
    }
}