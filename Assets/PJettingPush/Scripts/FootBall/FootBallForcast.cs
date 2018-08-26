
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 26, 2018
	Purpose: 利用JetPointsRenderController.cs產生足球落點。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
    public class FootBallForcast : MonoBehaviour
    {

        [Header("Needed Information")]
		public ShootAI shootAI;
		public Transform ballIntersection;

        [Header("Force settting")]
        public float force;
        public float forceInterval;
        private float forceAreaSize;
        public JetPoint.Type forceType = JetPoint.Type.Pluse;
        


        private JetPointsRenderController jprcontroller;



        public void Forcast(){

            JetPoint point = new JetPoint(  true,
                                            Time.time + shootAI.GetRemainTime,
                                            force,
                                            forceInterval,
                                            forceAreaSize,
                                            forceType,
                                            shootAI._ballTarget);

            JetPrePoping prepoing = jprcontroller.GetPrePop();
            prepoing.jPoints.listPoint.Add(point);
            prepoing.PrePoping();
        }



        // Use this for initialization
        void Start()
        {
            jprcontroller = JetPointsRenderController.instance;

            if(jprcontroller == null){
                _CheckSetting("JetPointsRenderController.cs not found", true);
            }

            if(shootAI == null){
                _CheckSetting("doesnt set the ShootAI", true);
            }
            forceAreaSize = shootAI.transform.localScale.x;

            if(ballIntersection == null){
                _CheckSetting("dosent set the ball intersection", true);
            }

            // Register the forcasting result.
            Shoot.EventShoot += Forcast;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void _CheckSetting(string _warning, bool _destroy){
            Debug.Log("FootBallForcast.cs - " + _warning);
            if(_destroy)
                Destroy(this);
        }
    }
}

