using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerObject
{
    public Vector2 Position; // handles position of the walker
    public Vector2 Direction; // directions which ones to go
    public float ChanceToChange; //확률

    public TerrainData Type;

    public WalkerObject(Vector2 pos, Vector2 dir, float chanceToChange, TerrainData type)
    {
        Position = pos;
        Direction = dir;
        ChanceToChange = chanceToChange;
        Type = type;
    }
}
