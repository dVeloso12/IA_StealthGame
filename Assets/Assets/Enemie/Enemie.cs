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

    [Header("DeadState")]
    [SerializeField] AnimationClip deadClip;
    [SerializeField] float inGroundTime;
    [SerializeField] float timeToDestroy;
    bool canDestroy;
    float destroyTimer;
    bool isDoingAnimDead;
    float doingAnim;

    private void Start()
    {
        transform.position = new Vector3(PatrolPath.GetChild(0).position.x, transform.position.y, PatrolPath.GetChild(0).position.z);
        Mark.enabled = false;
        fow = GetComponent<FieldOfView>();
        patrolLight.color = Color.yellow;
        waypoints = new Vector3[PatrolPath.childCount];
        for(int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = PatrolPath.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        transform.position = waypoints[0];
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
