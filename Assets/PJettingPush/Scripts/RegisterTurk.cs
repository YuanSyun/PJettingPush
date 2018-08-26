
/*
	Date: Aug, 15, 2018
	Coder: YuanSyun Ye(yuansyuntw@gmail.com)
	Purpose: 向TurkManager.cs註冊此Turk。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
    public class RegisterTurk : MonoBehaviour
    {
        #region public variable

        public Transform tracker;
        public Camera turkCamera;
        public HaloHit haloDirection;
        public AirValveController airValveController;
        public JetButtonDetector detector;

        #endregion

        #region private variable

        private TurksManager turkManager;

        #endregion

        public void Add()
        {

            if (tracker == null)
            {
                _CheckSetting("not seted the tracker transform.");
            }

            if (turkCamera == null)
            {
                _CheckSetting("not seted the turk camera.");
            }

            if (detector == null)
            {
                _CheckSetting("not seted the jet button detector.");
                return;
            }

            if(airValveController == null){
                _CheckSetting("not set the air valve controller.");
            }

            if (TurksManager.instance != null)
            {
                turkManager = TurksManager.instance;
                turkManager.AddTurk(tracker, turkCamera, haloDirection, airValveController, detector.jetArduinoSerialPortName);
            }
            else
            {
                _CheckSetting("not found the turk manager");
            }
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            Add();
        }

        void _CheckSetting(string warning)
        {
            Debug.Log("TestAddTurk.cs - " + warning);
        }

    }
}


