using UnityEngine;

public class IdleState : MonsterState
{
    public IdleState(GameObject monster) : base(monster) { }
    public override void Action()
    {

    }

    public override string ToString() { return "ป๓ลย : Idle"; }
}
