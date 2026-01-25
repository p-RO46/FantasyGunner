using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AttackState : MonsterState
{
    private float attackTimeLastUpdate;
    private bool isRotating = false;
    public AttackState(GameObject monster) : base(monster)
    {
        attackTimeLastUpdate = data.AttackTime;
        isAgentStopped = true;
        isSuccessive = true; // 기본이 true
    }
    public override void Action()
    {
        base.Action();

        Attack();

        if (isRotating)
        {
            Vector3 direction = (player - monster.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            if (Quaternion.Angle(monster.transform.rotation, lookRotation) < 0.1f)
            {
                monster.transform.rotation = lookRotation;
                isRotating = false; 

                return;
            }
            monster.transform.rotation = Quaternion.Slerp(monster.transform.rotation, lookRotation, Time.deltaTime * data.RotationSpeed);
        }
    }
    void Attack()
    {
        attackTimeLastUpdate += Time.deltaTime;

        if (attackTimeLastUpdate >= data.AttackTime)
        {
            DetectPlayer();
            if (!isSuccessive) // Follow인 경우
            {
                return;
            }
            
            agent.ResetPath();
            isRotating = true;
            animator.SetTrigger("Attack");
            monster.GetComponentInChildren<AttackSpawner>().SpawnAttack(player);

            attackTimeLastUpdate = 0f;
        }
    }
}
