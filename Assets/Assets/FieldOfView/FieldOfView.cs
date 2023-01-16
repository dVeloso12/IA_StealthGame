using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    Enemie enemy;
    public float viewRadious;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform> ();

    public float MeshResolution;
    public int edgeResolveIterations;
    public float edgeDSTThreshold;

    public MeshFilter meshFilter;
    Mesh viewMesh;

    public bool onAlert;

    private void Start()
    {
        viewMesh = new Mesh();
        enemy = GetComponent<Enemie>();
        viewMesh.name = "View Mesh";
        meshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetsWithDelay",0.2f);
    }

    //private void Update()
    //{
    //    DrawFieldOfView();

    //}
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds (delay);
            FindVisibleTargets ();
        }
    }
    void FindVisibleTargets()
    {
        Collider[] targetsinViewRadius = Physics.OverlapSphere(transform.position, viewRadious, targetMask);

        for( int i = 0; i < targetsinViewRadius.Length;i++ )
        {
            Transform target = targetsinViewRadius[i].transform;
            Vector3 dirTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward,dirTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position,target.position);

                if (Physics.Raycast(transform.position, dirTarget, dstToTarget))
                {
                    if(target.name == "Player")
                    {
                        Debug.Log("Vi-te otario");
                        onAlert = true;
                        enemy.patrolLight.color = Color.red;
                        addToList(target);
                    }
                  
                }
                else
                {
                    onAlert = false;
                    enemy.patrolLight.color = Color.yellow;
                    visibleTargets.Remove(target);
                }
 

            }
            else
            {
                onAlert = false;
                enemy.patrolLight.color = Color.yellow;
                visibleTargets.Remove(target);
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * MeshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> ViewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for(int i = 0; i <= stepCount;i++ )
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if(i > 0 )
            {
                bool edgeDSTThresholderExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDSTThreshold;
                if(oldViewCast.hit != newViewCast.hit ||(oldViewCast.hit && newViewCast.hit && edgeDSTThresholderExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero)
                    {
                        ViewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        ViewPoints.Add(edge.pointB);

                    }
                }
            }


            ViewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;

        }
        int vertexCount = ViewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertexCount -1;i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(ViewPoints[i]);
            if(i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast,ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle / 2);
            ViewCastInfo newcast = ViewCast(angle);
            bool edgeDSTThresholderExceeded = Mathf.Abs(minViewCast.dst - newcast.dst) > edgeDSTThreshold;
            if (newcast.hit == minViewCast.hit && !edgeDSTThresholderExceeded)
            {
                minAngle = angle;
                minPoint = newcast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newcast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if(Physics.Raycast(transform.position,dir,out hit,viewRadious,obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadious, viewRadious, globalAngle);

        }

    }
    void addToList(Transform target)
    {
        foreach(Transform trans in visibleTargets)
        {
            if(trans == target)
            {
                return;
            }
        }
        visibleTargets.Add(target);
    }

    public Vector3 DirFromAngle(float angleInDegrees,bool angleisGlobal)
    {
        if(angleisGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));   
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit,Vector3 _point,float _dst,float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA,Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }

    }
}
