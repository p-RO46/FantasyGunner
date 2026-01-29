using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CameraChanger : MonoBehaviour
{
    public InputActionAsset ipa;
    public GameObject player;
    public Transform parent;

    private void Start()
    {
        StartCoroutine(playerGo());
    }

    IEnumerator playerGo()
    {
        ipa.FindActionMap("UI").Enable();
        ipa.FindActionMap("GamePlay").Disable();

        float time = 0;
        yield return Util.WaitForSecond(5f);

        while (time < 5)
        {
            yield return null;
            time += Time.deltaTime;
            player.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            player.transform.Translate(Vector3.forward * 20 * Time.deltaTime);
        }

        for (int i = 0; i < 3; i++)
        {
            parent.GetChild(i).gameObject.SetActive(true);
        }
        player.transform.position = new Vector3(0, 20, 0);

        ipa.FindActionMap("UI").Disable();
        ipa.FindActionMap("GamePlay").Enable();
        Destroy(gameObject);
    }
}