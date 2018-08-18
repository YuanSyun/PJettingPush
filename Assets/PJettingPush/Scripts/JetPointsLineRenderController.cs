
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: Controll the LineRender of componment;
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace edu.nctu.nllab.PJettingPush
{
    public class JetPointsLineRenderController : MonoBehaviour
    {

        #region public variable
        public LineRenderer lRender;
        [HideInInspector]
        public UnityAction action;
		public FaceTransform projectorVisualController;
        #endregion

        #region private variable
        private JetPoints jPoints;
        private int removeIndex = 0;
        private int removeCount = 0;
        private List<Transform> pTrans = new List<Transform>();
        private List<Vector3> ps = new List<Vector3>();
        private UnityEvent eventsTimeout = new UnityEvent();
        #endregion

        #region public api
        public void SetRenderPoints(JetPoints points)
        {
            jPoints = points;
			// Sort jet points.
			List<JetPoint> list = jPoints.GetSortListPoint;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    pTrans.Add(list[i].trans);
                    ps.Add(list[i].trans.position);
                }
				removeCount = 0;
				lRender.positionCount = list.Count;
                lRender.SetPositions(ps.ToArray());

				_RefreshVisualPosition();
				projectorVisualController.transform.gameObject.SetActive(true);
				lRender.enabled = true;
            }
            else
            {
                Debug.Log("the jPoints doesnt have point.");
            }
        }
        public void RefreshTimeoutEvent()
        {
            eventsTimeout.AddListener(action);
        }

		public void SetProjectorVisualCamera(Transform trans){
			projectorVisualController.target = trans;
		}
        #endregion

        #region lifecycle
        // Use this for initialization
        void Start()
        {
            if (lRender == null)
            {
                _CheckSetting("lRender not seted");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (ps.Count > 0)
            {
                _CheckTiming();
                _RefreshPointsPosition();
            }

        }
        #endregion

        void _CheckSetting(string warning)
        {
            Debug.Log("JetPointsLineRenderController.cs - " + warning);
            Destroy(this);
        }

        void _CheckTiming()
        {
            if (Time.time > jPoints.popingTime + jPoints.listPoint[removeCount].enableTime)
            {
				//Debug.Log("Remove :" + jPoints + "count: " + removeCount);
                _RemoveAIndexPoint();
            }
			//Debug.Log("Now time: " + Time.time + ", target time: " + (jPoints.popingTime + jPoints.listPoint[removeCount].enableTime));
        }

        void _RemoveAIndexPoint()
        {
            if (ps.Count > 0)
            {
                ps.RemoveAt(removeIndex);
                pTrans.RemoveAt(removeIndex);
                removeCount++;
				//Debug.Log("ps number: " + ps.Count);
                if (ps.Count > 0)
                {
					lRender.positionCount = ps.Count;
                    lRender.SetPositions(ps.ToArray());
					_RefreshVisualPosition();
                }
                else
                {
                    _Timeout();
                }
            }
            else
            {
                lRender.enabled = false;
            }
        }

        void _Timeout()
        {
            lRender.enabled = false;
			projectorVisualController.transform.gameObject.SetActive(false);
            eventsTimeout.Invoke();
            //Debug.Log("Timeout");
        }

        void _RefreshPointsPosition()
        {
			//Debug.Log("_RefreshPointsPosition");
			for(int i=0; i<ps.Count; i++){
				ps[i] = pTrans[i].position;
			}
			lRender.SetPositions(ps.ToArray());
        }

		void _RefreshVisualPosition(){
			projectorVisualController.transform.position = pTrans[removeIndex].position;
		}

		int _CompareJetPoint(JetPoint x, JetPoint y){
			return x.enableTime.CompareTo(y.enableTime);
		}
    }
}

