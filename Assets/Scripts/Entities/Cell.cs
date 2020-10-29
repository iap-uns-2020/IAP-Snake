using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
    private Entity snake, wall, fruit;

    public Entity Snake { get => snake; set => snake = value; }
    public Entity Wall { get => wall; set => wall = value; }
    public Entity Fruit { get => fruit; set => fruit = value; }

    public bool HasWall()
    {
        return wall != null;
    }
    public bool HasSnake()
    {
        return snake != null;
    }
    public bool HasFruit()
    {
        return fruit != null;
    }
}

