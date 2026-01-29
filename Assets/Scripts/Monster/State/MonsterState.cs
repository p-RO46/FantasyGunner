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

    private int obstacleMask; // layerMask

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

        obstacleMask = LayerMask.GetMask("Obstacle");
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

        if (Vector3.Distance(monster.transform.position, player) <= attackRange)
        {
            monsterMove.ToAttack();
            return;
        }

        if (CanSeePlayer(detect))
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

        if (Vector3.Distance(monster.transform.position, player) <= attackRange)
        {
            monsterMove.ToAttack();
            return;
        }
        monsterMove.ToFollow();
    }

    bool CanSeePlayer(float detectRange)
    {
        Vector3 directionToPlayer = (player - monster.transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(monster.transform.position, player);

        // 1 & 2단계: 거리와 각도 확인
        if (distanceToPlayer < detectRange && Vector3.Angle(monster.transform.forward, directionToPlayer) < detectAngle / 2f)
        {
            // 3단계: 장애물 체크 (Raycast)
            // Obstacle 레이어에 부딪히면 플레이어를 볼 수 없는 것으로 간주
            if (!Physics.Raycast(monster.transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
            {
                return true; // 장애물이 없으므로 플레이어 발견!
            }
        }
        return false;
    }
}