using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ColorChanger : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    public GameObject IP;
    public Color targetColor;
    public float duration = 5f;
    private bool colorChanged = false;

    // Update is called once per frame
    void Start()
    {
        targetColor = new Color(0.6f, 0, 1);
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        if(!colorChanged && IP.GetComponent<CaptureSpot>().activated)
        {
            StartCoroutine(LerpColor());
            colorChanged = true;
        }
    }

    IEnumerator LerpColor()
    {
        Color StartColor = meshRenderer.material.color;
        float time = 0;

        while(time < duration)
        {
            float ratio = time / duration;
            meshRenderer.material.color = Color.Lerp(StartColor, targetColor, ratio);

            time += Time.deltaTime;
            yield return null;
        }

        meshRenderer.material.color = targetColor;
    }
}
