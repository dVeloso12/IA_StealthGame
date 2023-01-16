using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    public static ColliderChecker Instance;

    PathNode currentNode;
    Collider collider;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        collider = GetComponent<Collider>();
    }

    public void CheckCollision(PathNode node, Vector3 positionTocheck)
    {
        transform.position = positionTocheck;

        Debug.Log("ENabling");

        collider.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit---");
        if (collision.collider.tag != "Ground")
        {
            Debug.Log("Hit");
            currentNode.SetIsWalkableTrue(collider);
        }
    }
    //private void on(Collider other)
    //{
    //    Debug.Log("Hit---");
    //    if(other.tag != "Ground")
    //    {
    //        Debug.Log("Hit");
    //        currentNode.SetIsWalkableTrue(collider);
    //    }
    //}
}
