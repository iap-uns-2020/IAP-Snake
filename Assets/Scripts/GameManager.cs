using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int[,] matrixWalls,matrixSnake,matrixFrutitas;
    public GameObject Walls, snake, frutita;
    private List<Vector3Int> snakePositions; //lista de posiciones
    private List<GameObject> snakeGraphicParts; //lista de partes de snake (cubitos)
    private int currentDirection  = 0; //dirección de movimiento
    public GameObject panelPerdiste;
    private GameObject laFrutita;
    private bool gotFrutita; //indica que agarró una frutita para agrandar la cola
    private Vector3Int ultimaPosicion;
    public GameObject camera;
    public float speed = 0.2f;
    private bool endOfGame = false;

    private const int boardRows = 200;
    private const int boardCols = 10;
    private const int minNumberOfWalls = 1;
    private const int maxNumberOfWalls = 200;

    private const int RIGHT = 1;
    private const int UP = 2;
    private const int LEFT = 3;
    private const int DOWN = 4;

    public GameObject panelAndroid;

    private SnakeState snakeState;

    // Start is called before the first frame update
    void Start()
    {
        InitializeBoard();
        InitializeWalls();
        InitializeSnake();
        InitializeFruits();
    }

    private void CheckPlatform()
    {
        //muestro controles solo si estoy en android
        #if UNITY_EDITOR
                Debug.Log("Unity Editor");
        #elif UNITY_ANDROID
                panelAndroid.SetActive(true);
        #else
                Debug.Log("Any other platform");
        #endif
    }

    private void InitializeBoard()
    {
        matrixWalls = new int[boardRows, boardCols];
        matrixSnake = new int[boardRows, boardCols];
        matrixFrutitas = new int[boardRows, boardCols];
    }

    private void InitializeSnake()
    {
        snakePositions = new List<Vector3Int>();
        snakeGraphicParts = new List<GameObject>();
        CreateSnakeFirstPosition();

        snakeState = new SnakeStopped();
    }

    private void InitializeWalls()
    {
        CreateBorderWalls();
        CreateRandomWalls();
    }

    // Update is called once per frame
    void Update()
    {
        //esto quedaría super lindo
        //hacer un patrón state y tal vez un command
        if (Input.GetKeyDown(KeyCode.D))
        {
            snakeState.Execute(this);
            if(currentDirection !=LEFT) currentDirection  = RIGHT;
        }else if (Input.GetKeyDown(KeyCode.W))
        {
            snakeState.Execute(this);
            if (currentDirection  != DOWN) currentDirection  = UP;
        }else if (Input.GetKeyDown(KeyCode.A))
        {
            snakeState.Execute(this);
            if (currentDirection  != RIGHT) currentDirection  = LEFT;
        }else if (Input.GetKeyDown(KeyCode.S))
        {
            snakeState.Execute(this);
            if (currentDirection  != UP) currentDirection  = DOWN;
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

        MoveCamera();
    
    }

    private void MoveCamera()
    {
        Vector3Int snakeHead = snakePositions[snakePositions.Count - 1];
        snakeHead.z -= 15;
        snakeHead.y = 20;
        camera.transform.position = Vector3.Lerp(camera.transform.position, snakeHead, 1 * Time.deltaTime);
    }

    public void StartExecution()
    {
        Invoke("Execute", speed);
        snakeState = new SnakeMooving();
    }

    private void FreeSnakeTailPosition()
    {
        ultimaPosicion = snakePositions[0];
        matrixSnake[ultimaPosicion.x, ultimaPosicion.z] = 0;
    }

    private void UpdateSnakePositionsWhileMoving()
    {
        for (int i = 0; i < snakePositions.Count - 1; i++)
        {
            snakePositions[i] = snakePositions[i + 1];
        }
    }

    private void UpdateSnakeHeadPosition(Vector3Int pos)
    {
        snakePositions[snakePositions.Count - 1] = pos;
        matrixSnake[snakePositions[snakePositions.Count - 1].x, snakePositions[snakePositions.Count - 1].z] = 1;
    }

    private void UpdateSnakeGraphicsWhileMoving()
    {
        for (int i = 0; i < snakeGraphicParts.Count; i++)
        {
            snakeGraphicParts[i].transform.position = snakePositions[i];
        }
    }

    private void Execute()
    {
        FreeSnakeTailPosition();
        UpdateSnakePositionsWhileMoving();
        Vector3Int pos = snakePositions[snakePositions.Count - 1];

        if (currentDirection  == RIGHT)
        {
            pos.x += 1;
        }
        else if (currentDirection  == UP)
        {
            pos.z += 1;
        }
        else if (currentDirection  == LEFT)
        {
            pos.x -= 1;
        }
        else if (currentDirection  == DOWN)
        {
            pos.z-= 1;
        }

        CheckCollisions();

        UpdateSnakeHeadPosition(pos);
        UpdateSnakeGraphicsWhileMoving();
        

        //si agarró frutita se añade un pedaso más a la cola de la serpiente
        if (gotFrutita)
        {
            gotFrutita = false;
            AgregarElementoGraficoAlFinalDeLaColaDeLaSerpiente();
        }

        //vuelve a llamar a ejecutar 
        if(!endOfGame)Invoke("Execute", speed);
    }

    private void AgregarElementoGraficoAlFinalDeLaColaDeLaSerpiente()
    {
        snakePositions.Insert(0, ultimaPosicion);
        GameObject nuevoCubito = Instantiate(snake, ultimaPosicion, transform.rotation);
        snakeGraphicParts.Insert(0, nuevoCubito);

        matrixSnake[ultimaPosicion.x, ultimaPosicion.z] = 1;
    }


    private void CheckCollisions()
    {
        if (CheckFruitCollision())
        {
            DestroyFruit();
            CreateNewFruit();
        }
        if (CheckWallsCollision()) //agregar colision con la serpiente
        {
            endOfGame = true;
            panelPerdiste.SetActive(true);
        }
        
    }

    private bool CheckWallsCollision()
    {
        return (matrixWalls[snakePositions[snakePositions.Count - 1].x, snakePositions[snakePositions.Count - 1].z] == 1);
    }

    private bool CheckFruitCollision()
    {
        return (matrixFrutitas[snakePositions[snakePositions.Count - 1].x, snakePositions[snakePositions.Count - 1].z] == 1);

    }

    private void DestroyFruit()
    {
        matrixFrutitas[(int)laFrutita.transform.position.x, (int)laFrutita.transform.position.z] = 0;
        Destroy(laFrutita);
    }

    private void CreateNewFruit()
    {
        InitializeFruits();
        gotFrutita = true;
    }

    private bool CheckSnakeCollision()
    {
        return (matrixSnake[snakePositions[snakePositions.Count - 1].x, snakePositions[snakePositions.Count - 1].z] == 1);
    }

    private void CreateRandomWalls()
    {
        int numberOfRandomWalls = Random.Range(minNumberOfWalls,maxNumberOfWalls);
        for(int i = 0; i < numberOfRandomWalls; i++)
        {
            Vector3Int nuevaPos = GetFreePosition();
            CreateWall(nuevaPos);
        }
    }

    private void CreateBorderWalls()
    {
        for(int i = 0; i < boardCols; i++)
        {
            CreateWall(new Vector3Int(0, 0, i));
            CreateWall(new Vector3Int(boardRows - 1, 0, i));
        }

        for (int j = 0; j < boardRows; j++)
        {
            CreateWall(new Vector3Int(j, 0, 0));
            CreateWall(new Vector3Int(j, 0, boardCols - 1));
        }
    }

    private void CreateWall(Vector3Int nuevaPos)
    {
        Instantiate(Walls, nuevaPos, transform.rotation);
        Debug.Log(nuevaPos);
        matrixWalls[nuevaPos.x, nuevaPos.z] = 1;
    }

    private void CreateSnakeFirstPosition()
    {
        Vector3Int nuevaPos = GetFreePosition();
        AddNewSnakeGraphicPart(nuevaPos);
        UpdateSnakeMatrix(nuevaPos);
    }


    private void UpdateSnakeMatrix(Vector3Int nuevaPos)
    {
        snakePositions.Add(nuevaPos);
        matrixSnake[nuevaPos.x, nuevaPos.z] = 1;
    }

    private void AddNewSnakeGraphicPart(Vector3Int nuevaPos)
    {
        GameObject nuevoSnakePart = Instantiate(snake, nuevaPos, transform.rotation) as GameObject;
        snakeGraphicParts.Add(nuevoSnakePart);
    }

    private Vector3Int GetFreePosition()
    {
        bool isAvailable = false;
        Vector3Int newFreePosition = new Vector3Int(0, 0, 0);
        while (!isAvailable)
        {
            newFreePosition = new Vector3Int(Random.Range(1, boardRows), 0, Random.Range(1, boardCols));
            isAvailable = IsFreeSurroundings(newFreePosition);
        }
        return newFreePosition;
    }

    private bool IsFreeSurroundings(Vector3Int pos)
    {
        bool isAvailable = true;
        for(int i = pos.x - 1; i< pos.x+1 && isAvailable ; i++)
        {
            for(int j = pos.z - 1; j < pos.z + 1 && isAvailable; j++)
            {
                isAvailable = matrixWalls[i,j] != 1;
            }
        }
        return isAvailable;
    }

    private void InitializeFruits()
    {
        Vector3Int nuevaPos = GetFreePosition();
        CreateFruit(nuevaPos);        
    }

    private void CreateFruit(Vector3Int nuevaPos)
    {
        laFrutita = Instantiate(frutita, nuevaPos, transform.rotation) as GameObject;
        matrixFrutitas[nuevaPos.x, nuevaPos.z] = 1;
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(0);
    }


    //android part

    public void pressedUp()
    {
        if (currentDirection  == 0) StartExecution();
        if (currentDirection  != 4) currentDirection  = 2;
    }

    public void pressedDown()
    {
        if (currentDirection  == 0) StartExecution();
        if (currentDirection  != 2) currentDirection  = 4;
    }

    public void PressedLeft()
    {
        if (currentDirection  == 0) StartExecution();
        if (currentDirection  != 1) currentDirection  = 3;
    }

    public void PressedRight()
    {
        if (currentDirection  == 0) StartExecution();
        if (currentDirection  != 3) currentDirection  = 1;
    }

    public void PressedSpaceUp()
    {
        speed = 0.05f;
    }

    public void releasedSpaceUp()
    {
        speed = 0.2f;
    }
}
