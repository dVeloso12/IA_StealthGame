using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemie : MonoBehaviour
{
    public enum EnemyState
    {
        Iddle,Patrol,FoundEnemy,Dead
    }

    [SerializeField] public EnemyState state;
    EnemyState LastState;
    Coroutine currentCorritene;
    [Header("UI_Elements")]
    [SerializeField] Image Mark;
    [Header("General Stuff")]
    FieldOfView fow;
    [SerializeField] float onAlertSpeed;
    [SerializeField] public Light patrolLight;
    float AlertTimer;
    [Header("Animation Stuff")]
    [SerializeField] Animator anim;

    public bool Catched;

    [Header("PatrolState")]
    [SerializeField] Transform PatrolPath;
    [SerializeField] float patrolSpeed = 5;
    [SerializeField] float waitTime = .3f;
    [SerializeField] float turnSpeed = 90;
    Vector3[] waypoints;
    [Header("DeadState")]
    [SerializeField] float inGroundTime;
    

    bool stateCheck;
     bool onPatrol;
     Vector3 LastPosition;
     int LastWaypoint;

    private void Start()
    {
        transform.position = new Vector3(PatrolPath.GetChild(0).position.x, transform.position.y, PatrolPath.GetChild(0).position.z);
        //state = EnemyState.Iddle;
        Mark.enabled = false;
        fow = GetComponent<FieldOfView>();
        patrolLight.color = Color.yellow;
        waypoints = new Vector3[PatrolPath.childCount];
        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = PatrolPath.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        stateCheck = false;
        onPatrol = false;
        //StartCoroutine(FollowPath(waypoints));
    }

    private void Update()
    {
        UpdateMarkImage();
        StateMachine();
       
    }
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y,targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        int targetWaypointIndex;
        if (!onPatrol)
        {
            transform.position = waypoints[0];
            targetWaypointIndex = 1;
            LastWaypoint = 1;
            onPatrol = true;
        }
        else
        {
            targetWaypointIndex = LastWaypoint;
            transform.position = LastPosition;
        }
        
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while(true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, patrolSpeed * Time.deltaTime);
            anim.SetBool("isWalk", true);
            LastPosition = transform.position;
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length; // quando chegar ao valor igual, volta para 0
                targetWaypoint = waypoints[targetWaypointIndex];
                LastWaypoint = targetWaypointIndex;
                anim.SetBool("isWalk", false);
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    IEnumerator Dead()
    {
        anim.SetTrigger("isDead");
        yield return new WaitForSeconds(inGroundTime);
         gameObject.SetActive(false);
        yield return null;
    }
    void StateMachine()
    {
        if(LastState != state)
        {
            stateCheck = true;
            LastState = state;
            StopCoroutine(currentCorritene);
        }
        if(state == EnemyState.Iddle)
        {
            anim.SetBool("isWalk", false);
        }
        else if (state == EnemyState.Patrol)
        {
            if(stateCheck)
            {
                currentCorritene = StartCoroutine(FollowPath(waypoints));
                
                stateCheck = false;
            }
          
            
        }
        else if(state == EnemyState.Dead)
        {
            if(stateCheck)
            {
                currentCorritene = StartCoroutine(Dead());
                stateCheck = false;
            }
           
        }
    }

    void UpdateMarkImage()
    {
        if(fow.onAlert)
        {
            Mark.enabled = true;
            Mark.fillAmount += onAlertSpeed * Time.deltaTime;
            if(Mark.fillAmount == 1f)
            {
                Debug.Log("FOSTE VISTOOO");
                Catched = true;
            }
            
        }
        else
        {
            Mark.fillAmount = 0f;
            Mark.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = PatrolPath.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach(Transform tras in PatrolPath)
        {
            Gizmos.DrawSphere(tras.position, 0.3f);
            Gizmos.DrawLine(previousPosition,tras.position);
            previousPosition = tras.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

    }


}
