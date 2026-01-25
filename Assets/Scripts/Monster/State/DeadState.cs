using UnityEngine;

public class DeadState : MonsterState
{
    bool isAlive = true;
    bool isKilled = false;
    float timeUpdate = 0;

    public DeadState(GameObject monster) : base(monster) { }
    public override void Action()
    {
        Dead();
    }
    void Dead()
    {
        if (isKilled) { return; }

        if (isAlive)
        {
            agent.enabled = false;
            animator.SetTrigger("Die");
            isAlive = false;
        }
        else if(timeUpdate >= data.DeadTime)
        {
            monsterManager.AddKills(monster);
            isKilled = true;
        }

        timeUpdate += Time.deltaTime;
    }
}
