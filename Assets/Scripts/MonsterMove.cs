using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    Animator animator;
    Rigidbody rigid;
    public float moveSpeed = 2f;
    public int nextMove;
    private bool dead = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        if (!dead)
        {
            if (!GetComponent<CharacterHandler>().isAlive)
            {
                animator.SetBool("Alive", false);
                CancelInvoke();
                dead = true;
            }
            Vector3 moveOffset = transform.forward * moveSpeed * Time.fixedDeltaTime * nextMove;
            rigid.MovePosition(rigid.position + moveOffset);
        }
    }
    
    void Think()
    {
        nextMove = Random.Range(0, 2);
        animator.SetInteger("Walk", nextMove);
        if(nextMove != 0)
        {
            float turnAngle = Random.Range(10.0f, 180.0f);
            rigid.rotation = Quaternion.AngleAxis(turnAngle * nextMove, Vector3.up) * rigid.rotation; // �¿� ȸ��
        }
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }
}
