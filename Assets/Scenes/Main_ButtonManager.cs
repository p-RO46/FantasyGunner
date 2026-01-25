using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_ButtonManager : MonoBehaviour
{
    public string startScene;
    public string loadScene;

    public void OnRetryButtonClick()
    {
        LoadingSceneManager.Instance.LoadScene(loadScene);
    }

    public void OnMainButtonClick()
    {
        SceneManager.LoadScene(startScene);
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }
}
