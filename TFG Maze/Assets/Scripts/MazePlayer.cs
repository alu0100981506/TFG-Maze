using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Text;
using System.Linq;
using System.IO;
using System;
namespace Maze
{
    public class MazePlayer : MazeAgent
    {
        [Header("Input Bindings")]
        public InputAction Input;
       
        //public InputAction verticalInput;


        public override void InitializeAgent()
        {
            base.InitializeAgent();
            Input.Enable();
            
           /* Point start = new Point { X = startX, Y = startY };
            myStack.Push(start);
            visited.Add(start);*/
            //   verticalInput.Enable();


        }

        public override float[] Heuristic()
        {
           
            float changeValue = 0;

            callFDSpath();
            int horizontalChange = myStack.Peek().Y - actualPosY;
            int verticalChange = myStack.Peek().X - actualPosX;

            if(horizontalChange == 2) {
                changeValue = 2F;
                //Debug.Log("este");
            }
            if (horizontalChange == -2) {
                changeValue = 3F;
                //Debug.Log("oeste");
            }
            if (verticalChange == 2) {
                changeValue = 0F;
                //Debug.Log("norte");
            }
            if (verticalChange == -2) {
                changeValue = 1F;
                //Debug.Log("sur");
            }


            
            return new float[] { changeValue };

            /*
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
             */
        }

        private void OnDestroy()
        {
            Input.Disable();
            //verticalInput.Disable();
        }

        public void callFDSpath()
        {

            if(myStack.Count > 0) {
                //Debug.Log("StackCount " + myStack.Count);
                aStarTruePath();

            }
            else {

                Debug.Log("FAIL");

            }

        }
        void aStarTruePath()
        {
          //  int totalSteps = 0;                                                  
                Point next = null;
                next = choosePath(myStack.Peek().X, myStack.Peek().Y, visited);
                if (next == null) {

                    visited.Add(myStack.Peek());
                    myStack.Pop();

                }
                else {
                    visited.Add(myStack.Peek());
                    myStack.Push(next);


                }
                if (myStack.Count > 0 && myStack.Peek().X == finalX && myStack.Peek().Y == finalY) {

                //Debug.Log("Fin " + finalX + "    "  +  finalY  );
                return;
                   

                }
                //Thread.Sleep(2000);
           

            

        }

        Point choosePath(int x, int y, List<Point> visited)
        {
            var pointN = new Point { X = x + 2, Y = y};
            var pointS = new Point { X = x - 2, Y = y};
            var pointE = new Point { X = x, Y = y + 2};
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
    }
}
