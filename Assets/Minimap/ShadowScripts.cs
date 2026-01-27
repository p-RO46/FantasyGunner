using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;

public class ShadowScripts : MonoBehaviour
{
    public GameObject shadowPlane;
    public Transform player;
    public LayerMask shadowLayer;
    public float shadowRadius = 10f;
    private float radiuscircle { get { return shadowRadius * shadowRadius; } }

    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        StartCoroutine(UpdateVertices());
    }

    IEnumerator UpdateVertices()
    {
        while (true)
        {
            Ray r = new Ray(transform.position, player.position - transform.position);
            RaycastHit hit;

            if (Physics.Raycast(r, out hit, 1000, shadowLayer, QueryTriggerInteraction.Collide))
            {
                // 1. hit.point를 평면 기준의 로컬 좌표로 변환 (루프 밖에서 1번만 수행)
                Vector3 localHitPoint = shadowPlane.transform.InverseTransformPoint(hit.point);

                for (int i = 0; i < vertices.Length; i++)
                {
                    float dist = Vector3.Distance(vertices[i], localHitPoint);

                    
                    float effectIntensity = Mathf.Clamp01(dist / radiuscircle);

                    if(effectIntensity < 1) colors[i].a = 0f;
                    //colors[i].a = Mathf.Min(colors[i].a, effectIntensity);
                }

                UpdateColors();
            }

            yield return Util.WaitForSecond(0.05f);
        }
    }

    public void UpdateColors()
    {
        mesh.colors = colors;
    }

    // Initialize 내부에서 확실히 적용되었는지 확인
    void Initialize()
    {
        mesh = shadowPlane.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        colors = new Color[vertices.Length];
        //Debug.Log(vertices.Length);
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0f, 0f, 0f, 1f); // 255가 아닌 1.0f 사용
            //Debug.Log(i + " : " + colors[i]);
        }

        UpdateColors();
    }
}

