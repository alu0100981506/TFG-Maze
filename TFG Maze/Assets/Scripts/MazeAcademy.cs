using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


namespace Maze
{
    public class MazeAcademy : Academy
    {

       // [HideInInspector]
        //public GameObject[] agents;
        [HideInInspector]
        public MazeArea[] listArea;

        [Tooltip("If true, enable training mode")]
        public bool trainingMode;

        public bool reset = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (reset) {
                reset = false;
                eset();
                
            }
        }

        public override void AcademyReset()
        {
            
            //agents = GameObject.FindGameObjectsWithTag("agent");
            listArea = FindObjectsOfType<MazeArea>();
            foreach (var fa in listArea) {
                fa.ResetMazeArea();
            }
        }

        public void eset()
        {
            listArea = FindObjectsOfType<MazeArea>();
            foreach (var fa in listArea) {
                fa.ResetMazeArea();
            }

        }
    }
}
