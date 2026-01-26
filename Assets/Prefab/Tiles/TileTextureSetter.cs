using UnityEngine;

public class TileTextureSetter : MonoBehaviour
{
    public int lb, rb, lt, rt; // 4개 영역의 텍스처 인덱스

    public float smoothness_1 = 0.7f; // Lava
    public float smoothness_2 = 0.89f; // Water
    public float smoothness_3 = 0.5f; // Grass
    public float smoothness_4 = 0.55f; // Ground

    void Start()
    {   
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];

        int vertexCount = mesh.vertexCount;
   
        // 모든 정점에 4개 타일의 인덱스 정보를 컬러 값으로 저장
        // 0~255 범위를 0~1로 변환하여 저장 (셰이더에서 다시 복구)
        Color tileIndices = new Color(lb / 255f, rb / 255f, lt / 255f, rt / 255f);

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = tileIndices;
        }

        mesh.colors = colors;

        // 1. Vector4 배열 생성
        Vector4[] customData = new Vector4[vertexCount];

        int[] v = { lb, rb, lt, rt };
        float[] sv = new float[4];

        for(int i = 0; i < 4; i++)
        {
            switch (v[i])
            {
                case 0:
                    sv[i] = smoothness_1; break;
                case 1:
                    sv[i] = smoothness_2; break;
                case 2:
                    sv[i] = smoothness_3; break;
                case 3:
                    sv[i] = smoothness_4; break;
                default:
                    sv[i] = 0.5f; break;
            }
        }

        // 2. 하나의 Vector4에 4개의 정보를 압축
        Vector4 sValues = new Vector4(sv[0], sv[1], sv[2], sv[3]);

        // 3. 모든 정점에 데이터 할당
        for (int i = 0; i < vertexCount; i++)
        {
            customData[i] = sValues;
        }

        // 4. uv2 채널에 Vector4 배열 대입 (유니티가 자동으로 인식함)
        mesh.SetUVs(1, customData);
        // 참고: SetUVs(1, ...)은 uv2를 의미합니다. (0이 uv1)
    }
}