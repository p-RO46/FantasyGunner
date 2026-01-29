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
        GameObject gameManager = GameObject.Find("GameManager");
        if(gameManager != null)
        {
            Destroy(gameManager);
        }
        SceneManager.LoadScene(startScene);
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }
}
