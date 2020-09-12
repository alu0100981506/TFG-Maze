using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


namespace Maze
{
    public class MazeAreaWilson : Area
    {
      
        [System.Serializable]
        public class Cell
        {
            public bool visited; //pertenencia de la celda al laberinto en construcción
            public int north; //Indicadores de pared o borde (0 libre, -1 borde, 1 pared)
            public int east;
            public int west;
            public int south;
            public int exit; //flag de pertenencia al path (-1 es no visitado) y de ruta de salida (0, 1, 2, 3 = N, W, E, S)

            public Cell()
            {
                visited = false;
                north = 1; // Paredes por defecto
                east = 1;
                west = 1;
                south = 1;
                exit = -1; //flag de pertenencia al path y de ruta de salida (0, 1, 2, 3 = N, W, E, S)
            }
        }
        public char[,] mazeInt;
        public GameObject agent;
        public GameObject wall;
        public GameObject floor;
        public GameObject final;
        public GameObject start;
        public int whereStart = 0;
        public int whereFinal = 0;
        public float WallLenght = 1f;
        public int xSize = 5;
        public int ySize = 5;
        public bool PerfectMaze = true;
        public int PercentWallEraser = 20;
        private Vector3 initialPos;
        private GameObject mazeHolder;
        private Cell[] cells;
    //    private int currentCell = 0;
    //    private int totalCells;
    //    private int visitedCells = 0;
    //    private bool startedBuilding = false;
    //    private int currentNeighbour = 0;
    //    private List<int> lastCells;
    //    private int backingUp = 0;
    //    private int wallToBreak = 0;
        private Vector3 medPos;
        public float wallLength = 1.0f;
        public float wallWidth = 0.1f;
        public float floorWidth = 0.1f;


        [HideInInspector]
        public Vector3 whereStart3;
        [HideInInspector]
        public Vector3 whereFinal3;
        [HideInInspector]
        public Vector3 whereStart300;
        [HideInInspector]
        public MazeAcademy academy;
        void Start()
        {
            //CreateWalls();
            academy = FindObjectOfType<MazeAcademy>();
            mazeCreator();
        }

        void mazeCreator()
        {

            InitCells();
            BuildWilsonMaze();
            //CreateStartFinal();
            CreateMaze();
            CreateStartFinal();
        }

        void CreateWalls()
        {
            Vector3 scaleChange = new Vector3(wallWidth, 1.0f, wallLength + wallWidth);
            wall.transform.localScale = scaleChange;
            mazeHolder = new GameObject();
            mazeHolder.name = "Maze";
            initialPos = new Vector3(-xSize * wallLength / 2.0f, 0.0f, -ySize * wallLength / 2.0f);
            Vector3 myPos = initialPos;
            GameObject tempWall;

            //x Axis
            for (int i = 0; i < ySize; i++) { //cambia filas (recorre columnas) 
                for (int j = 0; j <= xSize; j++) {
                    myPos = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) + wallLength / 2);
                    tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                    tempWall.transform.parent = mazeHolder.transform;

                }
            }

            //y Axis
            for (int i = 0; i <= ySize; i++) {
                for (int j = 0; j < xSize; j++) {
                    myPos = new Vector3(initialPos.x + (j * wallLength) + wallLength / 2, 0.0f, initialPos.z + (i * wallLength));
                    tempWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                    tempWall.transform.parent = mazeHolder.transform;
                }
            }

        }

        void InitCells()
        {
            cells = new Cell[xSize * ySize];
            //Datos de cada celda vienen del constructor (existen todas las paredes) y solo se añaden bordes 
            for (int i = 0; i < ySize; i++) {
                for (int j = 0; j < xSize; j++) {
                    cells[i * xSize + j] = new Cell();
                }
            }
            //Bordes inf y sup
            for (int i = 0; i < xSize; i++) {
                cells[i].south = -1;
                cells[xSize * (ySize - 1) + i].north = -1;
            }
            //Bordes izq y dcho
            for (int i = 0; i < ySize; i++) {
                cells[i * xSize].west = -1;
                cells[i * xSize + (xSize - 1)].east = -1;
            }
        }

        int Neighbor(int origin, int dir)
        { //Devuelve indice de vecino de origin en direcc dir salvo si es borde , que dev -1
            switch (dir) {
                case 0: //north
                    if (cells[origin].north != -1) {
                        return origin + xSize;
                    }
                    break;
                case 1: //west
                    if (cells[origin].west != -1) {
                        return origin - 1;
                    }
                    break;
                case 2: //east
                    if (cells[origin].east != -1) {
                        return origin + 1;
                    }
                    break;
                case 3: //south
                    if (cells[origin].south != -1) {
                        return origin - xSize;
                    }
                    break;
                default:
                    Debug.Log("Dirección inválida\n");
                    break;
            }
            return -1; //Tropezado con borde
        }

        int NextRandomNeighbor(int origin)
        {
            //Da vecino aleatorio de origin, repitiendo si es borde y guardando el camino seguido
            int dir, next;
            while (true) {
                dir = UnityEngine.Random.Range(0, 4); //Devuelve un número entre 0 y 3
                next = Neighbor(origin, dir);
                if (next != -1) {
                    cells[origin].exit = dir;
                    return next;
                }
            }
        }

        void EraseWalls(int origin, int dir)
        {
            //Borra paredes colindantes entre celdas origin y la encontrada en direcc. dir 
            int dest = Neighbor(origin, dir);
            // Debug.Log("Origin " + origin);

            int xposORI = origin % xSize;
            xposORI *= 2;
            int yposORI = Mathf.FloorToInt(origin / xSize);
            yposORI *= 2;
        //    int xposDEST = dest % xSize;
        //    xposDEST *= 2;
        //    int yposDEST = Mathf.FloorToInt(dest / xSize);
       //     yposDEST *= 2;
            switch (dir) {
                case 0: //north

               //     Debug.Log(" Origen => " + origin + " direccion norte");
                //    Debug.Log("xPos => " + xposORI + " , yPos => " + yposORI);
                  //  Debug.Log(" Destino => " + dest + " direccion sur");

                    cells[origin].north = 0;

                //    if(yposORI < ySize)
                        mazeInt[xposORI, yposORI + 1] = '·';

                    cells[dest].south = 0;

            //        if (yposDEST > 0)
           //             mazeInt[xposDEST, yposDEST - 1] = '·';
                    break;
                case 1: //west
               //     Debug.Log(" Origen => " + origin + " direccion oeste");
               //     Debug.Log("xPos => " + xposORI + " , yPos => " + yposORI);
                    // Debug.Log(" Destino => " + dest + " direccion este");

                    cells[origin].west = 0;
                    
                  //  if(xposORI < xSize)
                        mazeInt[xposORI - 1, yposORI] = '·';

                    cells[dest].east = 0;

            //        if (xposDEST > 0)
            //            mazeInt[xposDEST - 1, yposDEST] = '·';
                    break;
                case 2: //east
               //     Debug.Log(" Origen => " + origin + " direccion este");
               //     Debug.Log("xPos => " + xposORI + " , yPos => " + yposORI);
                    //  Debug.Log(" Destino => " + dest + " direccion oeste");

                    cells[origin].east = 0;

                //    if(xposORI > 0)
                        mazeInt[xposORI + 1, yposORI] = '·';

                    cells[dest].west = 0;

               //     if (xposDEST < xSize)
              //          mazeInt[xposDEST + 1, yposDEST] = '·';
                    break;
                case 3: //south
              //     Debug.Log(" Origen => " + origin + " direccion sur");
              //      Debug.Log("xPos => " + xposORI + " , yPos => " + yposORI);
                    //   Debug.Log(" Destino => " + dest + " direccion norte");


                    cells[origin].south = 0;
//
                    if(yposORI > 0)
                        mazeInt[xposORI, yposORI - 1] = '·';

                    cells[dest].north = 0;
             //       if (yposDEST < ySize)
              //          mazeInt[xposDEST, yposDEST + 1] = '·';
                    break;
                default:
                    Debug.Log("Dirección inválida\n");
                    break;
            }
        }

        void BuildWilsonMaze()
        {

            mazeInt = new char[xSize * 2 - 1, ySize * 2 - 1];
            for (int i = 0; i < xSize * 2 - 1; i++) {
                for (int j = 0; j < ySize * 2 - 1; j++) {
                    mazeInt[i, j] = '·';
                }
            }
            for (int i = 1; i < xSize * 2 - 1; i += 2) {
                for (int j = 0; j < ySize * 2 - 1; j++) {
                    mazeInt[i, j] = '#';
                }
            }
            for (int i = 1; i < ySize * 2 - 1; i += 2) {
                for (int j = 0; j < xSize * 2 - 1; j++) {
                    mazeInt[j, i] = '#';
                }
            }


            //Laberinto inicial, empezamos en celda inferior izq 
            cells[0].visited = true;
            //Elegimos en secuencia la celda donde empieza el random walk, no afecta a la aleatoriedad
            int startCell = 1;
            while (startCell < (xSize * ySize)) {
                if (cells[startCell].visited) { //si ya pertenece al laberinto, seguimos a la siguiente
                    startCell++;
                }
                else { //No pertenece, hacemos random walk hasta llegar a celda que sí pertenezca al lab
                    int next;
                    int current = startCell;
                    do {
                        next = NextRandomNeighbor(current); //Vecino aleatorio y marca camino salida
                        if (cells[next].visited) { //Pertenece al laberinto, salimos
                            break;
                        }
                        if (cells[next].exit != -1) { //Pertenece al camino ya recorrido (bucle), hay que borrar el bucle empezando en ella
                            int sigbucle;
                            do {
                                sigbucle = Neighbor(next, cells[next].exit);
                                cells[next].exit = -1; //Borramos camino bucle
                                next = sigbucle; //seguimos camino
                            } while (cells[next].exit != -1); //se sale con next original 
                        }
                        current = next;
                    } while (true);
                    //Derriba paredes del camino encontrado desde startCell a next
                    int dir;
                    current = startCell;
                    do {
                        cells[current].visited = true;
                        dir = cells[current].exit;
                        EraseWalls(current, dir);
                        current = Neighbor(current, dir);
                    } while (current != next);
                }
            }

            /*string debugMaze = "";
             for (int i = 0; i < xSize * 2 - 1; i++) {
                 for (int j = 0; j < ySize * 2 - 1; j++) {
                     debugMaze += mazeInt[i, j];
                 }
                 Debug.Log(debugMaze);
                 debugMaze = "";
             }
            Debug.Log("Holiwi");*/
        }

        void CreateMaze()
        {
            

            Vector3 scaleChange = new Vector3(wallWidth, 1.0f, wallLength + wallWidth);
            wall.transform.localScale = scaleChange;
            Vector3 floorScaleChange = new Vector3(xSize * wallLength + wallWidth, floorWidth, ySize * wallLength + wallWidth);
            floor.transform.localScale = floorScaleChange;
            Vector3 floorPos = new Vector3(0, -floorWidth / 2.0f, 0);

            //floor.transform.position = floorPos;
            mazeHolder = new GameObject();
            mazeHolder.name = "Maze";
            mazeHolder.transform.parent = transform;
            initialPos = new Vector3(-xSize * wallLength / 2.0f, wallLength / 2.0f, -ySize * wallLength / 2.0f);
            Vector3 myPos = initialPos;
            GameObject tempWall;

            //Paredes "verticales"
            for (int i = 0; i < ySize; i++) { //cambia filas (recorre columnas) 
                for (int j = 0; j <= xSize; j++) {
                    myPos = new Vector3(initialPos.x + (j * wallLength), wallLength / 2.0f, initialPos.z + (i * wallLength) + wallLength / 2);
                    if ((j > 0) && (j < xSize) && (cells[i * xSize + j].west == 0)) {
                        continue; //omite las que no estén en el laberinto final
                    }

                    
                 //   if (j < xSize-1) {
                     //   Debug.Log("j => " + j + "   xSize => " + xSize  + " j * 2 + 1 "  + (j * 2 + 1));
                     //   Debug.Log("i => " + i + "   ySize => " + ySize + " i * 2 + 1 " + (i * 2 + 1)  + " mazeInt.Length " + mazeInt.Length);
                 //       mazeInt[j * 2 + 1, i * 2 + 1] = '·';
                 //   }


                    tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                    tempWall.transform.parent = mazeHolder.transform;

                }
            }

            //Paredes "horizontales"
            for (int i = 0; i <= ySize; i++) {
                for (int j = 0; j < xSize; j++) {
                    myPos = new Vector3(initialPos.x + (j * wallLength) + wallLength / 2, wallLength / 2.0f, initialPos.z + (i * wallLength));
                    if ((i > 0) && (i < ySize) && (cells[i * xSize + j].south == 0)) {
                        continue; //omite las que no estén en el laberinto final
                    }


                   // if(i<ySize-1)
                   //     mazeInt[j*2+1, i*2+1] = '·';
                    
                    
                    tempWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                    tempWall.transform.parent = mazeHolder.transform;
                }
            }

            //Suelo
            tempWall = Instantiate(floor, floorPos, Quaternion.identity) as GameObject;
            tempWall.transform.parent = mazeHolder.transform;
        }


        private void CreateStartFinal()
        {
            GameObject tempWall = null;
            whereStart3 = new Vector3();
            whereFinal3 = new Vector3();
            whereStart300 = new Vector3(medPos.x - (((float)xSize / 2) * WallLenght) + ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
            if (whereStart == whereFinal) {
                whereFinal = 0;
            }
            if (whereStart != 1 && whereStart != 2 && whereStart != 3 && whereStart != 4) {
                do {
                    whereStart = UnityEngine.Random.Range(1, 4);
                } while (whereStart == whereFinal);
            }
            switch (whereStart) {
                case 1:
                    whereStart3 = new Vector3(medPos.x - (((float)xSize / 2) * WallLenght) + ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                    break;
                case 2:
                    whereStart3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                    break;
                case 3:
                    whereStart3 = whereStart300;
                    break;
                case 4:
                    whereStart3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
                    break;
                default:
                    Debug.Log("Nunca deberias entrar aqui");
                    break;

            }
            tempWall = Instantiate(start, whereStart3, Quaternion.identity) as GameObject;
            tempWall.transform.parent = mazeHolder.transform;
            if (whereFinal != 1 && whereFinal != 2 && whereFinal != 3 && whereFinal != 4) {
                do {
                    whereFinal = UnityEngine.Random.Range(1, 4);
                } while (whereStart == whereFinal);
            }
            switch (whereFinal) {
                case 1:
                    whereFinal3 = new Vector3(medPos.x - (((float)xSize / 2) * WallLenght) + ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                    break;
                case 2:
                    whereFinal3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                    break;
                case 3:
                    whereFinal3 = new Vector3(medPos.x - (((float)xSize / 2) * WallLenght) + ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
                    break;
                case 4:
                    whereFinal3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
                    break;
                default:
                    Debug.Log("Nunca deberias entrar aqui");
                    break;

            }
            tempWall = Instantiate(final, whereFinal3, Quaternion.identity) as GameObject;
            tempWall.transform.parent = mazeHolder.transform;
            agent.transform.localPosition = whereStart3 - transform.localPosition;
            agent.GetComponent<MazeAgentWilson>().reset();


        }




        /* void CreateWalls()
         {

             mazeInt = new char[xSize * 2 - 1, ySize * 2 - 1];
             for (int i = 0; i < xSize * 2 - 1; i++) {
                 for (int j = 0; j < ySize * 2 - 1; j++) {
                     mazeInt[i, j] = '·';
                 }
             }
             for (int i = 1; i < xSize * 2 - 1; i += 2) {
                 for (int j = 0; j < ySize * 2 - 1; j++) {
                     mazeInt[i, j] = '#';
                 }
             }
             for (int i = 1; i < ySize * 2 - 1; i += 2) {
                 for (int j = 0; j < xSize * 2 - 1; j++) {
                     mazeInt[j, i] = '#';
                 }
             }


             mazeHolder = new GameObject();
             mazeHolder.name = "Maze";
             mazeHolder.transform.parent = transform;
             // mazeHolder.transform.localPosition = transform.localPosition;
             wall.transform.localScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, WallLenght);
             floor.transform.localScale = new Vector3(xSize * WallLenght, floor.transform.localScale.y, ySize * WallLenght);
             initialPos = new Vector3((((float)-xSize / 2) + (float)WallLenght / 2) + transform.localPosition.x, 0, (((float)-ySize / 2) + (float)WallLenght / 2) + transform.localPosition.z); // Original


             Vector3 myPos = initialPos;
             GameObject tempWall;
             medPos = new Vector3(initialPos.x + (((float)xSize / 2) * WallLenght) - (float)WallLenght / 2, transform.localPosition.y - wall.transform.localScale.y / 2, initialPos.z + (((float)ySize / 2) * WallLenght) - (float)WallLenght);


             for (int i = 0; i < ySize; i++) {
                 for (int j = 0; j <= xSize; j++) {
                     myPos = new Vector3(initialPos.x + (j * WallLenght) - (float)WallLenght / 2, transform.localPosition.y, initialPos.z + (i * WallLenght) - (float)WallLenght / 2);
                     tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                     tempWall.transform.parent = mazeHolder.transform;
                 }
             }

             //for Y axes
             for (int i = 0; i <= ySize; i++) {
                 for (int j = 0; j < xSize; j++) {
                     myPos = new Vector3(initialPos.x + (j * WallLenght), transform.localPosition.y, initialPos.z + (i * WallLenght) - WallLenght);
                     tempWall = Instantiate(wall, myPos, Quaternion.Euler(0f, 90f, 0f)) as GameObject;
                     tempWall.transform.parent = mazeHolder.transform;


                 }
             }
             tempWall = Instantiate(floor, medPos, Quaternion.identity) as GameObject;
             tempWall.transform.parent = mazeHolder.transform;
             CreateCells();
             CreateStartFinal();
             if (!PerfectMaze) {
                 NonPerfectgenerator();
             }

             // debugMaze = "";


         }

         private void NonPerfectgenerator()
         {
             // Debug.Log("A romper");
             int numWalls = (ySize - 1) * (2 * (xSize - 1) + 1) + xSize - 1;
             numWalls = numWalls - (xSize * ySize) + 1;

             int wallsToBreak = (int)Math.Floor(((double)PercentWallEraser / (double)100) * numWalls);
             // Debug.Log("A romper " + (int)Math.Floor(((double)PercentWallEraser / (double)100) * numWalls) + " muros");
             bool toBreak = false;
             while (wallsToBreak > 0) {

                 toBreak = false;
                 currentCell = UnityEngine.Random.Range(0, (xSize * ySize) - 1);
                 GiveMeNeighbourToBreak();
                 int xpos = currentCell % xSize;
                 xpos *= 2;
                 int ypos = Mathf.FloorToInt(currentCell / xSize);
                 ypos *= 2;
                 // Debug.Log(" Cell " + currentCell + " , wall : " + wallToBreak);
                 switch (wallToBreak) {
                     case 1:
                         if (mazeInt[xpos, ypos + 1] == '#')
                             toBreak = true;
                         break;
                     case 2:

                         if (mazeInt[xpos - 1, ypos] == '#')
                             toBreak = true;
                         break;
                     case 3:
                         if (mazeInt[xpos + 1, ypos] == '#')
                             toBreak = true;
                         break;
                     case 4:
                         if (mazeInt[xpos, ypos - 1] == '#')
                             toBreak = true;
                         break;
                 }

                 if (toBreak) {
                     //   Debug.Log("La pared " + wallToBreak + " de la celula " + currentCell + " no esta rota");
                     BreakWall();
                     //  Debug.Log("Ahora si");
                     wallsToBreak--;
                 }

             }

         }

         void GiveMeNeighbourToBreak()
         {

             int[] connectingWall = new int[4];
             int length = 0;
             int[] neighbours = new int[4];
             int check = 0;
             check = ((currentCell + 1) / xSize);
             check -= 1;
             check *= xSize;
             check += xSize;

             //west
             if (currentCell + 1 < totalCells && (currentCell + 1) != check) {

                 neighbours[length] = currentCell + 1;
                 connectingWall[length] = 3;
                 length++;

             }
             //east
             if (currentCell - 1 >= 0 && currentCell != check) {

                 neighbours[length] = currentCell - 1;
                 connectingWall[length] = 2;
                 length++;

             }

             //north
             if (currentCell + xSize < totalCells) {

                 neighbours[length] = currentCell + xSize;
                 connectingWall[length] = 1;
                 length++;

             }

             //South
             if (currentCell - xSize >= 0) {

                 neighbours[length] = currentCell - xSize;
                 connectingWall[length] = 4;
                 length++;

             }

             if (length != 0) {
                 int theChoosenOne = UnityEngine.Random.Range(0, length);
                 currentNeighbour = neighbours[theChoosenOne];
                 wallToBreak = connectingWall[theChoosenOne];
             }
             else {
                 if (backingUp > 0) {
                     currentCell = lastCells[backingUp];
                     backingUp--;
                 }
             }

         }

         private void CreateStartFinal()
         {
             GameObject tempWall = null;
             whereStart3 = new Vector3();
             whereFinal3 = new Vector3();
             whereStart300 = new Vector3(medPos.x - (((float)xSize / 2) * WallLenght) + ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
             if (whereStart == whereFinal) {
                 whereFinal = 0;
             }
             if (whereStart != 1 && whereStart != 2 && whereStart != 3 && whereStart != 4) {
                 do {
                     whereStart = UnityEngine.Random.Range(1, 4);
                 } while (whereStart == whereFinal);
             }
             switch (whereStart) {
                 case 1:
                     whereStart3 = new Vector3(medPos.x - (((float)xSize / 2) * WallLenght) + ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                     break;
                 case 2:
                     whereStart3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                     break;
                 case 3:
                     whereStart3 = whereStart300;
                     break;
                 case 4:
                     whereStart3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
                     break;
                 default:
                     Debug.Log("Nunca deberias entrar aqui");
                     break;

             }
             tempWall = Instantiate(start, whereStart3, Quaternion.identity) as GameObject;
             tempWall.transform.parent = mazeHolder.transform;
             if (whereFinal != 1 && whereFinal != 2 && whereFinal != 3 && whereFinal != 4) {
                 do {
                     whereFinal = UnityEngine.Random.Range(1, 4);
                 } while (whereStart == whereFinal);
             }
             switch (whereFinal) {
                 case 1:
                     whereFinal3 = new Vector3(medPos.x - (((float)xSize / 2) * WallLenght) + ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                     break;
                 case 2:
                     whereFinal3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                     break;
                 case 3:
                     whereFinal3 = new Vector3(medPos.x - (((float)xSize / 2) * WallLenght) + ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
                     break;
                 case 4:
                     whereFinal3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
                     break;
                 default:
                     Debug.Log("Nunca deberias entrar aqui");
                     break;

             }
             tempWall = Instantiate(final, whereFinal3, Quaternion.identity) as GameObject;
             tempWall.transform.parent = mazeHolder.transform;
             agent.transform.localPosition = whereStart3 - transform.localPosition;
             agent.GetComponent<MazeAgent>().reset();


         }
         private void CreateCells()
         {
             lastCells = new List<int>();
             lastCells.Clear();
             totalCells = xSize * ySize;
             GameObject[] allWalls;
             int children = mazeHolder.transform.childCount;
             allWalls = new GameObject[children];
             cells = new Cell[xSize * ySize];
             int eastWestProcess = 0;
             int childProcess = 0;
             int termCount = 0;
             //Gets All the children
             for (int i = 0; i < children; i++) {
                 allWalls[i] = mazeHolder.transform.GetChild(i).gameObject;
             }
             //Assigns waqll to the cells
             for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++) {
                 cells[cellprocess] = new Cell();
                 cells[cellprocess].east = allWalls[eastWestProcess];
                 cells[cellprocess].south = allWalls[childProcess + (xSize + 1) * ySize];
                 if (termCount == xSize) {
                     eastWestProcess += 2;
                     termCount = 0;
                 }
                 else
                     eastWestProcess++;

                 termCount++;
                 childProcess++;
                 cells[cellprocess].west = allWalls[eastWestProcess];
                 cells[cellprocess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];


             }
             createMaze();
         }

         void createMaze()
         {
             while (visitedCells < totalCells) {
                 if (startedBuilding) {
                     GiveMeNeighbour();
                     if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true) {
                         BreakWall();
                         cells[currentNeighbour].visited = true;
                         visitedCells++;
                         lastCells.Add(currentCell);
                         currentCell = currentNeighbour;
                         if (lastCells.Count > 0) {
                             backingUp = lastCells.Count - 1;
                         }
                     }
                 }
                 else {
                     currentCell = UnityEngine.Random.Range(0, totalCells);
                     cells[currentCell].visited = true;
                     visitedCells++;
                     startedBuilding = true;
                 }



             }
             //Debug.Log("Finished");

         }

         void BreakWall()
         {
             int xpos = currentCell % xSize;
             xpos *= 2;
             int ypos = Mathf.FloorToInt(currentCell / xSize);
             ypos *= 2;
             switch (wallToBreak) {
                 case 1:
                     mazeInt[xpos, ypos + 1] = '·';
                     Destroy(cells[currentCell].north);
                     break;
                 case 2:
                     mazeInt[xpos - 1, ypos] = '·';
                     Destroy(cells[currentCell].east);
                     break;
                 case 3:
                     mazeInt[xpos + 1, ypos] = '·';
                     Destroy(cells[currentCell].west);
                     break;
                 case 4:
                     mazeInt[xpos, ypos - 1] = '·';
                     Destroy(cells[currentCell].south);

                     break;
             }
         }


         void GiveMeNeighbour()
         {

             int[] connectingWall = new int[4];
             int length = 0;
             int[] neighbours = new int[4];
             int check = 0;
             check = ((currentCell + 1) / xSize);
             check -= 1;
             check *= xSize;
             check += xSize;

             //west
             if (currentCell + 1 < totalCells && (currentCell + 1) != check) {
                 if (cells[currentCell + 1].visited == false) {
                     neighbours[length] = currentCell + 1;
                     connectingWall[length] = 3;
                     length++;
                 }
             }
             //east
             if (currentCell - 1 >= 0 && currentCell != check) {
                 if (cells[currentCell - 1].visited == false) {
                     neighbours[length] = currentCell - 1;
                     connectingWall[length] = 2;
                     length++;
                 }
             }

             //north
             if (currentCell + xSize < totalCells) {
                 if (cells[currentCell + xSize].visited == false) {
                     neighbours[length] = currentCell + xSize;
                     connectingWall[length] = 1;
                     length++;
                 }
             }

             //South
             if (currentCell - xSize >= 0) {
                 if (cells[currentCell - xSize].visited == false) {
                     neighbours[length] = currentCell - xSize;
                     connectingWall[length] = 4;
                     length++;
                 }
             }

             if (length != 0) {
                 int theChoosenOne = UnityEngine.Random.Range(0, length);
                 currentNeighbour = neighbours[theChoosenOne];
                 wallToBreak = connectingWall[theChoosenOne];
             }
             else {
                 if (backingUp > 0) {
                     currentCell = lastCells[backingUp];
                     backingUp--;
                 }
             }

         } */

        // Update is called once per frame
        void Update()
        {

        }

        void EraseMaze()
        {
            for (int i = 0; i < mazeHolder.transform.childCount; i++) {
                Destroy(mazeHolder.transform.GetChild(i).gameObject);
            }
            Destroy(mazeHolder);
            cells = new Cell[0];
     //       visitedCells = 0;
     //       startedBuilding = false;
            whereFinal = 0;
            whereStart = 0;
        }
        public void ResetMazeArea()
        {

            EraseMaze();
            mazeCreator();

        }
        public void resetAgentPos()
        {
            agent.transform.localPosition = whereStart3 - transform.localPosition;
        }

        public override void ResetArea()
        {
        }
    }

}