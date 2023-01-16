using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{

    private const int MOVE_STRAIGHT_COST = 10, MOVE_DIAGONAL_COST = 15;
    public static Pathfinder Instance;

    MapGrid<PathNode> grid;

    List<PathNode> OpenList, ClosedList;

    float raycastHeight;
    LayerMask groundLayerMask;
    float CheckRadius;

    public Pathfinder(MapGrid<PathNode> grid, float raycastHeight, LayerMask groundLayerMask, float CheckRadius)
    {
        this.grid = grid;
        this.raycastHeight = raycastHeight;
        this.groundLayerMask = groundLayerMask;
        this.CheckRadius = CheckRadius;

        Instance = this;
    }

    public List<PathNode> FindPath(float startX, float startY, float endX, float endY, Vector3 EnemyPos, Vector3 TargetPos)
    {
        int tempX = (int)startX, tempY = (int)startY;

        grid.GetXY(EnemyPos, out tempX, out tempY);


        PathNode StartNode = grid.GetGridObj(tempX, tempY);

        tempX = (int)endX;
        tempY = (int)endY;

        grid.GetXY(TargetPos, out tempX, out tempY);

        PathNode EndNode = grid.GetGridObj(tempX, tempY);


        Debug.LogWarning("Start Node : " + StartNode.x + " , " + StartNode.y);
        Debug.LogWarning("End Node : " + EndNode.x + " , " + EndNode.y);


        if (StartNode == null)
        {
            Debug.LogWarning("Start Node e nula.");
            
            Debug.LogWarning("Start Node Coord : " + startX + " : " + startY);
            return null;
        }
        if(EndNode == null)
        {
            Debug.LogWarning("End Node e nula");
            Debug.LogWarning("End Node Coord : " + endX + " : " + endY);

            return null;
        }

        OpenList = new List<PathNode>() { StartNode };
        ClosedList = new List<PathNode>();
        
        for(int x = 0; x < grid.Width; x++)
        {
            for(int y = 0; y < grid.Height; y++)
            {

                PathNode currentNode = grid.GetGridObj(x, y);
                currentNode.gCost = int.MaxValue;

                currentNode.CalculateFCost();
                currentNode.previousNode = null;
            }
        }


        StartNode.gCost = 0;
        StartNode.hCost = CalculateDistance(StartNode, EndNode); //??????
        StartNode.CalculateFCost();

        while(OpenList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(OpenList);
            if(currentNode == EndNode)
            {
                Debug.Log("Final Node");
                return CalculatePath(EndNode);
            }

            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            if(currentNode.neighborNodes == null)
            {
                currentNode.neighborNodes = GetNeighborNodes(currentNode);
            }

            foreach(PathNode neighborNode in currentNode.neighborNodes)
            {
                if (ClosedList.Contains(neighborNode)) continue;

                if (!neighborNode.isWalkable)
                {
                    ClosedList.Add(neighborNode);
                    continue;
                }

                int tentativeCost = currentNode.gCost + CalculateDistance(currentNode, neighborNode);
                if(tentativeCost < neighborNode.gCost)
                {
                    neighborNode.previousNode = currentNode;
                    neighborNode.gCost = tentativeCost;
                    neighborNode.hCost = CalculateDistance(neighborNode, EndNode);
                    neighborNode.CalculateFCost();

                    if(!OpenList.Contains(neighborNode)) OpenList.Add(neighborNode);
                }
            }

        }

        Debug.LogWarning("Out of Nodes. No available Path");
        return null;

    }

    public List<PathNode> GetNeighborNodes(PathNode currentNode)
    {
        List<PathNode> neighborNodes = new List<PathNode>();

        if (currentNode.x - 1 >= 0)
        {
            //Left
            neighborNodes.Add(GetNode(currentNode.x - 1, currentNode.y));

            //Left Down
            if (currentNode.y - 1 >= 0) neighborNodes.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            //Left Up
            if (currentNode.y + 1 < grid.Height) neighborNodes.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.Height)
        {
            //Right
            neighborNodes.Add(GetNode(currentNode.x + 1, currentNode.y));

            //Right Down
            if (currentNode.y - 1 >= 0) neighborNodes.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            //Right Up
            if (currentNode.y + 1 < grid.Height) neighborNodes.Add(GetNode(currentNode.x + 1, currentNode.y + 1));

        }

        //Down
        if (currentNode.y - 1 >= 0) neighborNodes.Add(GetNode(currentNode.x, currentNode.y - 1));
        //Up
        if (currentNode.y + 1 < grid.Height) neighborNodes.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighborNodes;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObj(x, y);
    }

    public List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathList = new List<PathNode>();

        pathList.Add(endNode);
        PathNode currentNode = endNode;

        while(currentNode.previousNode != null)
        {
            pathList.Add(currentNode.previousNode);
            currentNode = currentNode.previousNode;
        }

        pathList.Reverse();
        return pathList;
        
    }

    public int CalculateDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;   

    }

    public PathNode GetLowestFCostNode(List<PathNode> openListTemp)
    {
        PathNode lowestFCostNode = openListTemp[0];

        for(int i = 1; i < openListTemp.Count; i++)
        {
            if(openListTemp[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = openListTemp[i];
            }
        }

        return lowestFCostNode;
    }

    public List<Vector3> FindPathPositionsOnMap(List<PathNode> pathList)
    {
        List<Vector3> PathPositionsOnMap = new List<Vector3>();

        if (pathList == null) return null;

        foreach (PathNode node in pathList)
        {

            float height = -1f;
            //Debug.Log("Height : " + height);

            Vector3 worldPosition = grid.GetWorldPosition(node.x, node.y);

            RaycastHit hit;
            if (Physics.Raycast(new Vector3(worldPosition.x, raycastHeight, worldPosition.y), -Vector3.up, out hit, 100f, groundLayerMask))
            {

                height = hit.point.y + 0.5f;
                //Debug.Log("tutsjsHeight : " + height);
            }

            Vector3 tempPos = grid.GetWorldPosition((int)hit.point.x, (int)hit.point.y);

            Vector3 position = new Vector3(worldPosition.x, height, worldPosition.y);

            PathPositionsOnMap.Add(position);

        }

        return PathPositionsOnMap;
    }
}
