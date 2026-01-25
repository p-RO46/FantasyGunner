using UnityEngine;

public class IconColorChanger : MonoBehaviour
{
    public GameObject IP;

    private bool colorChanged = false;

    void Update()
    {
        if (!colorChanged && IP.GetComponent<CaptureSpot>().activated)
        {
            GetComponent<MeshRenderer>().material.color = new Color(0.6f, 0f, 1f);
            colorChanged = true;
        }
    }

}
