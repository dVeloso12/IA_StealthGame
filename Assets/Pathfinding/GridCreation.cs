using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreation : MonoBehaviour
{

    [Header("Grid Settings")]
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;

    [SerializeField] Vector3 gridOrigin;
    [SerializeField] float raycastHeight;

    [SerializeField] GameObject PlayerObj;
    [SerializeField] float GridLimit;

    [Header("Is Node Walkable Check")]
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float CheckRadius;

    MapGrid<PathNode> grid;
    Pathfinder pathfinder;


    // Start is called before the first frame update
    void Start()
    {

        width /= (int)cellSize;
        height /= (int)cellSize;


        grid = new MapGrid<PathNode>(gridOrigin, width, height, cellSize, CheckRadius, raycastHeight, PlayerObj, groundLayerMask, (MapGrid<PathNode> g, int x, int y, float cellSize) => new PathNode(g, x, y, cellSize));

        pathfinder = new Pathfinder(grid, raycastHeight, groundLayerMask, CheckRadius);

    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerObj.transform.position.z >= GridLimit)
        {
            GridLimit *= 2f;

            width *= 2;
            height *= 2;

            grid = new MapGrid<PathNode>(gridOrigin, width, height, cellSize, CheckRadius, raycastHeight, PlayerObj, groundLayerMask, (MapGrid<PathNode> g, int x, int y, float cellSize) => new PathNode(g, x, y, cellSize));

            pathfinder = new Pathfinder(grid, raycastHeight, groundLayerMask, CheckRadius);

        }
    }
}
