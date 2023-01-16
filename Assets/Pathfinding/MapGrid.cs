using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using CodeMonkey.Utils;

public class MapGrid<TGridObj>
{

    TGridObj[,] grid;

    int width, height;
    float cellSize;

    public float CheckRadius, CheckHeight;
    public LayerMask groundLayerMask;

    Vector3 gridOrigin;

    TextMesh[,] debugTextArray;

    #region Propriedades
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public float CellSize { get { return cellSize; } }
    #endregion


    public MapGrid(Vector3 gridOrigin, int width, int height, float cellSize, float checkRadius, float CheckHeight, GameObject PlayerObj, LayerMask groundLayerMask, Func<MapGrid<TGridObj>, int, int, float, TGridObj> createGridObj)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.CheckRadius = checkRadius;
        this.CheckHeight = CheckHeight;

        this.gridOrigin = gridOrigin;

        this.groundLayerMask = groundLayerMask; 

        grid = new TGridObj[width, height];

        for(int x = 0; x < grid.GetLength(0); x++)
        {
            for(int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = createGridObj(this, x, y, cellSize);
            }
        }

        DrawGrid();
    }

    public TGridObj GetGridObj(float x, float y)
    {
        
        //Debug.LogWarning("Width : " + width + "; height : " + height);        

        if (x >= 0 && x < width && y >= 0 && y < height)
        {            
            return grid[(int)x, (int)y];
        }
        else
        {
            //Debug.LogWarning("No pos on grid.");
            //Debug.LogWarning("Pos : " + x + " : " + y);

            return default(TGridObj);
        }

    }
    public Vector3 GetWorldPosition(int x, int y)
    {
        Vector3 tempVec = new Vector3(x, 0, y);


        return tempVec * cellSize + gridOrigin;

    }
    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        //Debug.Log("WOrld position x : " + worldPosition.x);
        x = Mathf.FloorToInt((worldPosition - gridOrigin).x / cellSize);
        //Debug.Log("Grid X : " + x);
        y = Mathf.FloorToInt((worldPosition - gridOrigin).y / cellSize);

    }
    public void DrawGrid()
    {
        bool ToDebug = true;


        if (ToDebug)
        {

            debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {

                    //debugTextArray[x, y] = UtilsClass.CreateWorldText(GridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter);

                    //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

                    Vector3 worldPosition = GetWorldPosition(x, y);
                    Vector3 worldPositionYPlus1 = GetWorldPosition(x, y + 1);
                    Vector3 worldPositionXPlus1 = GetWorldPosition(x + 1, y);

                    //Debug.Log("World Position 01 : " + worldPosition);
                    //Debug.Log("World Position 02 : " + worldPositionYPlus1);
                    //Debug.Log("World Position 03 : " + worldPositionXPlus1);

                    //string toDebug = GridArray[x, y]?.ToString() + GridArray[x, y].



                    debugTextArray[x, y] = UtilsClass.CreateWorldText(grid[x, y]?.ToString(), null, new Vector3(worldPosition.x, 0, worldPosition.y) + new Vector3(cellSize, 0, cellSize) * 0.5f, 5, Color.white, TextAnchor.MiddleCenter);

                    Debug.DrawLine(new Vector3(worldPosition.x, 0, worldPosition.y)
                        , new Vector3(worldPositionYPlus1.x, 0, worldPositionYPlus1.y)
                        , Color.white, 100f);


                    Debug.DrawLine(new Vector3(worldPosition.x, 0, worldPosition.y)
                        , new Vector3(worldPositionXPlus1.x, 0, worldPositionXPlus1.y)
                        , Color.white, 100f);

                }
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }

    }

}
