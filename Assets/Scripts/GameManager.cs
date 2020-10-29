using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject wallGraphic, snakeGraphic, fruitGraphic;
    private List<GameObject> snakeGraphicParts; //lista de partes de snake (cubitos)
    public GameObject panelPerdiste;
    private GameObject laFrutita;
    private bool gotFrutita; //indica que agarró una frutita para agrandar la cola
    public GameObject camera;
    private bool endOfGame = false;
    private Vector3Int ultimaPosicion;


    public GameObject panelAndroid;



    private WorldMap worldMap;
    private Snake snake;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = new Player();
        InitializeWorldMap();
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

    private void InitializeWorldMap()
    {
        worldMap = new WorldMap();
        worldMap.InitializeBoard();
    }

    private void InitializeSnake()
    {
        snake = new Snake();
        
        snakeGraphicParts = new List<GameObject>();
        CreateSnakeFirstPosition();

        
    }

    private void InitializeWalls()
    {
        CreateBorderWalls();
        CreateRandomWalls();
    }

    // Update is called once per frame
    void Update()
    {
        player.Update(this);

        MoveCamera();
    
    }

    private void MoveCamera()
    {
        Vector3Int snakeHead = snake.GetHead();
        snakeHead.z -= 15;
        snakeHead.y = 20;
        camera.transform.position = Vector3.Lerp(camera.transform.position, snakeHead, 1 * Time.deltaTime);
    }

    public void StartExecution()
    {
        Invoke("Execute", player.GetSpeed());
        player.StartMoving();
    }

    private void FreeSnakeTailPosition()
    {
        worldMap.FreeSnakePartAt(snake.GetTail().x,snake.GetTail().z);
    }



    private void UpdateSnakeHeadPosition(Vector3Int pos)
    {
        snake.UpdateHeadPosition(pos);
        worldMap.PlaceSnakePartAt(snake.GetHead().x, snake.GetHead().z);
    }

    private void UpdateSnakeGraphicsWhileMoving()
    {
        for (int i = 0; i < snakeGraphicParts.Count; i++)
        {
            snakeGraphicParts[i].transform.position = snake.GetSnakePartPosition(i);
        }
    }

    private void Execute()
    {
        ultimaPosicion = snake.GetTail();
        FreeSnakeTailPosition();
        snake.UpdateSnakePositionsWhileMoving();
        Vector3Int pos = snake.GetHead();

        if (player.IsRight())
        {
            pos.x += 1;
        }
        else if (player.IsUp())
        {
            pos.z += 1;
        }
        else if (player.IsLeft())
        {
            pos.x -= 1;
        }
        else if (player.IsDown())
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
        if(!endOfGame)Invoke("Execute", player.GetSpeed());
    }

    private void AgregarElementoGraficoAlFinalDeLaColaDeLaSerpiente()
    {
        //ver que onda la ultima posicion
        snake.UpdateTailPosition(ultimaPosicion);
        GameObject nuevoCubito = Instantiate(snakeGraphic, ultimaPosicion, transform.rotation);
        snakeGraphicParts.Insert(0, nuevoCubito);

        worldMap.PlaceSnakePartAt(ultimaPosicion.x, ultimaPosicion.z);
    }


   

    private void DestroyFruit()
    {
        int r = (int)laFrutita.transform.position.x;
        int c = (int)laFrutita.transform.position.z;
        worldMap.FreeFruitAt(r, c);
        Destroy(laFrutita);
    }

    private void CreateNewFruit()
    {
        InitializeFruits();
        gotFrutita = true;
    }

    public void CheckCollisions()
    {
        if (worldMap.CheckFruitCollision(snake.GetHead()))
        {
            DestroyFruit();
            CreateNewFruit();
        }
        if (worldMap.CheckWallsCollision(snake.GetHead())) //agregar colision con la serpiente
        {
            endOfGame = true;
            panelPerdiste.SetActive(true);
        }
    }

    

    private void CreateRandomWalls()
    {
        int numberOfRandomWalls = worldMap.RandomWallValue();
        for(int i = 0; i < numberOfRandomWalls; i++)
        {
            Vector3Int nuevaPos = worldMap.GetFreePosition();
            CreateWall(nuevaPos);
        }
    }

    private void CreateBorderWalls()
    {
        for(int i = 0; i < worldMap.GetColumns(); i++)
        {
            CreateWall(new Vector3Int(0, 0, i));
            CreateWall(new Vector3Int(worldMap.GetRows() - 1, 0, i));
        }

        for (int j = 0; j < worldMap.GetRows(); j++)
        {
            CreateWall(new Vector3Int(j, 0, 0));
            CreateWall(new Vector3Int(j, 0, worldMap.GetColumns() - 1));
        }
    }

    private void CreateWall(Vector3Int nuevaPos)
    {
        Instantiate(wallGraphic, nuevaPos, transform.rotation);
        worldMap.PlaceWallAt(nuevaPos.x, nuevaPos.z);
    }

    private void CreateSnakeFirstPosition()
    {
        Vector3Int nuevaPos = worldMap.GetFreePosition();
        AddNewSnakeGraphicPart(nuevaPos);
        UpdateSnakeMatrix(nuevaPos);
    }


    private void UpdateSnakeMatrix(Vector3Int nuevaPos)
    {
        snake.AddNewPart(nuevaPos);
        worldMap.PlaceSnakePartAt(nuevaPos.x, nuevaPos.z);
    }

    private void AddNewSnakeGraphicPart(Vector3Int nuevaPos)
    {
        GameObject nuevoSnakePart = Instantiate(snakeGraphic, nuevaPos, transform.rotation) as GameObject;
        snakeGraphicParts.Add(nuevoSnakePart);
    }

    

    private void InitializeFruits()
    {
        Vector3Int nuevaPos = worldMap.GetFreePosition();
        CreateFruit(nuevaPos);        
    }

    private void CreateFruit(Vector3Int nuevaPos)
    {
        laFrutita = Instantiate(fruitGraphic, nuevaPos, transform.rotation) as GameObject;
        worldMap.PlaceFrutitaAt(nuevaPos.x, nuevaPos.z);
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(0);
    }


    //android part
    /*
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
    */
    public void PressedSpaceUp()
    {
        player.SetSpeed(0.05f);
    }

    public void releasedSpaceUp()
    {
        player.SetSpeed(0.2f);
    }
}
