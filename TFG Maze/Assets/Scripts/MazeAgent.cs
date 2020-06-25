
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Text;
using System.Linq;
using System.IO;
using System;

namespace Maze
{

    class Location
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public Location Parent;
    }

    class Point
    {
        public int X;
        public int Y;

        public bool Equals(Point other)
        {
            if (other == null)
                return false;

            if (this.X == other.X && this.Y == other.Y)
                return true;
            else
                return false;
        }

    }
    
    public class MazeAgent : Agent
    {

        //  public bool taged { get; private set; }
        
        private MazeArea area;
        private float horizontalChange;
        private float verticalChange;
        private int actualPosX = 0;
        private int actualPosY = 0;
        private int finalX;
        private int finalY;
        private int startX;
        private int startY;
        private MazeAcademy academy;
        [Header("Training")]
        [Tooltip("number of steps to time out after in training")]
        public int stepTimeout = 1000;

        private StringBuilder csvContent;
        private string csvPath;
        

        [Tooltip("Number os Scenarios")]
        public int scenarioNumber = 100;
        public int studioNumber = 100;
        public string studioString;
        private int scenariosteps;
        private int scenarioIterations;
        private int studioIteration;
        private int acumulateSteps;
        void Start()
        {
            academy = FindObjectOfType<MazeAcademy>();
            area = GetComponentInParent<MazeArea>();
            csvContent = new StringBuilder();
            csvContent.AppendLine("Laberinto, Optimo, Pasos A* optimo, Pasos A* modificado, Veces mejor, Veces igual, Media Modelo ");
            csvPath = "C:\\Users\\pablo\\Desktop\\TFG\\"+ area.academy.estudioName + ".csv";
            
        }

        // Update is called once per frame
        void Update()
        {

        }


        public override void InitializeAgent()
        {
            scenarioIterations = 0;
            scenariosteps = 0;
            acumulateSteps = 0;
            studioIteration = 0;
            studioString = " ";
            academy = FindObjectOfType<MazeAcademy>();
            area = GetComponentInParent<MazeArea>();
            
            base.InitializeAgent();
            
            switch (area.whereStart) {
                case 3:
                    actualPosX = 0;
                    actualPosY = 0;
                    break;
                case 4:
                    actualPosX = (area.xSize*2)-2;
                    actualPosY = 0;
                    break;
                case 1:
                    actualPosX = 0;
                    actualPosY = (area.ySize*2) - 2;
                    break;
                case 2:
                    actualPosX = (area.xSize*2) - 2;
                    actualPosY = (area.ySize*2) - 2;
                    break;
            }
            switch (area.whereFinal) {
                case 3:
                    finalX = 0;
                    finalY = 0;
                    break;
                case 4:
                    finalX = (area.xSize * 2) - 2;
                    finalY = 0;
                    break;
                case 1:
                    finalX = 0;
                    finalY = (area.ySize * 2) - 2;
                    break;
                case 2:
                    finalX = (area.xSize * 2) - 2;
                    finalY = (area.ySize * 2) - 2;
                    break;
            }

          

        }

        public override void CollectObservations()
        {
            int positiveVerticalSpace = 0;
            bool findWall = false;
            for(int i = actualPosY+1; (i < (area.ySize * 2) - 1) && !findWall; i++) {
                if(area.mazeInt[actualPosX,i] == '·') {
                    positiveVerticalSpace++;
                }else if(area.mazeInt[actualPosX, i] == '#') {
                    findWall = true;
                }
            }
            AddVectorObs(positiveVerticalSpace * (area.WallLenght / 2));

            int negativeVerticalSpace = 0;
            findWall = false;
            for (int i = actualPosY - 1; (i > - 1) && !findWall; i--) {
                if (area.mazeInt[actualPosX, i] == '·') {
                    negativeVerticalSpace++;
                }
                else if (area.mazeInt[actualPosX, i] == '#') {
                    findWall = true;
                }
            }
            AddVectorObs(negativeVerticalSpace * (area.WallLenght / 2));

            int positiveHorizontalSpace = 0;
            findWall = false;
            for (int i = actualPosX + 1; (i < (area.xSize * 2)-1) && !findWall; i++) {
                if (area.mazeInt[i, actualPosY] == '·') {
                    positiveHorizontalSpace++;
                }
                else if (area.mazeInt[i, actualPosY] == '#') {
                    findWall = true;
                }
            }
            AddVectorObs(positiveHorizontalSpace * (area.WallLenght / 2));

            int negativeHorizontalSpace = 0;
            findWall = false;
            for (int i = actualPosX - 1; (i >  - 1) && !findWall; i--) {
                if (area.mazeInt[i, actualPosY] == '·') {
                    negativeHorizontalSpace++;
                }
                else if (area.mazeInt[i, actualPosY] == '#') {
                    findWall = true;
                }
            }
            AddVectorObs(negativeHorizontalSpace * (area.WallLenght / 2));


            AddVectorObs(transform.localPosition.x);//Agent position
            AddVectorObs(transform.localPosition.z);
            AddVectorObs(area.whereFinal3.x);//Final position
            AddVectorObs(area.whereFinal3.z);

        }

        public override void AgentAction(float[] vectorAction)
        {
            
            horizontalChange = 0;
            verticalChange = 0;
            

            switch (vectorAction[0]) {

                case 0:
                    verticalChange = 1f;
                    break;
                case 1:
                    verticalChange = -1f;
                    break;
                case 2:
                    horizontalChange = 1f;
                    break;
                case 3:
                    horizontalChange = -1f;
                    break;

            }

            if (area.academy.DebugMode) {


                scenariosteps++;


            }

            ProcessMovement();

            //AddReward(-1.0f / (((area.xSize *2-1)*(area.ySize*2-1)) * 6));
            AddReward(-1.0f / (area.xSize * area.ySize * 6));
           
            


        }
       /* public override void AgentAction(float[] vectorAction)
        {

            horizontalChange = vectorAction[0];
            if (horizontalChange == 2) horizontalChange = -1f;
  
            verticalChange = vectorAction[1];
            if (verticalChange == 2) verticalChange = -1f;
           
         
            ProcessMovement();
           
            //AddReward(-1.0f / (((area.xSize *2-1)*(area.ySize*2-1)) * 6));
            AddReward(-1.0f / (area.xSize * area.ySize * 6));
           
            if (area.academy.DebugMode) {


                scenariosteps++;
                

            }


        }*/

        private void ProcessMovement()
        {

            
            if((actualPosX+verticalChange) < (area.xSize*2)-1 && (actualPosX + verticalChange) > -1 ) {
                if (area.mazeInt[actualPosX + (int)verticalChange, actualPosY] != '#') {
                    if (verticalChange != 0) {
                        actualPosX = actualPosX + (int)verticalChange * 2;
                        //actualPosX = actualPosX + (int)verticalChange;
                        transform.localPosition = new Vector3(transform.localPosition.x + (area.WallLenght) * verticalChange, transform.localPosition.y, transform.localPosition.z);
                       // transform.localPosition = new Vector3(transform.localPosition.x + (area.WallLenght / 2) * verticalChange, transform.localPosition.y, transform.localPosition.z);
                    }
                }
            }

            if ((actualPosY + horizontalChange) < (area.ySize*2)-1 && (actualPosY + horizontalChange) > -1) {
                if (area.mazeInt[actualPosX, actualPosY + (int)horizontalChange] != '#') {
                    if (horizontalChange != 0) {
                        actualPosY = actualPosY + (int)horizontalChange * 2 ;
                      //  actualPosY = actualPosY + (int)horizontalChange;
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + (area.WallLenght ) * horizontalChange);
                      //  transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + (area.WallLenght / 2) * horizontalChange);
                    }

                }
            }
            if (actualPosX == finalX && actualPosY == finalY) {
                AddReward(1f);
                Done();
                
            }



        }
        void estudio()
        {
            if (scenarioIterations == 1) {
                aStarAlgorimth();
                aStarTruePath();
                
            }
            Debug.Log(scenarioIterations + " <-- Escenarios   | Pasos en ese escenario  --> " + scenariosteps);
            studioString += scenariosteps + ",";

            scenariosteps = 0;
            if (scenarioIterations == scenarioNumber) {

                
                acumulateSteps /=  scenarioNumber;
                Debug.Log("De media en " + scenarioNumber + " escenarios de " + area.xSize + " x, " + area.ySize + "y.  Ha hecho un total de " + acumulateSteps + " pasos");
                acumulateSteps = 0;
                scenarioIterations = 0;
                csvContent.AppendLine(studioString);
                studioString = "";
                
                studioIteration++;
                var rand = new System.Random();
                area.xSize = rand.Next(5, 10);
                area.ySize = rand.Next(5, 10);
               
                area.ResetMazeArea();
                if (studioIteration == studioNumber) {

                    File.AppendAllText(csvPath, csvContent.ToString());
                    Debug.Log("Finiquitado");

                }


            }
            

            
        }
        public override void AgentReset()
        {
            if(area.academy.DebugMode)
                scenariosteps++;
            
            if (area.academy.DebugMode) {
             //   if (scenariosteps != 0 && scenariosteps != area.xSize - 1 && scenariosteps < agentParameters.maxStep) {
             // if (scenariosteps != 0 && scenariosteps != area.xSize - 1 ) {
                if (scenariosteps != 0 && scenariosteps != 1) {
                    acumulateSteps += scenariosteps;
                    scenarioIterations++;
                    
                    if(area.academy.estudioMode)
                        estudio();
                    else
                        Debug.Log(scenarioIterations + " <-- Escenarios   | Pasos en ese escenario  --> " + scenariosteps);
                }
                else {
                    scenariosteps = 0;
                }
            }
            


            
            area.resetAgentPos();
            configureAreaCurricula();
            if (!area.academy.estudioMode) {
                area.ResetMazeArea();
                
            }

            switch (area.whereStart) {
                case 3:
                    startX = 0;
                    startY = 0;
                    break;
                case 4:
                    startX = (area.xSize * 2) - 2;
                    startY = 0;
                    break;
                case 1:
                    startX = 0;
                    startY = (area.ySize * 2) - 2;
                    break;
                case 2:
                    startX = (area.xSize * 2) - 2;
                    startY = (area.ySize * 2) - 2;
                    break;
            }
            actualPosX = startX;
            actualPosY = startY;

            switch (area.whereFinal) {
                case 3:
                    finalX = 0;
                    finalY = 0;
                    break;
                case 4:
                    finalX = (area.xSize * 2) - 2;
                    finalY = 0;
                    break;
                case 1:
                    finalX = 0;
                    finalY = (area.ySize * 2) - 2;
                    break;
                case 2:
                    finalX = (area.xSize * 2) - 2;
                    finalY = (area.ySize * 2) - 2;
                    break;
            }

        }

        void configureAreaCurricula()
        {
            
            
            int sizeX = (int)academy.FloatProperties.GetPropertyWithDefault("x_size", area.xSize);
            int sizeY = (int)academy.FloatProperties.GetPropertyWithDefault("y_size", area.ySize);
            //Debug.Log("x -> " + academy.FloatProperties.GetPropertyWithDefault("x_size", area.xSize) + " , " + academy.FloatProperties.GetPropertyWithDefault("y_size", area.ySize) + " <- y  ");
            if(sizeX != area.xSize || sizeY != area.ySize) {
                Debug.Log("Cambio de tamaño!!!!  x-> " + sizeX + " ,   y -> " + sizeY);
            }
            area.xSize = sizeX;
            area.ySize = sizeY;
            agentParameters.maxStep = (((area.xSize ) * (area.ySize )) * 40);
        }

        public void reset()
        {
            switch (area.whereStart) {
                case 3:
                    startX = 0;
                    startY = 0;
                    break;
                case 4:
                    startX = (area.xSize * 2) - 2;
                    startY = 0;
                    break;
                case 1:
                    startX = 0;
                    startY = (area.ySize * 2) - 2;
                    break;
                case 2:
                    startX = (area.xSize * 2) - 2;
                    startY = (area.ySize * 2) - 2;
                    break;
            }
            actualPosX = startX;
            actualPosY =  startY;

            switch (area.whereFinal) {
                case 3:
                    finalX = 0;
                    finalY = 0;
                    break;
                case 4:
                    finalX = (area.xSize * 2) - 2;
                    finalY = 0;
                    break;
                case 1:
                    finalX = 0;
                    finalY = (area.ySize * 2) - 2;
                    break;
                case 2:
                    finalX = (area.xSize * 2) - 2;
                    finalY = (area.ySize * 2) - 2;
                    break;
            }
        }



        //A* stuff
        void aStarAlgorimth()
        {
            //Console.Title = "A* Pathfinding";

            // draw map

           

            

            // algorithm

            Location current = null;
            var start = new Location { X = startX, Y = startY };
            var target = new Location { X = finalX, Y = finalY };
            var openList = new List<Location>();
            var closedList = new List<Location>();
            int g = 0;
            int aStarSteps = 0;
            // start by adding the original position to the open list
            openList.Add(start);

            while (openList.Count > 0) {
                
                // get the square with the lowest F score
              /*  var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);*/

                
                int min = openList.ElementAt(0).F;
                current = openList.ElementAt(0);
                for (int i = 0; i < openList.Count; i++) {
                    int aux = openList.ElementAt(i).F;
                    if (aux < min) {

                            min = aux;
                            current = openList.ElementAt(i);

                    }

                }
                 

                // add the current square to the closed list
                closedList.Add(current);

                // show current square on the map
                //Console.SetCursorPosition(current.X, current.Y);
                //Console.Write('.');
                //Console.SetCursorPosition(current.X, current.Y);
                //System.Threading.Thread.Sleep(1000);

                // remove it from the open list

                aStarSteps++;

                openList.Remove(current);

                // if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                    break;

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y);
                g++;

                foreach (var adjacentSquare in adjacentSquares) {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) != null)
                        continue;

                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) == null) {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Add(adjacentSquare);
                    }
                    else {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F) {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            // assume path was found; let's show it
            int mostOptimo = 0;
           while (current != null) {
           //     Console.SetCursorPosition(current.X, current.Y);
           //     Console.Write('_');
           //     Console.SetCursorPosition(current.X, current.Y);
                current = current.Parent;
                mostOptimo++;
           //     System.Threading.Thread.Sleep(1000);
            }
            studioString +=area.xSize+"x"+area.ySize+", " + mostOptimo + ", " + aStarSteps+", ";
            Debug.Log("Optimo -> " + mostOptimo);
            Debug.Log("A* pasos -> " + aStarSteps);




            // end

            //Console.ReadLine();
        }

         List<Location> GetWalkableAdjacentSquares(int x, int y)
        {

            
            var proposedLocations = new List<Location>() { };
            if ((y - 1) < (area.ySize * 2) - 1 && (y - 1) > -1 && area.mazeInt[x,y-1] != '#')
                proposedLocations.Add(new Location { X = x, Y = y - 2 });
            if ((y + 1) < (area.ySize * 2) - 1 && (y + 1) > -1 && area.mazeInt[x, y + 1] != '#')
                proposedLocations.Add(new Location { X = x, Y = y + 2 });
            if ((x - 1) < (area.xSize * 2) - 1 && (x - 1) > -1 && area.mazeInt[x - 1, y] != '#')
                proposedLocations.Add(new Location { X = x-2, Y = y });
            if ((x + 1) < (area.xSize * 2) - 1 && (x + 1) > -1 && area.mazeInt[x + 1, y] != '#')
                proposedLocations.Add(new Location { X = x+2, Y = y });

            




            return proposedLocations;
           // return proposedLocations.Where(l => area.mazeInt[l.Y][l.X] == ' ' || area.mazeInt[l.Y][l.X] == 'B').ToList();
        }

         int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

        void aStarTruePath()
        {
            int totalSteps = 0;
            List<Point> visited = new List<Point>();
            Stack<Point> myStack = new Stack<Point>();
            Point start = new Point { X = startX, Y = startY };
            myStack.Push(start);
            visited.Add(start);
            do {
                
                totalSteps++;
                Point next = null;
                next = choosePath(myStack.Peek().X, myStack.Peek().Y, visited);
                if(next == null) {

                    visited.Add(myStack.Peek());
                    myStack.Pop();

                }
                else {
                    visited.Add(myStack.Peek());
                    myStack.Push(next);
                    

                }
                if(myStack.Count > 0 && myStack.Peek().X == finalX && myStack.Peek().Y == finalY) {
                    totalSteps++;
                    break;

                }
                //Thread.Sleep(2000);
            } while (myStack.Count > 0);

            Debug.Log("Total steps A* -> " + totalSteps);
            studioString += totalSteps + " ,   ,   ,   ,";
           
        }

        Point choosePath(int x, int y, List<Point> visited)
        {
            var pointN = new Point { X = x + 2, Y = y };
            var pointS = new Point { X = x - 2, Y = y };
            var pointE = new Point { X = x, Y = y + 2 };
            var pointO = new Point { X = x, Y = y - 2 };
            List<Point> proposedLocations = new List<Point>() { };
            if ((y - 1) < (area.ySize * 2) - 1 && (y - 1) > -1 && area.mazeInt[x, y - 1] != '#' && visited.FirstOrDefault(l => l.X == pointO.X && l.Y == pointO.Y) == null)
                proposedLocations.Add(pointO);
            if ((y + 1) < (area.ySize * 2) - 1 && (y + 1) > -1 && area.mazeInt[x, y + 1] != '#' && visited.FirstOrDefault(l => l.X == pointE.X && l.Y == pointE.Y) == null)
                proposedLocations.Add(pointE);
            if ((x - 1) < (area.xSize * 2) - 1 && (x - 1) > -1 && area.mazeInt[x - 1, y] != '#' && visited.FirstOrDefault(l => l.X == pointS.X && l.Y == pointS.Y) == null)
                proposedLocations.Add(pointS);
            if ((x + 1) < (area.xSize * 2) - 1 && (x + 1) > -1 && area.mazeInt[x + 1, y] != '#' && visited.FirstOrDefault(l => l.X == pointN.X && l.Y == pointN.Y) == null)
                proposedLocations.Add(pointN);

            if (proposedLocations.Count == 0)
                return null;

            int lowest = ComputeHScore(proposedLocations.ElementAt(0).X, proposedLocations.ElementAt(0).Y, finalX, finalY);
            Point dummy = proposedLocations.ElementAt(0);
            for (int i = 0; i < proposedLocations.Count; i++) {
                int aux = ComputeHScore(proposedLocations.ElementAt(i).X, proposedLocations.ElementAt(i).Y, finalX, finalY);
                if (aux < lowest) {

                    lowest = aux;
                    dummy = proposedLocations.ElementAt(i);

                }

            }
            return dummy;
        }

        /*     Point choosePath(int x, int y, List<Point> visited)
        {

            var proposedLocations = new List<Point>() { };
            if ((y - 1) < (area.ySize * 2) - 1 && (y - 1) > -1 && area.mazeInt[x, y - 1] != '#' )
                proposedLocations.Add(new Point { X = x, Y = y - 2, Distance = ComputeHScore(x , y - 2, finalX, finalY) });
            if ((y + 1) < (area.ySize * 2) - 1 && (y + 1) > -1 && area.mazeInt[x, y + 1] != '#' )
                proposedLocations.Add(new Point { X = x, Y = y + 2, Distance = ComputeHScore(x, y+2, finalX, finalY) });
            if ((x - 1) < (area.xSize * 2) - 1 && (x - 1) > -1 && area.mazeInt[x - 1, y] != '#' )
                proposedLocations.Add(new Point { X = x - 2, Y = y, Distance = ComputeHScore(x - 2, y, finalX, finalY) });
            if ((x + 1) < (area.xSize * 2) - 1 && (x + 1) > -1 && area.mazeInt[x + 1, y] != '#' )
                proposedLocations.Add(new Point { X = x + 2, Y = y , Distance = ComputeHScore(x+2,y,finalX,finalY)});

            if (proposedLocations.Count == 0)
                return null;

            var lowest = ComputeHScore(proposedLocations.ElementAt(0).X , proposedLocations.ElementAt(0).Y, finalX, finalY);
            Point dummy = proposedLocations.ElementAt(0);
            for(int i = 0; i < proposedLocations.Count; i++) {
                int aux = ComputeHScore(proposedLocations.ElementAt(i).X, proposedLocations.ElementAt(i).Y, finalX, finalY);
                if (aux < lowest) {

                    lowest = aux;
                    dummy = proposedLocations.ElementAt(i);

                }

            }
            return dummy;
        }*/
    }
}
