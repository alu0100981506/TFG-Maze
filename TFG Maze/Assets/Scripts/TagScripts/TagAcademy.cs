using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace TagGame
{
    public class TagAcademy : Academy
    {

        [HideInInspector]
        public TagAgent[] agents;
        [HideInInspector]
        public TagGame.TagArea[] listArea;
        public override void AcademyReset()
        {
            // ClearObjects(GameObject.FindGameObjectsWithTag("food"));
            // ClearObjects(GameObject.FindGameObjectsWithTag("badFood"));

            agents = FindObjectsOfType<TagGame.TagAgent>();
            listArea = FindObjectsOfType<TagGame.TagArea>();
            foreach (var fa in listArea) {
                fa.ResetTagAgents(agents);
            }

            //   totalScore = 0;
        }
    }
}
