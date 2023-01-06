using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemie : MonoBehaviour
{
    enum EnemyState
    {
        Iddle,Patrol,FoundEnemy,Dead
    }

    [SerializeField] EnemyState state;

    private void Start()
    {
        state = EnemyState.Iddle;
    }


}
