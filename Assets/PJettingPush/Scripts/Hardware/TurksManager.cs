
/*
	Date: Aug, 13, 2018
	Coder: YuanSyun Ye(yuansyuntw@gmail.com)
	Purpose: 存放目前在系統中的Human Turks資訊。tracker transform、Camera、Serial Port.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


namespace edu.nctu.nllab.PJettingPush
{
    public class TurksManager : MonoBehaviour
    {
        #region public variable

        public static TurksManager instance;
        public UnityEvent readyEvents;

		#endregion


        #region private variable

        private List<Turk> turks = new List<Turk>();

        #endregion

        #region api

        public void AddTurk(Transform _transform, Camera _camera, HaloHit direction, AirValveController _valveController, string _serialPort)
        {
            Turk turk = new Turk(_transform, _camera, direction, _valveController, _serialPort);
            turks.Add(turk);
            Debug.Log("Add a turk: " + _transform.name);
        }

        public Turk GetNearestTurk(Vector3 position)
        {
            Turk nearestTurk = null;
            float nearestDistance = float.MaxValue;

            float distance;
            if (turks.Count > 0)
            {
                foreach (Turk t in turks)
                {
                    distance = Vector3.Distance(t.transform.position, position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestTurk = t;
                    }
                }
            }

            return nearestTurk;
        }

        #endregion

        #region lifecycle

        void Awake()
        {
            if (TurksManager.instance == null)
            {
                TurksManager.instance = this;
            }
            else
            {
                if (TurksManager.instance != this)
                {
                    Destroy(this);
                }
            }
        }

        // Use this for initialization
        void Start()
        {
            readyEvents.Invoke();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion
    }
}