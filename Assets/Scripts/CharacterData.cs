using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Stats")]
    public string characterName;
    public string charcaterType;
    public float damage;
    public float speed;
    public float maxHP;
}
