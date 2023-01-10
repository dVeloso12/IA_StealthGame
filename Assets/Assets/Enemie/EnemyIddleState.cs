using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIddleState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Estou Parado");
    }
    public override void UpdateState(EnemyStateManager enemy)
    {

    }
    public override void OnCollisionEnter(EnemyStateManager enemy)
    {

    }
}
