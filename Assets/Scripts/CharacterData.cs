using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Stats")]
    public string characterName;
    public float damage;
    public float speed;
    public float maxHP;
}
