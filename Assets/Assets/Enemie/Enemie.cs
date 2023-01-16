using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemie : MonoBehaviour
{
    [Header("UI_Elements")]
    [SerializeField] Image Mark;
    [Header("General Stuff")]
    FieldOfView fow;
    [SerializeField] float onAlertSpeed;
    [SerializeField] public Light patrolLight;
    bool canDetect;
    //float AlertTimer;
    [Header("Animation Stuff")]
    [SerializeField] Animator anim;

    public bool Catched;

    [Header("PatrolState")]
    [SerializeField] Transform PatrolPath;
    [SerializeField] float patrolSpeed = 5;
    [SerializeField] float waitTime = .3f;
    [SerializeField] float turnSpeed = 90;
    Vector3 targetWaypoint;
    public int targetWaypointIndex;
    float targetAngle;
    float toTurnTimer;
    bool toTurn,waitToTurn;
    bool canChoose;
    Vector3[] waypoints;
    Pathfinder pathfinder;

    [Header("DeadState")]
    [SerializeField] AnimationClip deadClip;
    [SerializeField] float inGroundTime;
    [SerializeField] float timeToDestroy;
    bool canDestroy;
    float destroyTimer;
    bool isDoingAnimDead;
    float doingAnim;


    [Header("Targets")]
    public Transform Target1,Target2;

    Vector3 StartPos_Patrol, EndPos_Patrol;

    List<PathNode> path;

    private void Start()
    {
        Mark.enabled = false;
        fow = GetComponent<FieldOfView>();

        pathfinder = Pathfinder.Instance;
        
        patrolLight.color = Color.yellow;
        if (pathfinder == null)
            Debug.LogWarning("Path e nulo");

        //transform.position += new Vector3(1, 0, 1);

        StartPos_Patrol = Target1.position;
        EndPos_Patrol = Target2.position;

        transform.position = StartPos_Patrol;

        Debug.LogWarning("Start Pos : " + StartPos_Patrol);
        Debug.LogWarning("End Pos : " + EndPos_Patrol);

        path = pathfinder.FindPath(/*Target1.position*/StartPos_Patrol.x, /*Target1.position*/StartPos_Patrol.z, /*Target2.position*/EndPos_Patrol.x, /*Target2.position*/EndPos_Patrol.z, transform.position, EndPos_Patrol);

        waypoints = new Vector3[path.Count];

        //transform.position = waypoints[0];

        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = new Vector3(path[i].x, transform.position.y, path[i].y);
        }


        //transform.position = waypoints[0];
        Debug.Log("path lenght : " + path.Count);
        targetWaypointIndex = 1;
        canChoose = true;
        canDetect = true;
    }

    private void Update()
    {
        UpdateMarkImage();   
    }

    public void toPatrol()
    {

        if(canChoose)
        {
            targetWaypoint = waypoints[targetWaypointIndex];
            transform.LookAt(targetWaypoint);
            canChoose = false;
            waitToTurn = false;
        }
        else
        {
            if(!waitToTurn)
            {
               
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, patrolSpeed * Time.deltaTime);
                anim.SetBool("isWalk", true);

                if (transform.position == targetWaypoint && !toTurn)
                {
                    targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length; // quando chegar ao valor igual, volta para 0

                    //Chegou ao final. Voltar para o inicial
                    if(targetWaypointIndex == waypoints.Length - 1)
                    {
                        path.Reverse();
                        waypoints = new Vector3[path.Count];
                        for(int i = 0; i < path.Count; i++)
                        {
                            waypoints[i] = new Vector3(path[i].x, transform.position.y, path[i].y);
                        }

                        targetWaypointIndex = 1;
                    }

                    targetWaypoint = waypoints[targetWaypointIndex];
                    anim.SetBool("isWalk", false);
                    waitToTurn = true;

                }
            }
            else
            {
                toTurnTimer += Time.deltaTime;
                if(toTurnTimer >= waitTime && !toTurn)
                {
                    Vector3 dirToLookTarget = (targetWaypoint - transform.position).normalized;
                    targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
                    toTurn = true;
                }
                if(toTurn)
                {
                    if(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
                    {
                        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
                        transform.eulerAngles = Vector3.up * angle;
                    }
                    else
                    {
                        toTurn = false;
                        toTurnTimer = 0;
                        canChoose = true;
                        waitToTurn=false;
                    }
                }
            }
        }
      

    }

    public void ToDie()
    {

        if(!isDoingAnimDead)
        {
            anim.SetTrigger("isDead");
            isDoingAnimDead = true;
            this.GetComponent<Collider>().enabled = false;
            UpdateEnemyOnDeadth();
        }
        else
        {
            if (!canDestroy)
            {
                doingAnim += Time.deltaTime;
                if (doingAnim > deadClip.length)
                {
                    canDestroy = true;
                }
            }
            else
            {
                destroyTimer += Time.deltaTime;
                if (destroyTimer >= timeToDestroy)
                {
                    gameObject.SetActive(false);
                }
            }    

        }
        
    }
    void UpdateMarkImage()
    {
        if (canDetect)
        {
            if (fow.onAlert)
            {
                Mark.enabled = true;
                Mark.fillAmount += onAlertSpeed * Time.deltaTime;
                if (Mark.fillAmount == 1f)
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
    }

    void UpdateEnemyOnDeadth()
    {
        patrolLight.enabled = false;
        canDetect = false;
    }
    private void OnDrawGizmos()
    {
        //Vector3 startPosition = PatrolPath.GetChild(0).position;
        //Vector3 previousPosition = startPosition;
        //foreach(Transform tras in PatrolPath)
        //{
        //    Gizmos.DrawSphere(tras.position, 0.3f);
        //    Gizmos.DrawLine(previousPosition,tras.position);
        //    previousPosition = tras.position;
        //}
        //Gizmos.DrawLine(previousPosition, startPosition);

    }


}
