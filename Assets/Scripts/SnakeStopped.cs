using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeStopped : SnakeState
{
    public void Execute(GameManager gameManager)
    {
        gameManager.StartExecution();
    }
}
