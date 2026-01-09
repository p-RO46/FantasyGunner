using UnityEngine;

public class AttackPrefab : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("적과 부딪혔다!");
        }
        if (collision.gameObject.CompareTag("Tile"))
        {
            Debug.Log("바닥에 부딪혔다!");
        }

        Destroy(gameObject);
    }
}
