
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: Controlling the jeted time.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
    public class JettingController : MonoBehaviour
    {

        #region public 
        public float jetInterval;

        [Header("Jet Points Render")]
        public GameObject jPointsRender;
        public Material matLineRender;
        public float widthLineRender = 0.1f;
		public Transform ProjectorVisualCamera;
        #endregion

        #region private
        private List<JetPoints> listJetPoints = new List<JetPoints>();
        private int indexWaitting = -1;
		private JetPointsLineRenderController jplrController;
        #endregion

        #region public api
        public void AddJetPoints(JetPoints points)
        {
            listJetPoints.Add(points);
            //Debug.Log("JController list points number: " + listJetPoints.Count +", pop time: " + points.popingTime);
        }
        public void NextJetPoints()
        {
			//Timeout
            Debug.Log("Remove :" + indexWaitting);
            listJetPoints.RemoveAt(indexWaitting);
            indexWaitting = -1;
        }
        #endregion

        #region lifecycle
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // Get the next jet points.
            if (indexWaitting == -1)
            {
                indexWaitting = _CheckJetPointsList();
                _ShowJetPoints();
            }
        }
        #endregion

        int _CheckJetPointsList()
        {
            float closestTime = float.MaxValue;
            int index = -1;

            if (listJetPoints.Count > 0)
            {
                for (int i=0; i<listJetPoints.Count; i++)
                {
                    if (listJetPoints[i].listPoint.Count > 0)
                    {
                        if ((listJetPoints[i].popingTime + listJetPoints[i].listPoint[0].enableTime) < closestTime)
                        {
                            closestTime = (listJetPoints[i].popingTime + listJetPoints[i].listPoint[0].enableTime);
                            index = i;
                        }
                    }
                }
            }

            return index;
        }

        void _ShowJetPoints()
        {
            if (indexWaitting == -1)
            {
                return;
            }

			if(jplrController == null){
				GameObject jpr = Instantiate(jPointsRender, transform);
            	jplrController = jpr.GetComponent<JetPointsLineRenderController>();
				jplrController.action += this.NextJetPoints;
				jplrController.RefreshTimeoutEvent();
				jplrController.SetProjectorVisualCamera(ProjectorVisualCamera);
			}

            if (widthLineRender > 0)
            {
                jplrController.lRender.startWidth = widthLineRender;
            }
            if (matLineRender != null)
            {
                jplrController.lRender.material = matLineRender;
            }
            jplrController.SetRenderPoints(listJetPoints[indexWaitting]);

            Debug.Log("Show New Jet Points: " + indexWaitting);
        }
    }
}

