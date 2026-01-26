using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Terrain", menuName = "Scriptable Objects/Terrain")]
[System.Serializable]
public class TerrainData : ScriptableObject
{
    [Header("Feature")]
    public string Type;
    public bool Walkable;
    public bool isEmpty;
    public float Difficulty;
    public Material TerrainMaterial;
    public GameObject SpawnMonster;
    public int TileIndex = 0; // tile texture¸¦ °áÁ¤Áþ´Â ÀÎµ¦½º. Grass:0 Ground:1 Lava:2 Water:3

    [Header("Objects")]
    public List<GameObject> ObjectList1;
    public List<GameObject> ObjectList2;
    public List<GameObject> ObjectList3;

    [Header("FootSteps")]
    public List<AudioClip> Walk;
    public List<AudioClip> Run;
    public List<AudioClip> JumpStart;
    public List<AudioClip> JumpLand;
}
