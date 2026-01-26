using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoData", menuName = "Scriptable Objects/AmmoData")]
public class AmmoData : ScriptableObject
{
    public string ammoName;
    public int scrapPrice;
    public int count;
    public Sprite icon;
    public List<WeaponData> fittedGuns;
}
