using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [System.Serializable]
    public class Cell         //Defenição da classe da celulas
    {
        public bool visited;
        public GameObject north;                //1
        public GameObject east;                 //2
        public GameObject west;                 //3
        public GameObject south;                //4
    }

    public GameObject wall;                //Parede prefab
    public float wallLength = 1.0f;        //Dimensão da parede
    public int xSize = 5;                  //largura
    public int ySize = 5;                  //altura
    public GameObject trigger;
    GameObject TriggerObject;
    private Vector3 initialPos;
    public GameObject player;
    private GameObject wallHolder,EnemyStuff;
    private Maze.Cell[] cells;
    int currentCell = 0;
    private int totalCells;
    private int visitedCells;
    private bool startedBuilding = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;
    public GameObject Enemy, Target;


    // Start is called before the first frame update
    void Start()
    {
       CreateWalls();


    }

    void CreateWalls()           //Cria as paredes com base nas dimensões dadas pelo utilizador
    {
        wallHolder = new GameObject();
        wallHolder.name = "Maze";
        EnemyStuff = new GameObject();
        initialPos = new Vector3((-xSize / 2) + wallLength / 2, 0.0f, (-ySize*wallLength / 2) + wallLength / 2);
        player.transform.position = new Vector3(xSize/2,1,initialPos.z);
        Physics.SyncTransforms();
        //Esta posição inicial obtem a parede mais "baixa" e mais "á esquerda" através do posição (0,0)
        Vector3 myPos = initialPos;
        GameObject tempWall;  //variavel auxiliar e cosmetica

        //Para o eixo X

        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j <= xSize; j++)     //normalmente o loop gera um conjunto de paredes a mais , por isso paramo-lo um numero antes
            {
                myPos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, 0.0f, initialPos.z + (i * wallLength) - wallLength / 2);
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
                if (i == 8 &&j==0)
                {
                    TriggerObject = Instantiate(trigger, new Vector3(0, 0, myPos.z), Quaternion.identity);
                    TriggerObject.GetComponent<Maze2>().startPosY = ySize * wallLength / 2;
                    TriggerObject.GetComponent<Maze2>().previousMaze = wallHolder;
                    TriggerObject.GetComponent<Maze2>().prevEnemy = EnemyStuff;
                    Debug.Log("Trigger");
                }
            } 
        }

        //Para o eixo Y

        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        CreateCells();

    }

    void CreateCells()
    {
        lastCells = new List<int>();
        lastCells.Clear();

        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Maze.Cell[xSize * ySize];
        int eastWestProcess = 0;
        int childProcess = 0;
        int termCount = 0;
        totalCells = xSize * ySize;



        //Obtem todos os filhos 
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }

        //Assina paredes ás quartos

        for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++)
        {
            if (termCount == xSize)
            {
                eastWestProcess++;
                termCount = 0;
            }

            cells[cellprocess] = new Maze.Cell();
            cells[cellprocess].east = allWalls[eastWestProcess];
            cells[cellprocess].south = allWalls[childProcess + (xSize + 1) * ySize];

            eastWestProcess++;


            termCount++;
            childProcess++;
            cells[cellprocess].west = allWalls[eastWestProcess];
            cells[cellprocess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];
        }
        Debug.Log(allWalls.Length+" "+cells.Length);

        TriggerObject.GetComponent<Maze2>().edge = allWalls;

        CreateMaze();
    }

    void CreateMaze()
    {

        while (visitedCells < totalCells)           //Se há células por visitar
        {
            if (startedBuilding)                    //Se a construção já começou
            {
                GiveMeNeighbour();                  //Procura vizinhos existentes
                if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true)    //Se não visitou o vizinho
                {
                    BreakWall();                  //Destroi a parede entre si e o vizinho
                    cells[currentNeighbour].visited = true;          //Marca o vizinho como visitado
                    visitedCells++;
                    lastCells.Add(currentCell);                      //Marca a celula onde estava como "Celula anterior"
                    currentCell = currentNeighbour;                  //Marca o vizinho escolhido como celula atual

                    if (lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;            //remove a celula anterior á nova celula anterior
                    }
                }
            }
            else
            {
                currentCell = Random.Range(0, totalCells);            //Escolhe uma celula aleatória para ser a celula 
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;                                //Inicia a construção
            }
        }

        int r = Random.Range(0, xSize);
        Destroy(cells[xSize * (ySize - 1)+r].north);
        int g=r;
        while (g - 1 == r || g + 1 == r || g == r)
        {
            g = Random.Range(0, xSize);
        }
        int j = r;
        while (j - 1 == g || j + 1 == g || j == g || j - 1 == r || j + 1 == r || j == r)
        {
            j = Random.Range(0, xSize);
        }
        Destroy(cells[xSize * (ySize - 1) + g].north);
        Destroy(cells[xSize * (ySize - 1) + j].north);

        Debug.Log("Finished");
        CreateEnemies();
    }

    void CreateEnemies()
    {
        int h = 0;
        for(int i = 5; i <= ySize; i += 5)
        {
            if (Random.Range(0, 2) == 5)
            {
                int sp = xSize / 3;

                for (int j = 1; j < 3; j++) {

                    h++;

                    int x = Random.Range(-1, 2);
                    int y = Random.Range(-1, 2);

                    Vector3 enemypos= Instantiate(Enemy, new Vector3(initialPos.x+((sp*j+x)*wallLength), 0, initialPos.z + ((i + y)*wallLength)-wallLength/2), Quaternion.identity).transform.position;


                    for (int g = 0; g < 2; g++)
                    {
                        x = Random.Range(-1, 2);
                        y = Random.Range(-1, 2);

                        GameObject sphere= Instantiate(Target, new Vector3(enemypos.x + (x * wallLength), 0, enemypos.z + (y * wallLength)), Quaternion.identity);
                        Debug.Log(h + " " + sphere.transform.position + " " + enemypos);

                    }

                }
            }
            else
            {
                int x = Random.Range(-2, 3);
                int y = Random.Range(-2, 3);

                GameObject enemypos = Instantiate(Enemy, new Vector3(initialPos.x + ((xSize / 2 + x) * wallLength), 0, initialPos.z + ((i + y) * wallLength) - wallLength / 2), Quaternion.identity);
                enemypos.transform.parent = EnemyStuff.transform;

                for (int g = 0; g < 2; g++)
                {
                    x = Random.Range(-1, 2);
                    y = Random.Range(-1, 2);

                    GameObject sphere = Instantiate(Target, new Vector3(enemypos.transform.position.x + (x * wallLength), 0, enemypos.transform.position.z + (y * wallLength)), Quaternion.identity);
                    sphere.transform.parent = EnemyStuff.transform;
                    if (g == 0)
                        enemypos.GetComponent<Enemie>().Target1 = sphere.transform;
                    else
                        enemypos.GetComponent<Enemie>().Target2 = sphere.transform;
                }
            }




        }


    }

    void BreakWall()
    {
        switch (wallToBreak)
        {
            case 1: Destroy(cells[currentCell].north); break;
            case 2: Destroy(cells[currentCell].east); break;
            case 3: Destroy(cells[currentCell].west); break;
            case 4: Destroy(cells[currentCell].south); break;
        }
    }

    void GiveMeNeighbour()
    {

        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];

        int check = 0;                                            //conjunto de variáveis para verificar se a celula está num canto (ultimo elemento de uma 
        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;

        //Oeste
        if (currentCell + 1 < totalCells && (currentCell + 1) != check)        //verifica se a celula está no canto e se o numero não excede 
        {
            if (cells[currentCell + 1].visited == false)         //verifica se o vizinho já foi visitado
            {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;
            }
        }

        //Este
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }

        //Norte 
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }

        //Sul
        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbours[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;
            }
        }

        //for (int i = 0; i<length; i++)
        //{
        //    Debug.Log(neighbours[i]);
        //}

        if (length != 0)                               //Se um vizinho foi encontrado
        {
            int chosen = Random.Range(0, length);         
            currentNeighbour = neighbours[chosen];       //assinalámos celula como vizinho
            wallToBreak = connectingWall[chosen];        //marca a parede entre a celula e o vizinho para ser destruida
        }
        else                                            //Se vizinho não é encontrado
        {
            if (backingUp > 0)                              //Começa a retroceder no seu caminho
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }

    }

}










