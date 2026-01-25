using UnityEngine;
using UnityEngine.AI;

public class WalkState : MonsterState
{
    private float timeSinceLastUpdate;
    private float updateInterval;

    public WalkState(GameObject monster, float interval) : base(monster)
    {
        updateInterval = interval;
        timeSinceLastUpdate = updateInterval;

        dest = new Vector3();
        anim = 1;
        speed = data.Speed;
        isAgentStopped = false;
    }
    public override void Action()
    {
        base.Action();

        DetectPlayer();
        Walk();
    }

    void Walk()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            Vector3 randomPosition = GetRandomPositionOnNavMesh();
            agent.SetDestination(randomPosition);
            agent.speed = speed;

            timeSinceLastUpdate = 0f;
        }

        if (HasArrived()) // Agent가 시간이 다 되기 전에 목적지에 도달
        {
            animator.SetInteger("Walk", 0);
        }
    }

    Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 50f; //랜덤 방향 벡터 생성

        randomDirection += monster.transform.position; //현재 위치에 랜덤 방향 벡터 더한다

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 50f, NavMesh.AllAreas))
        {
            animator.SetInteger("Walk", anim);
            return hit.position;
        }
        else
        {
            animator.SetInteger("Walk", 0);
            return monster.transform.position;
        }
    }
    bool HasArrived()
    {
        // 1. 경로를 계산 중인지 확인 (계산 중이면 아직 도착한 게 아님)
        if (!agent.pathPending)
        {
            // 2. 남은 거리가 멈춤 거리(Stopping Distance) 이하인지 확인
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 3. 경로가 없거나 속도가 거의 0인지 확인
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
