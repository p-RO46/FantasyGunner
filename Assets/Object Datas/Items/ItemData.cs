using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public abstract class ItemData : ScriptableObject
{
    [Header("Item Properties")]
    public string itemName;
    public int scrapPrice;
    public Sprite icon;

    public abstract void Use();
}