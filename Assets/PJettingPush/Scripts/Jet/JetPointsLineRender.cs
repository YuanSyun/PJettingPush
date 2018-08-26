
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: 顯示噴氣點，顯示方式為LineRedner搭配一個FaceTransform.cs。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
    public class JetPointsLineRender : MonoBehaviour
    {

        #region public variable

        public LineRenderer lineRender;
        public HitPoint hitPoint;

        #endregion





        #region private variable

        private JetPoints jPoints;
        private List<Transform> pTrans = new List<Transform>();
        private List<Vector3> ps = new List<Vector3>();
        private bool flagSetup = false;
        private List<JetPoint> listJetPoint;
        private bool flagWaitingDestory = false;

        #endregion





        #region public api

        public void SetRenderPoints(JetPoints points)
        {
            jPoints = points;
            listJetPoint = jPoints.GetSortListPoint;
            jPoints.timeMax = _RefreshTimeoutTransforms();
            //Debug.Log("setup time: " + Time.time + ", final time: " + lastTime);
        }

        public void SetProjectorVisualCamera(Transform trans)
        {

        }

        #endregion






        #region lifecycle

        // Use this for initialization
        void Awake()
        {
            if (lineRender == null)
            {
                _CheckSetting("lineRender not seted");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (flagSetup)
            {
                if (Time.time > jPoints.timeMax)
                {
                    _Timeout();
                }
                else
                {
                    // Refresh the last enable time.
                    jPoints.timeMax = _RefreshTimeoutTransforms();
                    _RefreshLineRender();
                }

                //Debug.Log("now time: " + Time.time + ", enable time: " + (listJetPoint[0].enableTime + jPoints.popingTime) + ",hit flag: " + listJetPoint[0].flagHit);
            }
        }

        #endregion






        void _CheckSetting(string warning)
        {
            Debug.Log("JetPointsLineRenderController.cs - " + warning);
            Destroy(gameObject);
        }

        void _Timeout()
        {
            // Send the signal to air valve controller to destory this gameobject.
            //Debug.Log("time out: " + Time.time + ", last time: " + lastTime);
            if (!flagWaitingDestory)
            {
                flagWaitingDestory = true;
                hitPoint.RemoveSignal();
                Destroy(gameObject);
            }
        }

        void _RefreshLineRender()
        {
            //Debug.Log("_RefreshPointsPosition");
            ps.Clear();
            int numPosition = pTrans.Count;
            for (int i = 0; i < numPosition; i++)
            {
                if (pTrans[i] != null)
                {
                    ps.Add(pTrans[i].position);
                }
            }
            lineRender.positionCount = ps.Count;
            if (lineRender.positionCount > 0)
            {
                lineRender.SetPositions(ps.ToArray());
            }

        }

        int _CompareJetPoint(JetPoint x, JetPoint y)
        {
            return x.enableTime.CompareTo(y.enableTime);
        }

        float _RefreshTimeoutTransforms()
        {
            flagSetup = false;

            float maxTimeout = -1f;
            float timeMinEnable = float.MaxValue;
            float timeTemp;
            int inedxMinEnable = -1;
            int indexMaxEnable = -1;

            pTrans.Clear();
            if (listJetPoint.Count > 0)
            {
                for (int i = 0; i < listJetPoint.Count; i++)
                {
                    timeTemp = listJetPoint[i].enableTime;

                    // Show the jet point, if it is not timeout and it hits something.
                    Debug.Log("now time: " + Time.time + ", enable time: " + timeTemp + ", hit flag: " + listJetPoint[i].flagHit);
                    if (Time.time < timeTemp && listJetPoint[i].flagHit)
                    {
                        // Add the unreach transform.
                        pTrans.Add(listJetPoint[i].trans);

                        // Find min time index
                        if (timeTemp < timeMinEnable)
                        {
                            timeMinEnable = timeTemp;
                            inedxMinEnable = i;
                        }

                        // Find max time index
                        if (timeTemp > maxTimeout)
                        {
                            maxTimeout = timeTemp;
                            indexMaxEnable = i;
                        }
                    }
                }

                //Debug.Log("the min enable index: " + inedxMinEnable);

                // Move the visual object to the min time index.
                if (inedxMinEnable != -1)
                {
                    // Set the jet signal of the jeted detecor.
                    if (hitPoint != null)
                    {
                        hitPoint.SetSignal(listJetPoint[inedxMinEnable]);
                    }
                }
                else
                {
                    // Hide the visual position.
                    if (hitPoint != null)
                    {
                        hitPoint.SetSignal(null);
                    }
                }
            }
            else
            {
                Debug.Log("the jPoints doesnt have point.");
                _Timeout();
            }

            flagSetup = true;

            if (maxTimeout == -1f)
            {
                // if not found any jet point to show, set the max enable time is the max jet points time.
                maxTimeout = jPoints.timeMax;
            }
            else
            {
                // Calculate the max timeout, the max enable added the interval of the last jet point.
                maxTimeout += jPoints.listPoint[indexMaxEnable].interval;
            }
            return maxTimeout;
        }
    }
}

