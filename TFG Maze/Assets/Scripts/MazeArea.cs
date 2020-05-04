using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


namespace Maze
{
    public class MazeArea : Area
    {
        [System.Serializable]
        public class Cell
        {
            public bool visited;
            public GameObject north; //1
            public GameObject east; //2
            public GameObject west; //3
            public GameObject south; //4

        }
        public char[,] mazeInt;
        public GameObject agent;
        public GameObject wall;
        public GameObject floor;
        public GameObject final;
        public GameObject start;
        public int whereStart;
        public int whereFinal;
        public float WallLenght = 1f;
        public int xSize = 5;
        public int ySize = 5;
        private Vector3 initialPos;
        private GameObject mazeHolder;
        private Cell[] cells;
        private int currentCell = 0;
        private int totalCells;
        private int visitedCells = 0;
        private bool startedBuilding = false;
        private int currentNeighbour = 0;
        private List<int> lastCells;
        private int backingUp = 0;
        private int wallToBreak = 0;
        private Vector3 medPos;
        

        [HideInInspector]
        public Vector3 whereStart3;
        [HideInInspector]
        public Vector3 whereFinal3;
        [HideInInspector]
        public Vector3 whereStart300;
        [HideInInspector]
        public MazeAcademy academy;
        // Start is called before the first frame update
        void Start()
        {
            // agent = transform.GetComponentsInChildren<ta>();
            academy = FindObjectOfType<MazeAcademy>();
            CreateWalls();
        }

        void CreateWalls()
        {
            
            mazeInt = new char[xSize*2-1, ySize * 2 - 1];
            for (int i = 0; i < xSize * 2 - 1; i ++ ) {
                for (int j = 0; j < ySize * 2 - 1; j++) {
                    mazeInt[i, j] = '·';
                }
            }
            for (int i = 1; i < xSize * 2 - 1; i+=2) {
                for(int j =0;  j < ySize*2-1; j++) {
                    mazeInt[i,j] = '#';
                }
            }
            for (int i = 1; i < ySize * 2 -1; i += 2) {
                for (int j = 0; j < xSize * 2 - 1; j++) {
                    mazeInt[j, i] = '#';
                }
            }
         /*   String debugMaze = "";
            for (int i = 0; i < xSize * 2 - 1; i++) {
                for (int j = 0; j < ySize * 2 - 1; j++) {
                    debugMaze += mazeInt[i, j];
                }
                Debug.Log(debugMaze);
                debugMaze = "";
            }*/

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


          // debugMaze = "";
           /* for (int i = 0; i < xSize * 2 - 1; i++) {
                for (int j = 0; j < ySize * 2 - 1; j++) {
                    debugMaze += mazeInt[i, j];
                }
                Debug.Log(debugMaze);
                debugMaze = "";
            }*/

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
                    // tempWall = Instantiate(start, whereStart3, Quaternion.identity) as GameObject;
                    break;
                case 2:
                    whereStart3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                    //  tempWall = Instantiate(start, whereStart3, Quaternion.identity) as GameObject;
                    break;
                case 3:
                    whereStart3 = whereStart300;
                    // tempWall = Instantiate(start, whereStart3, Quaternion.identity) as GameObject;
                    break;
                case 4:
                    whereStart3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
                    //tempWall = Instantiate(start, whereStart3, Quaternion.identity) as GameObject;
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
                    //Instantiate(final, whereFinal3, Quaternion.identity);
                    break;
                case 2:
                    whereFinal3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z + (((float)ySize / 2) * WallLenght) - ((float)WallLenght / 2));
                    //Instantiate(final, whereFinal3, Quaternion.identity);
                    break;
                case 3:
                    whereFinal3 = new Vector3(medPos.x - (((float)xSize / 2) * WallLenght) + ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
                    //Instantiate(final, whereFinal3, Quaternion.identity);
                    break;
                case 4:
                    whereFinal3 = new Vector3(medPos.x + (((float)xSize / 2) * WallLenght) - ((float)WallLenght / 2), medPos.y + wall.transform.localScale.y / 2, medPos.z - (((float)ySize / 2) * WallLenght) + ((float)WallLenght / 2));
                    // Instantiate(final, whereFinal3, Quaternion.identity);
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
               // if (transform.GetChild(i).gameObject != agent) {
                    allWalls[i] = mazeHolder.transform.GetChild(i).gameObject;
               // }
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
            //Debug.Log("Holiwi - 9");
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
         //   Debug.Log(currentCell + " x " + xpos + "  y " + ypos );
            switch (wallToBreak) {
                case 1:
                //    elevate(cells[currentCell].north);
                    mazeInt[xpos, ypos + 1] = '·';
                    Destroy(cells[currentCell].north);
                    break;
                case 2:
                //    elevate(cells[currentCell].east);
                    mazeInt[xpos - 1, ypos] = '·';
                    Destroy(cells[currentCell].east);     
                    break;
                case 3:
                 //   elevate(cells[currentCell].west);
                    mazeInt[xpos+1, ypos] = '·';
                    Destroy(cells[currentCell].west);
                    break;
                case 4:
                  //  elevate(cells[currentCell].south);
                    mazeInt[xpos, ypos - 1] = '·';
                    Destroy(cells[currentCell].south);
                    
                    break;
            }
        }

     /*   void elevate(GameObject ob)
        {
            ob.transform.localPosition = new Vector3(ob.transform.localPosition.x, 20f, ob.transform.localPosition.z);

        }*/
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

        }

        // Update is called once per frame
        void Update()
        {

        }

        void EraseMaze()
        {
            for(int i = 0; i < mazeHolder.transform.childCount; i++) {
                //if (transform.GetChild(i) != agent) {
                    Destroy(mazeHolder.transform.GetChild(i).gameObject);
                //}
            }
            Destroy(mazeHolder);
            cells = new Cell[0];
            visitedCells = 0;
            startedBuilding = false;
            whereFinal = 0;
            whereStart = 0;
        }
        public void ResetMazeArea()
        {

            EraseMaze();
            CreateWalls();

        }

        public override void ResetArea()
        {
        }
    }

}