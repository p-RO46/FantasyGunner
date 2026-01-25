using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    private GameObject _player;
    private Transform player;

    public CharacterData monsterData;

    private MonsterState state;

    public float curStunTime = 0; // public!!!

    private MonsterState walkState;
    private MonsterState attackState;
    private MonsterState followState;
    private MonsterState idleState;
    private MonsterState stunState;
    private MonsterState deadState;

    void Start()
    {
        _player = GameObject.Find("Player");
        player = _player.transform;
    }

    void Update()
    {
        MonsterState.UpdatePlayer(player.position);

        state.Action();
    }

    public void InitState()
    {
        walkState = new WalkState(gameObject, monsterData.WalkInterval);
        attackState = new AttackState(gameObject);
        followState = new FollowState(gameObject);
        idleState = new IdleState(gameObject);
        stunState = new StunState(gameObject);
        deadState = new DeadState(gameObject);
        state = idleState;
    }

    public void ToFollow()
    {
        // 걷기 => 추격
        // 1. 플레이어에게 맞았을 때 (Monster)
        // 2. 몬스터 시야 내로 들어온 경우 (MonsterMove)
        // 3. (미정) 몬스터 기믹에 따라 다른 인식 조건

        // 공격 => 추격
        // 플레이어가 공격 범위를 벗어났을 때 (MonsterMove)
        state = followState;
        followState.isSuccessive = true;
        attackState.isSuccessive = false;
    }

    public void ToAttack()
    {
        state = attackState;
        attackState.isSuccessive = true;
    }

    public void ToWalk()
    {
        // 어그로가 풀리는 조건?
        state = walkState;
    }

    public void ToDead()
    {
        state = deadState;
    }

    public void ToStun(float stunTime) // 연속으로 시도하면 어떡하나요
    {
        curStunTime = stunTime;
        stunState.isSuccessive = true; // 이...렇게요
        state = stunState;
    }

    public void ToIdle()
    {
        state = idleState;
    }
}
