using UnityEngine;
using UnityEngine.PlayerLoop;

public class StunState : MonsterState
{
    bool isStunned = false;
    float timeUpdate = 0;
    public StunState(GameObject monster) : base(monster) 
    {
        isAgentStopped = true;
    }
    public override void Action()
    {
        base.Action();

        if (isSuccessive)
        {
            isSuccessive = false;
            isStunned = false;
            timeUpdate = 0f;
        }
        Stun();
    }
    void Stun()
    {
        timeUpdate += Time.deltaTime;
        if (!isStunned)
        {
            agent.isStopped = true;
            animator.SetTrigger("Hit");
            isStunned = true;
        }
        else if (timeUpdate >= monsterMove.curStunTime)
        {
            isStunned = false;
            agent.isStopped = false;
            monsterMove.curStunTime = 0f;
            monsterMove.ToFollow();
        }
    }
}
