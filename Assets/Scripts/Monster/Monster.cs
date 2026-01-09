using System.Collections;
using UnityEngine;
using TMPro;

public class Monster : MonoBehaviour, ManagedMonster
{   
    public TextMeshProUGUI hpText;
    MonsterManager monsterManager; // Observer
    Animator animator;
    Rigidbody rigid;
    GameObject player;
    Transform playerTransform;
    public CharacterData monsterData;
    public int curHP { get; set; }
    public bool isAlive { get; set; } = true;

    private Vector3 attackDirection;

    private int nextMove;
    //public bool isMoving { get; set; } = false;
    //public bool isThinking { get; set; }

    public bool isAttacking { get; set; } = false;
    public bool isStunned { get; set; } = false;

    void Start()
    {
        curHP = monsterData.MaxHP;

        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.GetComponent<Transform>();

        GameObject managerObject = GameObject.FindGameObjectWithTag("MonsterManager");
        monsterManager = managerObject.GetComponent<MonsterManager>();

        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        UpdateHP();
        //Invoke("Think", 5);
    }
    void Update()
    {
        attackDirection = transform.position - playerTransform.position;
        //플레이어 감지 시 Attack 시도
        if(attackDirection.sqrMagnitude < monsterData.AttackRange * monsterData.AttackRange)
        {
            Attack(attackDirection.normalized);
        }
    }

    void FixedUpdate()
    {
        if (!isAlive)
        {
            //CancelInvoke();
            StopAllCoroutines();
            StartCoroutine(DeadAnimation());
        }
        else if (!isAttacking)
        {
            Vector3 moveOffset = transform.forward * monsterData.Speed * Time.fixedDeltaTime * nextMove;
            rigid.MovePosition(rigid.position + moveOffset);
        }
    }

    //몬스터 AI 관련 참조 필요
    public void Attack(Vector3 direction)
    {
        //CancelInvoke();

        if (isStunned || !isAlive)
        {
            return;
        }
        
        // 인스턴스 파괴 로직은 AttackObject에서 구현하도록
        GameObject attack = Instantiate(monsterData.AttackObject, transform.position + new Vector3(0,4,0), Quaternion.identity);
        Rigidbody attackRig = attack.GetComponent<Rigidbody>();
        attackRig.AddForce(direction * monsterData.AttackPower, ForceMode.Impulse);
        StartCoroutine(AttackAnimation());
        //Invoke("Think", 5);
    }
    
    IEnumerator AttackAnimation()
    {
        isAttacking = true;
        Debug.Log("골렘 공격!");
        animator.SetInteger("Walk", nextMove);
        // 공격 모션
        yield return Util.WaitForSecond(monsterData.AttackTime);
        isAttacking = false;
    }

    public void Attacked(int damage, bool hasStun, float stunTime)
    {
        if (!isAlive)
        {
            return;
        }

        curHP -= damage;
        UpdateHP();
        if(curHP <= 0)
        {
            curHP = 0;
            isAlive = false;
        }
        else if (hasStun)
        {
            StopAllCoroutines();
            StartCoroutine(StunAnimation(stunTime));
        }
    }

    private void UpdateHP()
    {
        if(curHP >= 0)
        {
            hpText.text = curHP.ToString();
        }
        else
        {
            hpText.text = "0";
        }
    }

    IEnumerator StunAnimation(float stunTime)
    {
        isStunned = true;
        // 피격 모션
        yield return Util.WaitForSecond(stunTime);
        isStunned = false;
    }
    
    IEnumerator DeadAnimation()
    {
        animator.SetBool("Alive", false);
        yield return Util.WaitForSecond(monsterData.DeadTime);
        //notify();
        Destroy(gameObject);
    }

    void Think()
    {
        if(!isAlive)
        {
            return;
        }

        nextMove = Random.Range(0, 2);
        animator.SetInteger("Walk", nextMove);
        if(nextMove != 0)
        {
            float turnAngle = Random.Range(10.0f, 180.0f);
            rigid.rotation = Quaternion.AngleAxis(turnAngle * nextMove, Vector3.up) * rigid.rotation;
        }
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    public void notify()
    {
        //Manager에게 저 죽었어요 라고 알려주기
        //Update 때마다 Manager가 일일이 알아보는건 좀 그렇지않나?
    }
}
