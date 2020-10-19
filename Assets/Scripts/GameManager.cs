using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int[,] matrixWalls,matrixSnake,matrixFrutitas;
    public GameObject Walls, snake, frutita;
    private List<Vector3Int> snakeList; //lista de posiciones
    private List<GameObject> snakeParts; //lista de partes de snake (cubitos)
    private int d  = 0; //dirección de movimiento
    public GameObject panelPerdiste;
    private GameObject laFrutita;
    private bool gotFrutita; //indica que agarró una frutita para agrandar la cola
    private Vector3Int ultimaPosicion;
    public GameObject camera;
    public float speed = 0.2f;
    private bool endOfGame = false;

    public GameObject panelAndroid;

    // Start is called before the first frame update
    void Start()
    {
        matrixWalls = new int[50,50];
        matrixSnake = new int[50, 50];
        matrixFrutitas = new int[50, 50];
        snakeList = new List<Vector3Int>();
        snakeParts = new List<GameObject>();
        createBorderWalls();
        CreateRandomWalls();
        createSnake();
        CrearFrutita();

        //muestro controles solo si estoy en android
        #if UNITY_EDITOR
                Debug.Log("Unity Editor");
        #elif UNITY_ANDROID
                    panelAndroid.SetActive(true);
        #else
                    Debug.Log("Any other platform");
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (d  == 0) comenzar();
            if(d !=3) d  = 1;
        }else if (Input.GetKeyDown(KeyCode.W))
        {
            if (d  == 0) comenzar();
            if (d  != 4) d  = 2;
        }else if (Input.GetKeyDown(KeyCode.A))
        {
            if (d  == 0) comenzar();
            if (d  != 1) d  = 3;
        }else if (Input.GetKeyDown(KeyCode.S))
        {
            if (d  == 0) comenzar();
            if (d  != 2) d  = 4;
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


        Vector3Int posCabeza = snakeList[snakeList.Count - 1];
        posCabeza.z -= 15;
        posCabeza.y = 20;
        //camera.transform.position = posCabeza;
        //mueve la camara para mirar a la cabeza de la serpiente
        camera.transform.position = Vector3.Lerp(camera.transform.position, posCabeza, 1 * Time.deltaTime);

    

    }

    void comenzar()
    {
        Invoke("ejecutar",speed);
    }

    void ejecutar()
    {
        if (d  == 1)
        {
 
            ultimaPosicion = snakeList[0];

            matrixSnake[ultimaPosicion.x, ultimaPosicion.z] = 0;

            for(int i = 0; i < snakeList.Count-1; i++)
            {
                snakeList[i] = snakeList[i+1];
            }

            Vector3Int pos = snakeList[snakeList.Count - 1];
            pos.x += 1;
            snakeList[snakeList.Count - 1]= pos;

            for(int i = 0; i < snakeParts.Count; i++)
            {
                snakeParts[i].transform.position = snakeList[i];
            }

            chequearColision();

            matrixSnake[snakeList[snakeList.Count - 1].x, snakeList[snakeList.Count - 1].z] = 1;

        }
        else if (d  == 2)
        {
            ultimaPosicion = snakeList[0];
            matrixSnake[ultimaPosicion.x, ultimaPosicion.z] = 0;


            for (int i = 0; i < snakeList.Count - 1; i++)
            {
                snakeList[i] = snakeList[i + 1];
            }

            Vector3Int pos = snakeList[snakeList.Count - 1];
            pos.z += 1;
            snakeList[snakeList.Count - 1] = pos;

            for (int i = 0; i < snakeParts.Count; i++)
            {
                snakeParts[i].transform.position = snakeList[i];
            }

            chequearColision();

            matrixSnake[snakeList[snakeList.Count - 1].x, snakeList[snakeList.Count - 1].z] = 1;
        }
        else if (d  == 3)
        {
            ultimaPosicion = snakeList[0];
            matrixSnake[ultimaPosicion.x, ultimaPosicion.z] = 0;


            for (int i = 0; i < snakeList.Count - 1; i++)
            {
                snakeList[i] = snakeList[i + 1];
            }

            Vector3Int pos = snakeList[snakeList.Count - 1];
            pos.x -= 1;
            snakeList[snakeList.Count - 1] = pos;

            for (int i = 0; i < snakeParts.Count; i++)
            {
                snakeParts[i].transform.position = snakeList[i];
            }

            chequearColision();

            matrixSnake[snakeList[snakeList.Count - 1].x, snakeList[snakeList.Count - 1].z] = 1;
        }
        else if (d  == 4)
        {
            ultimaPosicion = snakeList[0];
            matrixSnake[ultimaPosicion.x, ultimaPosicion.z] = 0;


            for (int i = 0; i < snakeList.Count - 1; i++)
            {
                snakeList[i] = snakeList[i + 1];
            }

            Vector3Int pos = snakeList[snakeList.Count - 1];
            pos.z-= 1;
            snakeList[snakeList.Count - 1] = pos;

            for (int i = 0; i < snakeParts.Count; i++)
            {
                snakeParts[i].transform.position = snakeList[i];
            }

            chequearColision();

            matrixSnake[snakeList[snakeList.Count - 1].x, snakeList[snakeList.Count - 1].z] = 1;
        }

        //si agarró frutita se añade un pedaso más a la cola de la serpiente
        if (gotFrutita)
        {
            gotFrutita = false;
            AgregarElementoALaCola();
        }

        //vuelve a llamar a ejecutar 
        if(!endOfGame)Invoke("ejecutar", speed);
    }

    void AgregarElementoALaCola()
    {
        snakeList.Insert(0, ultimaPosicion);
        GameObject nuevoCubito = Instantiate(snake, ultimaPosicion, transform.rotation);
        snakeParts.Insert(0, nuevoCubito);

        matrixSnake[ultimaPosicion.x, ultimaPosicion.z] = 1;
    }


    void chequearColision()
    {
        //si colisiona con una pared se termina la partida
        if (matrixWalls[snakeList[snakeList.Count - 1].x, snakeList[snakeList.Count - 1].z] == 1)
        {
            endOfGame = true;
            panelPerdiste.SetActive(true);
        }
        //si colisiona con una frutita se crea otra frutita
        if (matrixFrutitas[snakeList[snakeList.Count - 1].x, snakeList[snakeList.Count - 1].z] == 1)
        {
            matrixFrutitas[(int)laFrutita.transform.position.x, (int)laFrutita.transform.position.z] = 0;
            Destroy(laFrutita);
            CrearFrutita();
            gotFrutita = true;
        }
        if (matrixSnake[snakeList[snakeList.Count - 1].x, snakeList[snakeList.Count - 1].z] == 1)
        {
            endOfGame = true;
            panelPerdiste.SetActive(true);
        }
    }

    void CreateRandomWalls()
    {
        int cantidad = Random.Range(100,200);
        for(int i = 0; i < cantidad; i++)
        {
            bool estaOcupado = true;
            Vector3Int nuevaPos = new Vector3Int(Random.Range(0, 50), 0, Random.Range(0, 50));
            while (estaOcupado)
            {
                nuevaPos = new Vector3Int(Random.Range(0, 50), 0, Random.Range(0, 50));
                estaOcupado = matrixWalls[nuevaPos.x, nuevaPos.z] == 1;
            }
            Instantiate(Walls, nuevaPos, transform.rotation);
            matrixWalls[nuevaPos.x, nuevaPos.z] = 1;
        }
    }

    void createBorderWalls()
    {
        for(int i = 0; i < matrixWalls.GetLength(0); i++)
        {
            Vector3Int nuevaPos = new Vector3Int(0, 0, i);
            Instantiate(Walls, nuevaPos, transform.rotation);
            matrixWalls[nuevaPos.x, nuevaPos.z] = 1;
            nuevaPos = new Vector3Int(matrixWalls.GetLength(0)-1, 0, i);
            Instantiate(Walls, nuevaPos, transform.rotation);
            //Debug.Log(nuevaPos);
            matrixWalls[nuevaPos.x, nuevaPos.z] = 1;
            nuevaPos = new Vector3Int(i, 0, 0);
            Instantiate(Walls, nuevaPos, transform.rotation);
            matrixWalls[nuevaPos.x, nuevaPos.z] = 1;
            nuevaPos = new Vector3Int(i, 0, matrixWalls.GetLength(0)-1);
            Instantiate(Walls, nuevaPos, transform.rotation);
            matrixWalls[nuevaPos.x, nuevaPos.z] = 1;
        }
    }

    void createSnake()
    {
        bool ocupado = true;
        Vector3Int nuevaPos=new Vector3Int(0,0,0);
        while (ocupado)
        {
            nuevaPos = new Vector3Int(Random.Range(1, matrixSnake.GetLength(0)-1), 0, Random.Range(1, matrixSnake.GetLength(0) - 1));
            ocupado = matrixWalls[nuevaPos.x, nuevaPos.z] == 1 || matrixWalls[nuevaPos.x - 1, nuevaPos.z] == 1 || matrixWalls[nuevaPos.x + 1, nuevaPos.z] == 1 || matrixWalls[nuevaPos.x, nuevaPos.z - 1] == 1 || matrixWalls[nuevaPos.x, nuevaPos.z + 1] == 1;
        }
        GameObject nuevoSnakePart = Instantiate(snake, nuevaPos, transform.rotation)as GameObject;
        snakeList.Add(nuevaPos);
        snakeParts.Add(nuevoSnakePart);
        matrixSnake[nuevaPos.x, nuevaPos.z] = 1;
    }

    void CrearFrutita()
    {
        bool ocupado = true;
        Vector3Int nuevaPos = new Vector3Int(0, 0, 0);
        while (ocupado)
        {
            nuevaPos = new Vector3Int(Random.Range(1, matrixFrutitas.GetLength(0) - 1), 0, Random.Range(1, matrixFrutitas.GetLength(0) - 1));
            ocupado = matrixWalls[nuevaPos.x, nuevaPos.z] == 1 || matrixSnake[nuevaPos.x, nuevaPos.z] == 1;
        }
        laFrutita=Instantiate(frutita, nuevaPos, transform.rotation) as GameObject;
        matrixFrutitas[nuevaPos.x, nuevaPos.z] = 1;
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(0);
    }


    //android part

    public void pressedUp()
    {
        if (d  == 0) comenzar();
        if (d  != 4) d  = 2;
    }

    public void pressedDown()
    {
        if (d  == 0) comenzar();
        if (d  != 2) d  = 4;
    }

    public void PressedLeft()
    {
        if (d  == 0) comenzar();
        if (d  != 1) d  = 3;
    }

    public void PressedRight()
    {
        if (d  == 0) comenzar();
        if (d  != 3) d  = 1;
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
