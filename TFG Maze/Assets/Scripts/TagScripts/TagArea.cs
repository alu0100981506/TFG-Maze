using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TagGame
{

    public class TagArea : MonoBehaviour
    {
        [Tooltip("If true, enable training mode")]
        public bool trainingMode;
        [SerializeField] public int numTaggedAgents;
        public List<TagAgent> TagAgents { get; private set; }
        public TagAcademy TagAcademy { get; private set; }
        //public bool trainingMode;

        private void Awake()
        {
            TagAgents = transform.GetComponentsInChildren<TagAgent>().ToList();
            Debug.Assert(TagAgents.Count > 0, " No agents found");

            TagAcademy = FindObjectOfType<TagAcademy>();

            //Aqui supongo que puedo poner la corrutina que gestiona el tiempo y cuando se han de eliminar los agentes taggeados
        }

        public void resetAgent(TagAgent agent)
        {
            float separation_x = 19f / 5;
            float separation_y = 38 / Mathf.Floor(TagAgents.Count / 5);
            Vector3 position = new Vector3(9, 2, 19);

            int row = (int)Mathf.Floor(TagAgents.IndexOf(agent) / 5);
            int column = TagAgents.IndexOf(agent) - (row * 5);
            position.x = column * separation_x;
            position.z = row * separation_y;
            agent.transform.position = position;

            // Vector3 possitionOffset = Vector3.right * (TagAgents.IndexOf(agent) - TagAgents.Count / 2f) * 10f;


        }

        public void ResetTagAgents(TagAgent[] agents)
        {
            foreach (TagAgent agent in agents) {

                resetAgent(agent);

            } 
            SetTaggedAgents();
        }

        private void SetTaggedAgents()
        {
            for(int i = 0; i < numTaggedAgents ; i++) {
                TagAgents[UnityEngine.Random.Range(0, TagAgents.Count - 1)].tagged = true;
            }
        }
    }

}

