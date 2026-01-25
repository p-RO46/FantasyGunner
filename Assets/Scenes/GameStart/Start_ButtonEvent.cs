using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_ButtonEvent : MonoBehaviour
{
    public string GameScene; //scene name
    public GameObject mainCanvas;
    public GameObject Manual;

    public void OnStartButtonClick()
    {
        mainCanvas.SetActive(false);
        LoadingSceneManager.Instance.LoadScene(GameScene);
    }

    public void OnExitButtonClick()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Debug.Log("∞‘¿” ¡æ∑·µ ");
    }
}
