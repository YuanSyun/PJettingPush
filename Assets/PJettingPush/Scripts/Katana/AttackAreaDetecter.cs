
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 7, 2018
	Purpose: 
		PJettingPush是研究利用空氣模擬揮擊效果，而此程式碼依AttackAreaRecorder.cs產生出來的偵測位置來判斷武器揮擊範圍是否有障礙物。
		一個攻擊對應一個Dectector，直到實際動作結束才刪除此偵測程式。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace edu.nctu.nllab.PJettingPush
{
    public class AttackAreaDetecter : MonoBehaviour
    {

        #region public variable

        public string pathRecorderFolder;
        public LayerMask lasyerDetector;
        public GameObject objectHiter;
        public GameObject debugHitedLineRender;

        [Header("Force Setting")]
        public JetPoint.Type forceType = JetPoint.Type.Pluse;
        public float intervalForce = 1.0f;
        public float jettingForce = 1.0f;
        public float jettingSize = 1.0f;

        [HideInInspector]
        public GameObject objectJetController;
        [HideInInspector]
        public Transform transRecorderBase;

        #endregion





        #region private variable
        
        private bool flagSetup = false;
        private float timeStart;
        private float timeWaitToPlayAnimation = 0.0f;
        private float timeEnd;
        private string nameAnimation;
        private float rateRecord; // is the recorder's rateRecord, It's mean the dectioned points's interval.
        private List<Vector3> listStartNode = new List<Vector3>();
        private List<Vector3> listEndNode = new List<Vector3>();
        private List<JetPoint> listJetPoint = new List<JetPoint>();
        private JetPrePoping jPrepoping = null;

        #endregion





        #region api
        public void ShowDetectAttackArea(string fileName, Transform transBase, float timeWaitPlay)
        {
            //Debug.Log("detect");

            this.transRecorderBase = transBase;
            this.timeStart = Time.time;
            this.nameAnimation = fileName;
            this.timeWaitToPlayAnimation = timeWaitPlay;

            if (transRecorderBase == null)
            {
                _ChecksSetting("The transform recorder base not set", true);
            }

            StartCoroutine(_Setting());
        }
        #endregion





        #region lifecyccle
        // Use this for initialization
        void Awake()
        {
            if (objectHiter == null)
            {
                _ChecksSetting("objectHiter not setting", true);
            }

            JetPointsRenderController jcontroller = JetPointsRenderController.instance;
            if (jcontroller == null)
            {
                jcontroller = Instantiate(objectJetController).GetComponent<JetPointsRenderController>();
            }
            if (jcontroller != null)
            {
                jPrepoping = jcontroller.GetPrePop();
                if (jPrepoping == null)
                {
                    _ChecksSetting("doesnt get JetPrepoping.", false);
                }
            }
            else
            {
                _ChecksSetting("the objectJetController doesnot contain JettingController.cs", false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (flagSetup)
            {
                if (Time.time > timeEnd)
                {
                    _Timeout();
                }
                if (listJetPoint.Count > 0)
                {
                    _DectectHit();
                }
            }
        }
        #endregion





        void _ChecksSetting(string warning, bool destory)
        {
            Debug.Log("AttackAreaDetecer.cs - " + warning);
            if (destory)
            {
                Destroy(this);
            }
        }

        IEnumerator _Setting()
        {
            // Stop to update.
            flagSetup = false;

            // Set the reading path.
            string path;
            if (pathRecorderFolder != "")
            {
                path = Application.dataPath + "/" + pathRecorderFolder + "/" + nameAnimation + ".txt";
            }
            else
            {
                path = Application.dataPath + "/" + nameAnimation + ".txt";
            }

            // Read the file.
            string[] listPos = null;
            try
            {
                listPos = File.ReadAllLines(path);
            }
            catch (FileNotFoundException e)
            {
                Debug.Log(e.ToString());
                yield break;
            }

            // Get the rate of recording.
            rateRecord = float.Parse(listPos[0]);
            timeEnd = (listPos.Length - 1) * rateRecord + timeStart;

            Vector3 start = new Vector3();
            Vector3 end = new Vector3();
            GameObject hitObject;
            string[] datas;

            // Parse the file.
            for (int i = 1; i < listPos.Length; i++)
            {
                datas = listPos[i].Split(' ');
                if (datas.Length == 6)
                {
                    start.x = float.Parse(datas[0]);
                    start.y = float.Parse(datas[1]);
                    start.z = float.Parse(datas[2]);

                    // Transform the point from local to world.
                    start = transRecorderBase.TransformPoint(start.x, start.y, start.z);

                    end.x = float.Parse(datas[3]);
                    end.y = float.Parse(datas[4]);
                    end.z = float.Parse(datas[5]);

                    // Transform the point from local to world.
                    end = transRecorderBase.TransformPoint(end.x, end.y, end.z);

                    listStartNode.Add(start);
                    listEndNode.Add(end);

                    // Prepare hited objects.
                    hitObject = Instantiate(objectHiter, start, Quaternion.identity, transform);
                    hitObject.SetActive(true);

                    // Set jet point.
                    JetPoint point = new JetPoint((timeStart + timeWaitToPlayAnimation + i * rateRecord), jettingForce, rateRecord, jettingSize, forceType, hitObject.transform);
                    listJetPoint.Add(point);

                    // Show debug line render. 
                    if (debugHitedLineRender != null)
                    {
                        LineRenderer lr = Instantiate(debugHitedLineRender, transform).GetComponent<LineRenderer>();
                        if (lr != null)
                        {
                            Debug.Log("show debug hited line render.");
                            lr.positionCount = 2;
                            lr.SetPosition(0, start);
                            lr.SetPosition(1, end);
                        }
                    }
                }
                else
                {
                    _ChecksSetting("The file format does not correct. path: " + path, true);
                }
            }

            // Update hited position.
            _DectectHit();

            // Show the predicted points to jet.
            if (jPrepoping != null)
            {
                jPrepoping.jPoints.listPoint = listJetPoint;
                jPrepoping.PrePoping();
                //Debug.Log("Prepoping jetpoints");
            }

            // Recorve to update.
            flagSetup = true;
        }

        void _DectectHit()
        {
            if (listJetPoint.Count > 0)
            {
                RaycastHit hiter;

                for (int i = 0; i < listJetPoint.Count; i++)
                {
                    // Check hit object.
                    Debug.DrawLine(listStartNode[i], listEndNode[i], Color.red);
                    if (Physics.Linecast(listStartNode[i], listEndNode[i], out hiter, lasyerDetector))
                    {
                        // Show the jet point
                        listJetPoint[i].Refresh(true, hiter.point);
                    }
                    else
                    {
                        // Hide the jet point
                        listJetPoint[i].Refresh(false);
                    }
                }
            }
        }

        void _Timeout()
        {
            //Debug.Log("time out");
            Destroy(gameObject);
        }
    }
}

