using UnityEngine;

public class TileTextureSetter : MonoBehaviour
{
    public int lt, rt, lb, rb; // 4개 영역의 텍스처 인덱스

    private static float lava_smooth = 0.6f; 
    private static float water_smooth = 0.89f; // 
    private static float grass_smooth = 0.5f; // 
    private static float ground_smooth = 0.51f; // 

    public void SetColors(int _lt, int _rt, int _lb, int _rb)
    {
        lt = _lt; rt = _rt; lb = _lb; rb = _rb;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];

        int vertexCount = mesh.vertexCount;
   
        // 모든 정점에 4개 타일의 인덱스 정보를 컬러 값으로 저장
        // 0~255 범위를 0~1로 변환하여 저장 (셰이더에서 다시 복구)
        Color tileIndices = new Color(rb / 255f, lb / 255f, rt / 255f, lt / 255f);

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = tileIndices;
        }

        mesh.colors = colors;

        // 1. Vector4 배열 생성
        Vector4[] customData = new Vector4[vertexCount];

        int[] v = { rt, lt, rb, lb };
        float[] sv = new float[4];

        for(int i = 0; i < 4; i++)
        {
            switch (v[i])
            {
                case 0:
                    sv[i] = grass_smooth; break; //lava?
                case 1:
                    sv[i] = ground_smooth; break; //water?
                case 2:
                    sv[i] = lava_smooth; break; //grass?
                case 3:
                    sv[i] = water_smooth; break; //ground?
                default:
                    sv[i] = 0.5f; break;
            }
        }

        // 2. 하나의 Vector4에 4개의 정보를 압축
        Vector4 sValues = new Vector4(sv[0] / 255, sv[1] / 255, sv[2] / 255, sv[3] / 255);
        for (int i = 0; i < vertexCount; i++)

        // 3. 모든 정점에 데이터 할당
        {
            customData[i] = sValues;
        }

        // 4. uv2 채널에 Vector4 배열 대입 (유니티가 자동으로 인식함)
        mesh.SetUVs(1, customData);
        // 참고: SetUVs(1, ...)은 uv2를 의미합니다. (0이 uv1)

        mesh.UploadMeshData(false);
    }
}