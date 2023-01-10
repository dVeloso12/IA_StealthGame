using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{

    public override void EnterState(EnemyStateManager enemy)
    {
        
    }
      public override void UpdateState(EnemyStateManager enemy)
     {
        enemy.enemie.toPatrol();
    }
     public override void OnCollisionEnter(EnemyStateManager enemy)
    {

    }
}
