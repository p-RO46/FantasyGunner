using UnityEngine;

public class AttackSpawner : MonoBehaviour
{
    private CharacterData monsterData;
    private GameObject attackPrefab;

    private void Start()
    {
        monsterData = GetComponentInParent<Monster>().monsterData;
        attackPrefab = monsterData.AttackObject;
    }

    public void SpawnAttack(Vector3 dest)
    {
        GameObject attack = Instantiate(attackPrefab, transform.position, Quaternion.identity);
        attack.GetComponent<AttackPrefab>().Setting(monsterData.Damage, dest);
    }
}
