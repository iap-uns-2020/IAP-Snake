using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake 
{
    private List<Vector3Int> snakePositions;
    //private Vector3Int ultimaPosicion;

    public Snake()
    {
        snakePositions = new List<Vector3Int>();
    }

    public Vector3Int GetHead()
    {
        return snakePositions[snakePositions.Count - 1];
    }

    public Vector3Int GetTail()
    {
        return snakePositions[0];
    }

    public int GetLength()
    {
        return snakePositions.Count;
    }

    public void UpdateSnakePositionsWhileMoving()
    {
        for (int i = 0; i < snakePositions.Count - 1; i++)
        {
            snakePositions[i] = snakePositions[i + 1];
        }
    }

    public void UpdateHeadPosition(Vector3Int newPosition)
    {
        snakePositions[snakePositions.Count - 1] = newPosition;
    }

    public void UpdateTailPosition(Vector3Int newPosition)
    {
        snakePositions.Insert(0, newPosition);
    }

    public Vector3Int GetSnakePartPosition(int i)
    {
        return snakePositions[i];
    }

    public void AddNewPart(Vector3Int nuevaPos)
    {
        snakePositions.Add(nuevaPos);
    }
}
