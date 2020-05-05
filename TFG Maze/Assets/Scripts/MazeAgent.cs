using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Maze
{
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
        private MazeAcademy academy;
        [Header("Training")]
        [Tooltip("number of steps to time out after in training")]
        public int stepTimeout = 1000;

      
        [Tooltip("Number os Scenarios")]
        public int scenarioNumber = 1000;

        private int scenariosteps;
        private int scenarioIterations;
        private int acumulateSteps;
        void Start()
        {
            
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

            horizontalChange = vectorAction[0];
            if (horizontalChange == 2) horizontalChange = -1f;
  
            verticalChange = vectorAction[1];
            if (verticalChange == 2) verticalChange = -1f;
           
         
            ProcessMovement();
           
            AddReward(-1.0f / (((area.xSize *2-1)*(area.ySize*2-1)) * 5));
           
            if (!area.academy.DebugMode) {

                scenariosteps++;
               

            }


        }

        private void ProcessMovement()
        {
            
          
            if((actualPosX+verticalChange) < (area.xSize*2)-1 && (actualPosX + verticalChange) > -1 ) {
                if (area.mazeInt[actualPosX + (int)verticalChange, actualPosY] == '·') {
                    if (verticalChange != 0) {
                        actualPosX = actualPosX + (int)verticalChange;
                        transform.localPosition = new Vector3(transform.localPosition.x + (area.WallLenght / 2) * verticalChange, transform.localPosition.y, transform.localPosition.z);
                    }
                }
            }

            if ((actualPosY + horizontalChange) < (area.ySize*2)-1 && (actualPosY + horizontalChange) > -1) {
                if (area.mazeInt[actualPosX, actualPosY + (int)horizontalChange] == '·') {
                    if (horizontalChange != 0) {
                        actualPosY = actualPosY + (int)horizontalChange;
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + (area.WallLenght / 2) * horizontalChange);
                    }

                }
            }
            if (actualPosX == finalX && actualPosY == finalY) {
                AddReward(1f);
                Done();
                
            }



        }

        public override void AgentReset()
        {
            if (area.academy.DebugMode) {
                acumulateSteps += scenariosteps;
                scenarioIterations++;
                scenariosteps = 0;
                if (scenarioIterations == scenarioNumber) {

                    acumulateSteps = acumulateSteps / scenarioNumber;
                    Debug.Log("De media en " + scenarioNumber + " escenarios de " + area.xSize + " x, " + area.ySize  + "y.  Ha hecho un total de " + acumulateSteps + " pasos");
                    acumulateSteps = 0;
                    scenarioIterations = 0;
                  

                }
            }
            configureAreaCurricula();
            area.ResetMazeArea();
            switch (area.whereStart) {
                case 3:
                    actualPosX = 0;
                    actualPosY = 0;
                    break;
                case 4:
                    actualPosX = (area.xSize * 2) - 2;
                    actualPosY = 0;
                    break;
                case 1:
                    actualPosX = 0;
                    actualPosY = (area.ySize * 2) - 2;
                    break;
                case 2:
                    actualPosX = (area.xSize * 2) - 2;
                    actualPosY = (area.ySize * 2) - 2;
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

        void configureAreaCurricula()
        {
            
            
            int sizeX = (int)academy.FloatProperties.GetPropertyWithDefault("x_size", area.xSize);
            int sizeY = (int)academy.FloatProperties.GetPropertyWithDefault("y_size", area.ySize);
            area.xSize = sizeX;
            area.ySize = sizeY;
        }

        public void reset()
        {
            switch (area.whereStart) {
                case 3:
                    actualPosX = 0;
                    actualPosY = 0;
                    break;
                case 4:
                    actualPosX = (area.xSize * 2) - 2;
                    actualPosY = 0;
                    break;
                case 1:
                    actualPosX = 0;
                    actualPosY = (area.ySize * 2) - 2;
                    break;
                case 2:
                    actualPosX = (area.xSize * 2) - 2;
                    actualPosY = (area.ySize * 2) - 2;
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
    }
}
