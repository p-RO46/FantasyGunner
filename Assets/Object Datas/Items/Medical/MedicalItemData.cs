using UnityEngine;

[CreateAssetMenu(fileName = "MedicalItemData", menuName = "Scriptable Objects/ItemData/MedicalItemData")]
public class MedicalItemData : ItemData
{
    public int healAmount = 50;
    public override void Use()
    {
        PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerController.healHP(healAmount);
    }
}