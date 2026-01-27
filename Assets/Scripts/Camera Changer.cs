using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraChanger : MonoBehaviour
{
    public GameObject player;
    public Transform parent;

    private void Start()
    {
        StartCoroutine(playerGo());
    }

    IEnumerator playerGo()
    {
        float time = 0;
        yield return Util.WaitForSecond(5f);


        while (time < 5)
        {
            yield return null;
            time += Time.deltaTime;
            player.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            player.transform.Translate(Vector3.forward * 10 * Time.deltaTime);
        }

        gameObject.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            parent.GetChild(i).gameObject.SetActive(true);
        }
        player.transform.position = new Vector3(0, 20, 0);
    }
}