using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class _Monster : MonoBehaviour
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
    public bool isFollowing = false;

    public float updateInterval = 3f; // 목표 위치를 갱신할 시간 간격 (초)
    private NavMeshAgent agent;
    private float timeSinceLastUpdate; // 마지막으로 목표 위치를 갱신했던 시간
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timeSinceLastUpdate = updateInterval;

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
        float viewAngle = 90f; //예비용
        float angleCos = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
        //플레이어 감지 시 Attack 시도
        if (attackDirection.sqrMagnitude < 100 || 
            (Vector3.Dot(transform.forward, attackDirection.normalized) >= angleCos 
            && attackDirection.sqrMagnitude < monsterData.AttackRange * monsterData.AttackRange))
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
            MonsterMove();
            /*
            //Debug.Log("Update걷기 모드 : " + nextMove.ToString());
            Vector3 moveOffset = transform.forward * monsterData.Speed * Time.fixedDeltaTime * nextMove * 0.1f;
            rigid.MovePosition(rigid.position + moveOffset);
            */
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

        //StopAllCoroutines();
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        //if(attackCoroutine != null) moveCoroutine = null;

        lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        lookRotation *= Quaternion.Euler(new Vector3(0, -40, 0));
        isRotating = true;

        //if(attackCoroutine != null) attackCoroutine = null;
        attackCoroutine = StartCoroutine(AttackAnimation());

        FollowPlayer();
    }
    
    IEnumerator AttackAnimation()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        // 공격 모션
        yield return Util.WaitForSecond(monsterData.AttackTime / 2);
        GetComponentInChildren<AttackSpawner>().SpawnAttack(player.transform.position);
        yield return Util.WaitForSecond(monsterData.AttackTime / 2);

        lookRotation *= Quaternion.Euler(new Vector3(0, 40, 0));
        isAttacking = false;

        moveCoroutine = StartCoroutine(MoveCoroutine());
        attackCoroutine = null;
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
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            isAttacking = false;
            //isMoving = false;
            isRotating = false;

            stunCoroutine = StartCoroutine(StunAnimation(stunTime));
        }

        FollowPlayer();
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

        moveCoroutine = StartCoroutine(MoveCoroutine());
        stunCoroutine = null;
    }
    
    IEnumerator DeadAnimation()
    {
        //Debug.Log("골렘 사망!");
        animator.SetTrigger("Die");
        yield return Util.WaitForSecond(monsterData.DeadTime);
        //notify();
        //Debug.Log("골렘 파괴됨");
        //monsterManager.AddKills();
        Destroy(gameObject);
    }

    IEnumerator MoveCoroutine()
    {
        if (isFollowing)
        {
            moveCoroutine = null;
            yield break;
        }

        nextThinkTime = Random.Range(2f, 5f);
        animator.SetInteger("Walk", nextMove);
        yield return Util.WaitForSecond(nextThinkTime);

        Think();
        moveCoroutine = null;
    }

    void MonsterMove()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            Vector3 randomPosition = GetRandomPositionOnNavMesh();
            agent.SetDestination(randomPosition);
            animator.SetInteger("Walk", nextMove);

            timeSinceLastUpdate = 0f;
        }
    }

    void Think()
    {
        isMoving = false;
        isRotating = false;

        if(!isAlive || isAttacking || isStunned || isFollowing)
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

    // 공격을 하거나 공격당하는 경우, 플레이어를 쫓아간다
    // 공격 범위 내면 공격하고, 멀어지면 쫓아감
    void FollowPlayer()
    {
        isFollowing = true;
        /*
        Vector3 direction = transform.position - player.transform.position;
        float distance = direction.sqrMagnitude;

        isMoving = true;
        isRotating = true;
        if (distance > monsterData.AttackRange)
        {
            lookRotation = Quaternion.Euler(direction.x, 0, direction.z);
            isMoving = true;
            isRotating = true;
        }*/
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        StartCoroutine(Following());
    }

    IEnumerator Following()
    {
        while (true)
        {
            Vector3 direction = player.transform.position - transform.position; // why not updated...
            float distance = direction.sqrMagnitude;
            if ((distance > monsterData.AttackRange * monsterData.AttackRange) && !isAttacking && !isStunned)
            {
                direction = direction.normalized;
                direction.y = 0;
                lookRotation = Quaternion.LookRotation(direction); 

                nextMove = 1;
                animator.SetInteger("Walk", nextMove);
                isMoving = true;
                isRotating = true;
            }
            yield return Util.WaitForSecond(0.01f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Think();
        }
    }
    Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 20f; //랜덤 방향 벡터 생성

        randomDirection += transform.position; //현재 위치에 랜덤 방향 벡터 더한다

        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomDirection, out hit, 20f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return transform.position;
        }
    }
}
