using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap 
{
    private Cell[,] matrix;
    private const int boardRows = 50;
    private const int boardCols = 50;
    private const int minNumberOfWalls = 50;
    private const int maxNumberOfWalls = 50;

    public void InitializeBoard()
    {
        matrix = new Cell[boardRows, boardCols];
        for(int i = 0; i < boardRows; i++)
        {
            for(int j = 0; j < boardCols; j++)
            {
                matrix[i, j] = new Cell();
            }
        }
    }



    public void PlaceSnakePartAt(int r, int c)
    {
        matrix[r, c].Snake=(new SnakeEntity());
        //matrixSnake[r, c] = 1;
    }

    public void PlaceWallAt(int r, int c)
    {
        matrix[r, c].Wall=(new WallEntity());
        //matrixWalls[r, c] = 1;
    }

    public void PlaceFrutitaAt(int r, int c)
    {
        matrix[r, c].Fruit=(new FruitEntity());
        //matrixFrutitas[r, c] =  1;
    }




    public bool IsWallAt(int r,int c)
    {
        return matrix[r, c].HasWall();
    }

    public bool IsFruitAt(int r, int c)
    {
        return matrix[r,c].HasFruit();
    }

    public bool IsSnakeAt(int r, int c)
    {
        return matrix[r, c].HasSnake();
    }





    public void FreeSnakePartAt(int r, int c)
    {
        matrix[r, c].Snake=null;
    }

    public void FreeFruitAt(int r, int c)
    {
        matrix[r, c].Fruit=null;
    }





    public int RandomWallValue()
    {
        return Random.Range(minNumberOfWalls, maxNumberOfWalls);
    }

    public int GetRows()
    {
        return boardRows;
    }

    public int GetColumns()
    {
        return boardCols;
    }


    public Vector3Int GetFreePosition()
    {
        bool isAvailable = false;
        Vector3Int newFreePosition = new Vector3Int(0, 0, 0);
        while (!isAvailable)
        {
            newFreePosition = new Vector3Int(Random.Range(1, GetRows()), 0, Random.Range(1, GetColumns()));
            isAvailable = IsFreeSurroundings(newFreePosition);
        }
        return newFreePosition;
    }

    public bool IsFreeSurroundings(Vector3Int pos)
    {
        bool isAvailable = true;
        for (int i = pos.x - 1; i < pos.x + 1 && isAvailable; i++)
        {
            for (int j = pos.z - 1; j < pos.z + 1 && isAvailable; j++)
            {
                isAvailable = !IsWallAt(i, j);
            }
        }
        return isAvailable;
    }




    public bool CheckWallsCollision(Vector3Int snakeHead)
    {
        return IsWallAt(snakeHead.x, snakeHead.z);
    }

    public bool CheckFruitCollision(Vector3Int snakeHead)
    {
        return IsFruitAt(snakeHead.x, snakeHead.z);
    }

    public bool CheckSnakeCollision(Vector3Int snakeHead)
    {
        return IsSnakeAt(snakeHead.x, snakeHead.z);
    }
}
