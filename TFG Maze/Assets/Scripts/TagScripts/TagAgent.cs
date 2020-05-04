using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TagGame
{
    public class TagAgent : Agent
    {


        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        //   [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] public bool tagged;
        [SerializeField] public Material materialNormal;
        [SerializeField] public Material materialTagged;
        private bool m_Jump;
        //  public bool taged { get; private set; }
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private TagArea area;
        private CharacterController m_CharacterController;
        private float horizontalChange;
        private float verticalChange;
        [Header("Training")]
        [Tooltip("number of steps to time out after in training")]
        public int stepTimeout = 300;


        public void setTagStatus(bool status)
        {
            if (status) {
                tagged = true;
                if(area.trainingMode)
                    AddReward(-1f);
            }
            else {
                tagged = false;
                if (area.trainingMode)
                    AddReward(1f);
            }
        }
        private float nextStepTimeOut;
        //   [SerializeField] private float m_StepInterval;


      


        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            

        }


        public override void InitializeAgent()
        {
            base.InitializeAgent();
            m_CharacterController = GetComponent<CharacterController>();
            area = GetComponentInParent<TagArea>();

           // agentParameters.maxStep = area.trainingMode ? 5000 : 0;
        
        }

        public override void AgentAction(float[] vectorAction)
        {


            horizontalChange = vectorAction[0];
            if (horizontalChange == 2) horizontalChange = -1f;

            verticalChange = vectorAction[1];
            if (verticalChange == 2) verticalChange = -1f;


            ProcessMovement();


        }

        private void ProcessMovement()
        {
            float speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_MoveDir.x = verticalChange * speed;
            m_MoveDir.z = horizontalChange * speed;

            m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            

        }

        public override void AgentReset()
        {
            
            area.resetAgent(agent: this);

            if (area.trainingMode) nextStepTimeOut = GetStepCount() + stepTimeout;
        }


    }
}


