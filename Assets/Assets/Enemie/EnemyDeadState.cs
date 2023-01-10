using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {

    }
    public override void UpdateState(EnemyStateManager enemy)
    {
        enemy.enemie.ToDie();
    }
    public override void OnCollisionEnter(EnemyStateManager enemy)
    {

    }

}

