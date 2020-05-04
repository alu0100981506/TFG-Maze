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

        [Header("With Model")]
        [Tooltip("Number os Scenarios")]
        public int scenarioNumber = 1000;

        private int scenariosteps;
        private int scenarioIterations;
        private int acumulateSteps;
        // When the next step timeout will be during training
        private float nextStepTimeout;
        private bool frozen = false;
        // Start is called before the first frame update
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
         //   m_CharacterController = GetComponent<CharacterController>();
            
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

            //  agentParameters.maxStep = area.trainingMode ? 5000 : 0;

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
         //   Debug.Log("Horizontal: " + horizontalChange);
            verticalChange = vectorAction[1];
            if (verticalChange == 2) verticalChange = -1f;
            //   Debug.Log("Vertical: " + verticalChange);
            if (frozen) return;
            //Debug.Log("Horizontal Change " + horizontalChange);
           // Debug.Log("Vertical Change " + verticalChange);
            ProcessMovement();
            //if (area.trainingMode) {
                // Small negative reward every step
            AddReward(-1.0f / (((area.xSize *2-1)*(area.ySize*2-1)) * 5));
            //Debug.Log("-1f / ((area.xSize *2-1)*(area.ySize*2-1)) * 5    " + -1.0f / ((float)((((float)area.xSize * 2.0f) - 1.0f) * (((float)area.ySize * 2.0f) - 1.0f)) * 5.0f));
            // Debug.Log("-1f / ((area.xSize *2-1)*(area.ySize*2-1)) * 5    " + -1.0f / (((area.xSize * 2 - 1) * (area.ySize * 2 - 1)) * 5));
            //Debug.Log(agentParameters.maxStep + "  agentParameters.maxStep          -1f/agentParameters.maxStep  " + -1f / agentParameters.maxStep);
            // Make sure we haven't run out of time if training
            //if (GetStepCount() > nextStepTimeout) {
            //Debug.Log("A partir de aqui fallas");
            // AddReward(-.01f);
            // Done();
            // }
            //}
          //  if (!area.academy.trainingMode) {

                scenariosteps++;
               

          //  }


        }

        private void ProcessMovement()
        {
            
          //  Debug.Log("actualPosX+verticalChange > area.xSize && actualPosX + verticalChange < -1 && area.mazeInt[actualPosX+(int)verticalChange, actualPosY] == '·' " + (actualPosX + verticalChange < area.xSize && actualPosX + verticalChange > -1 ));
            if((actualPosX+verticalChange) < (area.xSize*2)-1 && (actualPosX + verticalChange) > -1 ) {
                if (area.mazeInt[actualPosX + (int)verticalChange, actualPosY] == '·') {
                    if (verticalChange != 0) {
                        actualPosX = actualPosX + (int)verticalChange;
                        transform.localPosition = new Vector3(transform.localPosition.x + (area.WallLenght / 2) * verticalChange, transform.localPosition.y, transform.localPosition.z);
                        //transform.localPosition = new Vector3(area.whereStart300.x + (area.WallLenght / 2) * actualPosX, transform.localPosition.y, transform.localPosition.z);
                        //transform.localPosition = transform.localPosition - area.transform.localPosition;
                        //Debug.Log("actualposY : " + actualPosX + "   actualposY : " + actualPosY);
                    }
                }
            }

            //Debug.Log("Vertical Change: " + verticalChange);
            //Debug.Log("actualPosY + horizontalChange > area.ySize && actualPosY + horizontalChange < -1 && area.mazeInt[actualPosX,actualPosY+(int)horizontalChange] == '·' " + (actualPosY + horizontalChange < area.ySize && actualPosY + horizontalChange > -1 && area.mazeInt[actualPosX, actualPosY + (int)horizontalChange] == '·'));
            if ((actualPosY + horizontalChange) < (area.ySize*2)-1 && (actualPosY + horizontalChange) > -1) {
                if (area.mazeInt[actualPosX, actualPosY + (int)horizontalChange] == '·') {
                    if (horizontalChange != 0) {
                        actualPosY = actualPosY + (int)horizontalChange;
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + (area.WallLenght / 2) * horizontalChange);
                        //  transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, area.whereStart300.z + (area.WallLenght / 2) * actualPosY);
                        //  transform.localPosition = transform.localPosition - area.transform.localPosition;
                    }

                }
            }
            //  transform.localPosition = transform.localPosition - area.transform.localPosition;
            // Debug.Log("Pos: " + actualPosX + " , " + actualPosY + "    " + transform.localPosition.x + " , " + transform.localPosition.z);
            // Debug.Log(area.name);
            //Debug.Log("ActualPosx :" + actualPosX + " == FinalX : " + finalX + " || ActualPosY : " + actualPosY +  " == FinalY : " + finalY + "                   totalposx " + transform.localPosition.x + "   totalposz   " + transform.localPosition.z);
            if (actualPosX == finalX && actualPosY == finalY) {
                AddReward(1f);
                Done();
                
            }

           /* int positiveVerticalSpace = 0;
            bool findWall = false;
            for (int i = actualPosY + 1; (i < (area.ySize * 2) - 1) && !findWall; i++) {
                if (area.mazeInt[actualPosX, i] == '·') {
                    positiveVerticalSpace++;
                }
                else if (area.mazeInt[actualPosX, i] == '#') {
                    findWall = true;
                }
            }
            Debug.Log("Espacios para arriba " + positiveVerticalSpace);
           // AddVectorObs(positiveVerticalSpace * (area.WallLenght / 2));

            int negativeVerticalSpace = 0;
            findWall = false;
            for (int i = actualPosY - 1; (i > -1) && !findWall; i--) {
                if (area.mazeInt[actualPosX, i] == '·') {
                    negativeVerticalSpace++;
                }
                else if (area.mazeInt[actualPosX, i] == '#') {
                    findWall = true;
                }
            } 
            Debug.Log("Espacios para abajo " + negativeVerticalSpace + "  actualposY");
           // AddVectorObs(negativeVerticalSpace * (area.WallLenght / 2));

            int positiveHorizontalSpace = 0;
            findWall = false;
            for (int i = actualPosX + 1; (i < (area.xSize * 2) - 1) && !findWall; i++) {
                if (area.mazeInt[i, actualPosY] == '·') {
                    positiveHorizontalSpace++;
                }
                else if (area.mazeInt[i, actualPosY] == '#') {
                    findWall = true;
                }
            }
            Debug.Log("Espacios para la derecha " + positiveHorizontalSpace);
            //AddVectorObs(positiveHorizontalSpace * (area.WallLenght / 2));

            int negativeHorizontalSpace = 0;
            findWall = false;
            for (int i = actualPosX - 1; (i > -1) && !findWall; i--) {
                if (area.mazeInt[i, actualPosY] == '·') {
                    negativeHorizontalSpace++;
                }
                else if (area.mazeInt[i, actualPosY] == '#') {
                    findWall = true;
                }
            }
            Debug.Log("Espacios para la izquierda " + negativeHorizontalSpace);
            //AddVectorObs(negativeHorizontalSpace * (area.WallLenght / 2));*/


        }

        public override void AgentReset()
        {
           // if (area.academy.trainingMode) {
                acumulateSteps += scenariosteps;
                scenarioIterations++;
                scenariosteps = 0;
                if (scenarioIterations == scenarioNumber) {

                    acumulateSteps = acumulateSteps / scenarioNumber;
                    Debug.Log("De media en " + scenarioNumber + " escenarios de " + area.xSize + " x, " + area.ySize  + "y.  Ha hecho un total de " + acumulateSteps + " pasos");
                    acumulateSteps = 0;
                    scenarioIterations = 0;
                   // scenariosteps

                }
          //  }
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

            // Update the step timeout if training
            if (area.academy.trainingMode) nextStepTimeout = GetStepCount() + stepTimeout;
        }

        void configureAreaCurricula()
        {
            
            
            int sizeX = (int)academy.FloatProperties.GetPropertyWithDefault("x_size", area.xSize);
            int sizeY = (int)academy.FloatProperties.GetPropertyWithDefault("y_size", area.ySize);
            //Debug.Log("x Size -> " + sizeX + " || " + sizeY + " <- y size");
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
