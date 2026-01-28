using UnityEngine;
using UnityEngine.AI;

public class MonsterState
{
    protected static Vector3 player; // 플레이어 실시간 위치
    public static void UpdatePlayer(Vector3 _player)
    {
        player = _player;
    }

    public GameObject monster;
    public FootstepSound playerState;

    protected MonsterManager monsterManager;
    protected MonsterMove monsterMove; // 상태 객체를 가질 컴포넌트
    protected CharacterData data;

    protected NavMeshAgent agent;
    protected Animator animator;

    protected Vector3 dest;
    protected int anim; // 0:Idle 1:Walk 2:Run 3:Attack
    protected float speed;
    protected bool isAgentStopped;

    private float attackRange;
    private float detectRange;
    private float detectAngle;
    private float angleCos;

    public bool isSuccessive = false; // ToStun이 연속으로 호출된 경우.
    public float lostTimer = 0f; // Follow 용

    public MonsterState(GameObject monster)
    {
        this.monster = monster;

        // Really Dumb Code
        playerState = GameObject.FindWithTag("Player").transform.GetChild(2).gameObject.GetComponent<FootstepSound>();

        monsterManager = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        monsterMove = monster.GetComponent<MonsterMove>();
        agent = monster.GetComponent<NavMeshAgent>();
        animator = monster.GetComponent<Animator>();
        data = monsterMove.monsterData;

        attackRange = data.AttackRange;
        detectRange = data.DetectRange;
        detectAngle = data.DetectRange;

        angleCos = Mathf.Cos(detectAngle * 0.5f * Mathf.Deg2Rad);
    }

    public virtual void Action()
    {
        if (agent.enabled)
        {
            agent.isStopped = isAgentStopped;
        }
    }
    protected void DetectPlayer()
    {
        float detect = detectRange;
        if (playerState.isCrouching)
        {
            detect /= 2;
        }
        else if (playerState.isRunning)
        {
            detect *= 1.5f;
        }

        Vector3 direction = player - monster.transform.position;
        //플레이어 감지 시 상태변화

        if (Vector3.Dot(monster.transform.forward, direction.normalized) >= angleCos &&
            (direction.sqrMagnitude <= attackRange * attackRange))
        {
            monsterMove.ToAttack();
        }
        else if (direction.sqrMagnitude <= detect * detect)
        {
            monsterMove.ToFollow();
        }
        else
        {
            monsterMove.ToWalk();
        }
    }

    protected void DetectPlayer_Attack()
    {
        Vector3 direction = player - monster.transform.position;
        //플레이어 감지 시 상태변화

        if (Vector3.Dot(monster.transform.forward, direction.normalized) >= angleCos &&
            (direction.sqrMagnitude <= attackRange * attackRange))
        {
            monsterMove.ToAttack();
        }
        else
        {
            monsterMove.ToFollow();
        }
    }
}