using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TagGame
{

    public class TagPlayer : TagAgent
    {
        [Header("Input Bindings")]
        public InputAction horizontalInput;
        public InputAction verticalInput;
   



        public override void InitializeAgent()
        {
            base.InitializeAgent();
            horizontalInput.Enable();
            verticalInput.Enable();
    

        }

        public override float[] Heuristic()
        {
            //Horizontal 1 == right, 0 == nothing, 2 == left
            float horizontalValue = Mathf.Round(horizontalInput.ReadValue<float>());
            //Vertical 1 == up, 0 == nothing, 2 == down
            float verticalValue = Mathf.Round(verticalInput.ReadValue<float>());


            if (horizontalValue == -1f) horizontalValue = 2f;
            if (verticalValue == -1f) verticalValue = 2f;

            return new float[] {horizontalValue,verticalValue};
        }

        private void OnDestroy()
        {
            horizontalInput.Disable();
            verticalInput.Disable();
        }

    }
}
