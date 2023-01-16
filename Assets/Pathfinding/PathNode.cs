using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    
    MapGrid<PathNode> grid;

    public int x, y;
    public float cellSize;

    public int gCost, hCost, fCost;
    public PathNode previousNode;

    public List<PathNode> neighborNodes;

    public bool isWalkable;
    float CheckRadius;
    LayerMask groundLayerMask;

    Vector3 worldPosition;

    public PathNode(MapGrid<PathNode> grid, int x, int y, float cellSize)
    {
        this.grid = grid;

        this.x = x;
        this.y = y;
        this.cellSize = cellSize;

        CheckIfIsWalkableAndSave();

        //Debug.Log("Node Pos : " + x + "," + y);

        CheckRadius = grid.CheckRadius;
        groundLayerMask = grid.groundLayerMask;

        Debug.Log("Creating Node");
    }
        
    //public bool CheckIfIsWalkable()
    //{
    //    Vector3 WorldPosition = grid.GetWorldPosition(x, y);
    //    WorldPosition.y = grid.CheckHeight;

    //    if (Physics.OverlapSphere(WorldPosition, CheckRadius, groundLayerMask).Length > 0)
    //    {
    //        return false;
    //    }
    //    else
    //        return true;
    //}

    public void SetIsWalkableTrue(Collider collider)
    {
        isWalkable = false;

        Debug.DrawLine(worldPosition, worldPosition + Vector3.up * 20f, Color.red, 100f);
        collider.enabled = false;
    }

    public void CheckIfIsWalkableAndSave()
    {
        worldPosition = grid.GetWorldPosition(x, y);
        worldPosition.y = grid.CheckHeight;

        ColliderChecker.Instance.CheckCollision(this, worldPosition);

        isWalkable = true;
        ////Debug.Log("Node Pos : " + worldPosition);

        //Color tempColor = Color.white;     

        //if (!isWalkable)
        //{
        //    tempColor = Color.red;
        //}
        //else
        //{
        //    tempColor = Color.green;
        //}
        
        //Debug.DrawLine(worldPosition, worldPosition + Vector3.up * 20f, tempColor, 100f);
    }

    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }

    public override string ToString()
    {
        return x + " , " + y + " : " + isWalkable.ToString();
    }

}
