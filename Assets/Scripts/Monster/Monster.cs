using System.Collections;
using UnityEngine;
using TMPro;

public class Monster : MonoBehaviour, ManagedMonster
{   
    public TextMeshProUGUI hpText;

    Coroutine stunCoroutine;
    Coroutine attackCoroutine;
    Coroutine deadCoroutine;

    Coroutine moveCoroutine;

    MonsterManager monsterManager; // Observer
    Animator animator;
    Rigidbody rigid;
    GameObject player;
    Transform playerTransform;
    public CharacterData monsterData;
    public int curHP { get; set; }
    public bool isAlive { get; set; } = true;

    private Vector3 attackDirection;
    private bool destroy = false;

    private int nextMove = 0;
    private float nextThinkTime;
    private bool isRotating = false;
    private bool isMoving = false;
    private Quaternion lookRotation;

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

        nextThinkTime = Random.Range(0f, 5f);
        Think();
    }

    void FixedUpdate()
    {
        if(destroy) return;

        attackDirection = playerTransform.position - transform.position;
        //플레이어 감지 시 Attack 시도
        if(attackDirection.sqrMagnitude < 100 || 
            (Vector3.Dot(transform.forward, playerTransform.position) < 0 && attackDirection.sqrMagnitude < monsterData.AttackRange * monsterData.AttackRange))
        {
            Attack(attackDirection.normalized);
        }
    }

    void Update()
    {
        if(destroy) return;

        if (isRotating)
        {
            if(transform.rotation == lookRotation) { isRotating = false; }
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * monsterData.RotationSpeed);
        }

        if (!isAlive)
        {
            destroy = true;
            //CancelInvoke();
            StopAllCoroutines();

            isAttacking = false;
            isStunned = false;
            isMoving = false;

            deadCoroutine = StartCoroutine(DeadAnimation());
        }
        else if (!isAttacking && !isStunned && isMoving)
        {
            //Debug.Log("Update걷기 모드 : " + nextMove.ToString());
            Vector3 moveOffset = transform.forward * monsterData.Speed * Time.fixedDeltaTime * nextMove * 0.1f;
            rigid.MovePosition(rigid.position + moveOffset);
        }
    }

    //몬스터 AI 관련 참조 필요
    public void Attack(Vector3 direction)
    {
        if (!isAlive)
        {
            return;
        }

        if (isStunned || isAttacking)
        {
            return;
        }

        StopAllCoroutines();
        if(attackCoroutine != null) moveCoroutine = null;

        lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        lookRotation *= Quaternion.Euler(new Vector3(0, -40, 0));
        isRotating = true;
        //isMoving = false;

        GameObject attack = Instantiate(monsterData.AttackObject, transform.position + new Vector3(0,10,0), Quaternion.identity, transform);
        Rigidbody attackRig = attack.GetComponent<Rigidbody>();
        attackRig.AddForce(direction * monsterData.AttackPower, ForceMode.Force);

        if(attackCoroutine != null) attackCoroutine = null;
        attackCoroutine = StartCoroutine(AttackAnimation());
    }
    
    IEnumerator AttackAnimation()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        // 공격 모션
        yield return Util.WaitForSecond(monsterData.AttackTime);
        lookRotation *= Quaternion.Euler(new Vector3(0, 40, 0));
        isAttacking = false;

        moveCoroutine = StartCoroutine(MoveCoroutine());
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
            isAttacking = false;
            //isMoving = false;
            isRotating = false;

            stunCoroutine = StartCoroutine(StunAnimation(stunTime));
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
        animator.SetTrigger("Hit");
        yield return Util.WaitForSecond(stunTime);
        isStunned = false;

        stunCoroutine = null;

        moveCoroutine = StartCoroutine(MoveCoroutine());
    }
    
    IEnumerator DeadAnimation()
    {
        //Debug.Log("골렘 사망!");
        animator.SetTrigger("Die");
        yield return Util.WaitForSecond(monsterData.DeadTime);
        //notify();
        //Debug.Log("골렘 파괴됨");
        monsterManager.AddKills();
        Destroy(gameObject);
    }

    IEnumerator MoveCoroutine()
    {  
        nextThinkTime = Random.Range(2f, 5f);
        animator.SetInteger("Walk", nextMove);
        yield return Util.WaitForSecond(nextThinkTime);
        Think();
    }

    void Think()
    {
        isMoving = false;
        isRotating = false;

        if(!isAlive || isAttacking || isStunned)
        {
            return;
        }

        nextMove = Random.Range(0, 2);
        //Debug.Log("Think걷기 모드 : " + nextMove.ToString());

        if(nextMove != 0)
        {
            float turnAngle = Random.Range(-90.0f, 90.0f);
            lookRotation = Quaternion.AngleAxis(turnAngle, Vector3.up) * rigid.rotation;

            isMoving = true;
            isRotating = true;
        }

        moveCoroutine = StartCoroutine(MoveCoroutine());
    }

    public void notify()
    {
        //Manager에게 저 죽었어요 라고 알려주기
        //Update 때마다 Manager가 일일이 알아보는건 좀 그렇지않나?
    }
}
