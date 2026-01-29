using UnityEngine;

public class MissionListStarted : MonoBehaviour
{
    void Start()
    {
        MissionManager missionManager = GameObject.Find("GameManager").GetComponent<MissionManager>();
        missionManager.Notice_MissionList(transform);
    }
}
