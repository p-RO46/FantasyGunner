using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // GameManager 오브젝트는 파괴되면 안된다
    // 게임 스테이지 난이도. 점점 상승
    public int stage = 1;

    public bool complete = false; // 게임 시작 준비
    public bool missionComplete = false; // 미션 완료 여부

    public string gameClear;
    public string gameComplete;

    public MissionManager missionManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 존재한다면 새로 생긴 녀석은 파괴함
            Destroy(gameObject);
        }
    }

    public void OnMissionSuccess()
    {
        if (!missionComplete)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            missionComplete = false;
            stage ++;

            if(stage >= 4)
            {
                //최종적으로 게임 끝.
                Destroy(gameObject);
                SceneManager.LoadScene(gameComplete);
            }
            else
            {
                SceneManager.LoadScene(gameClear);
            }
        }
    }

    public void MakeMissions()
    {
        missionManager.MakeMissions(stage);
    }
}
