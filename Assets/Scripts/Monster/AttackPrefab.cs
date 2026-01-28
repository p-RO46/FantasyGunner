using System.Collections;
using UnityEngine;

public class AttackPrefab : MonoBehaviour
{
    private Vector3 destination = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private int damage;
    private Rigidbody rigid;

    public int speed = 10; //예비용

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    IEnumerator Firing(Vector3 moveOffset)
    {
        while (transform.position.y > -10)
        {
            yield return null;
            rigid.MovePosition(rigid.position + moveOffset);
        }
    }

    public void Setting(int monsterDamage, Vector3 playerPosition)
    {
        setDamage(monsterDamage);
        setDestination(playerPosition);
        direction = (destination - transform.position).normalized;
        Vector3 moveOffset = direction * speed * 0.02f;
        StartCoroutine(Firing(moveOffset));
    }

    void setDamage(int monsterDamage)
    {
        damage = monsterDamage;
    }

    void setDestination(Vector3 playerPosition)
    {
        destination = playerPosition;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            player.GetComponent<PlayerController>().changeHP(damage);
            //Debug.Log("적과 부딪혔다!");
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Tile"))
        {
            //Debug.Log("바닥에 부딪혔다!");
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            //Debug.Log("장애물에 부딪혔다!");
            Destroy(gameObject);
        }
    }
}
