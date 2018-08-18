
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: Pre-poping a JetPoints to JettingController.cs
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace edu.nctu.nllab.PJettingPush
{
    public class JetPrePoping : MonoBehaviour
    {

		#region public variable
        public JettingController jController;
		public JetPoints jPoints;
		#endregion

		#region public api
		public void PrePoping(){
			jPoints.popingTime = Time.time;
			jController.AddJetPoints(jPoints);
			//Debug.Log("PrePoing: " + transform.name);
		}
		#endregion

		#region lifecycle
        // Use this for initialization
        void Start()
        {
			if(jController == null){
				_CheckSetting("jController not seted");
			}
			if(jPoints == null){
				_CheckSetting("JPoints not seted");
			}
        }

        // Update is called once per frame
        void Update()
        {

        }
		#endregion

		void _CheckSetting(string warning){
			Debug.Log("JetPrePoing.cs - " + warning);
			Destroy(this);
		}
    }

}

