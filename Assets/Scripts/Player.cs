using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
    private SnakeState snakeState;
    private const int RIGHT = 1;
    private const int UP = 2;
    private const int LEFT = 3;
    private const int DOWN = 4;
    private int currentDirection = 0;
    public float speed = 0.2f;

    public Player()
    {
        snakeState = new SnakeStopped();
    }

    public void StartMoving()
    {
        snakeState = new SnakeMooving();
    }



    public void Update(GameManager gm)
    {
        //esto quedaría super lindo
        //hacer un patrón state y tal vez un command
        if (Input.GetKeyDown(KeyCode.D))
        {
            snakeState.Execute(gm);
            if (currentDirection != LEFT) currentDirection = RIGHT;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            snakeState.Execute(gm);
            if (currentDirection != DOWN) currentDirection = UP;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            snakeState.Execute(gm);
            if (currentDirection != RIGHT) currentDirection = LEFT;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            snakeState.Execute(gm);
            if (currentDirection != UP) currentDirection = DOWN;
        }
        //mientras se presiona el shift izquierdo, se incrementa la velocidad
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 0.05f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 0.2f;
        }
    }

    public bool IsRight()
    {
        return currentDirection == RIGHT;
    }
    public bool IsUp()
    {
        return currentDirection == UP;
    }
    public bool IsLeft()
    {
        return currentDirection == LEFT;
    }
    public bool IsDown()
    {
        return currentDirection == DOWN;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
