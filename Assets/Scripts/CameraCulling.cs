using UnityEngine;
public class CameraCulling : MonoBehaviour
{
    Camera cam;

    public float Monster_distance;
    public float CS_distance;
    public float Tile_distance;
    public float TileObject_distance;

    void Start()
    {
        cam = GetComponent<Camera>();

        float[] distances = new float[32];

        distances[LayerMask.NameToLayer("Monster")] = Monster_distance;
        distances[LayerMask.NameToLayer("CS")] = CS_distance;
        distances[LayerMask.NameToLayer("Tile")] = Tile_distance;
        distances[LayerMask.NameToLayer("TileObject")] = TileObject_distance;

        cam.layerCullDistances = distances;
    }
}