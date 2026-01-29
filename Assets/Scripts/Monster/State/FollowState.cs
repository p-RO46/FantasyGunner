using UnityEngine;

public class FollowState : MonsterState
{
    private float forgetTime = 5f;
    public FollowState(GameObject monster) : base(monster)
    {
        dest = new Vector3();
        anim = 2;
        speed = data.RunSpeed;
        isAgentStopped = false;
    }

    public override void Action()
    {
        base.Action();

        if (!isSuccessive)
        {
            lostTimer = 0f;
            isSuccessive = true;
        }
        DetectPlayer();
        Follow();
    }

    void Follow()
    {
        lostTimer += Time.deltaTime;

        if(lostTimer >= forgetTime)
        {
            monsterMove.ToWalk();
            isSuccessive = true;
            lostTimer = 0f;
        }
        else
        {
            agent.SetDestination(player);
            agent.speed = speed;
            animator.SetInteger("Walk", anim);
        }
    }
}
