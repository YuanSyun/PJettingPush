
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
    public class JetPointsRenderController : MonoBehaviour
    {

        #region public 

        public static JetPointsRenderController instance;
        public float jetInterval;
        [Header("Jet Points Render")]
        public GameObject jPointsRender;
        public Material matLineRender;

        #endregion





        #region private

        private List<JetPoints> listJetPoints = new List<JetPoints>();
        private JetPointsLineRender jPointsLineRender;

        #endregion





        #region public api

        public void AddJetPoints(JetPoints points)
        {
            listJetPoints.Add(points);
            _ShowJetPoints();
            //Debug.Log("JController list points number: " + listJetPoints.Count +", pop time: " + points.popingTime);
        }

        public JetPrePoping GetPrePop(){
            JetPrePoping jpp = new JetPrePoping();
            jpp.jController = this;
            return jpp;
        }

        #endregion






        #region lifecycle

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }else{
                if(instance != this){
                    Destroy(this);
                }
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        #endregion





        void _PickupTimeoutJetPoints()
        {
            if (listJetPoints.Count > 0)
            {
                JetPoints jpoints;
                JetPoint lastJPoint;
                for (int i = 0; i < listJetPoints.Count; i++)
                {
                    if (listJetPoints[i].listPoint.Count > 0)
                    {
                        jpoints = listJetPoints[i];
                        lastJPoint = jpoints.listPoint[jpoints.listPoint.Count - 1];
                        // Setting the timeout of the jet points.
                        jpoints.timeMax = lastJPoint.enableTime + lastJPoint.interval;

                        // Check the last enable jet point time.
                        if (Time.time > jpoints.timeMax)
                        {
                            Debug.Log("now time: " + Time.time + ", target time: " + jpoints.timeMax);
                            listJetPoints.RemoveAt(i);
                            continue;
                        }
                    }
                }
            }
        }

        void _ShowJetPoints()
        {
            // Clear timout points.
            _PickupTimeoutJetPoints();
            if (listJetPoints.Count == 0)
            {
                //Debug.Log("lsit jet point count: " + listJetPoints.Count);
                return;
            }

            // Instance a jet points render.
            GameObject jpr = Instantiate(jPointsRender, transform);

            // Setting jet points line render.
            jPointsLineRender = jpr.GetComponent<JetPointsLineRender>();
            if (jPointsLineRender != null)
            {
                if (matLineRender != null)
                {
                    jPointsLineRender.lineRender.material = matLineRender;
                }

                jPointsLineRender.SetRenderPoints(listJetPoints[0]);

                //Debug.Log("Set render points time: " + Time.time + ", last time : ");
            }else{
                Debug.Log("the jPointsRender doesnot found 'JetPointsLineRenderController'");
            }

            listJetPoints.RemoveAt(0);
        }
    }
}

