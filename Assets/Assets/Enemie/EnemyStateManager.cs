using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    EnemyBaseState currentState;
    public EnemyIddleState iddleState = new EnemyIddleState();
    public EnemyPatrolState patrolState = new EnemyPatrolState();
    public EnemyDeadState deadState = new EnemyDeadState();
    public Enemie enemie;

    private void Start()
    {
        enemie = GetComponent<Enemie>();
        currentState = patrolState;

        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}
