
/*
	Date: Aug, 13, 2018
	Coder: YuanSyun Ye(yuansyuntw@gmail.com)
	Purpose: 存放目前在系統中的Human Turks資訊。tracker transform、Camera、Serial Port.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace edu.nctu.nllab.PJettingPush
{
    public class Turk
    {
        public Transform transform;
        public Camera camera;
		public HaloHit haloHiter;
        public AirValveController valveController;
        public string serialPort;

        public Turk(Transform _transform, Camera _camera, HaloHit _director, AirValveController _valveController, string _serialPort)
        {
            this.transform = _transform;
            this.camera = _camera;
            this.haloHiter = _director;
            this.valveController = _valveController;
            this.serialPort = _serialPort;
        }

		public void RegisterHitPoint(HitPoint hiter){
			if(haloHiter != null){
				haloHiter.AddTarget(hiter);
			}
            if(valveController != null){
                valveController.AddJetSignal(hiter);
            }
		}

        public void ReleaseHitPoint(HitPoint hiter)
        {
			if(haloHiter != null){
				haloHiter.RemoveTarget(hiter);
			}
            if(valveController != null){
                valveController.RemoveJetSignal(hiter);
            }
        }
    }
}

