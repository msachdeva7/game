using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
	public class CustomCarControl : MonoBehaviour
    {
		public GameManager gm;
        private CarController m_Car; // the car controller we want to use

		/*
		private void Start()
		{
			gm = GameObject.Find ("GameManager").GetComponent<GameManager> ();
			if(gm == null) {
				Debug.Log("Cannot find gm!");
			}
		} */

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }
			
        private void FixedUpdate()
        {
            // pass the input to the car!
			PlayerCommands cmds = gm.inter.GetCommands();
			float h = Convert.ToSingle(cmds.right) - Convert.ToSingle(cmds.left);
			float v = Convert.ToSingle(cmds.forward) - Convert.ToSingle(cmds.backward);
			// We can also have handbrake controls if we want them:
			float handbrake = 0f;
            m_Car.Move(h, v, v, handbrake);
        }
    }
}
