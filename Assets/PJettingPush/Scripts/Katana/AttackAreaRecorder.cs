
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 7, 2018
	Purpose: 
        由於PJettingPush需要偵測武器揮動的範圍，藉此偵測範圍內是否有障礙物存在，並預先顯示撞擊部分。此程式碼用於錄製Animator的武器揮動範圍。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace edu.nctu.nllab.PJettingPush
{
    public class AttackAreaRecorder : MonoBehaviour
    {
        #region public variable
        public Animator animator;
        public string nameRecordedAnimation;
        public Transform transStartNode;
        public Transform transEndNode;
        public Transform transRecorderBase;
        public string pathRecordedPosition;
        public float rateRecord = 0.1f;
        public GameObject debugLineRender;
        [HideInInspector]
        public static float timeWaitLoadAnimation = 0.025f;

        #endregion





        #region private variable

        private bool flagFinish = false;

        StreamWriter sw;

        #endregion



        #region lifecycle

        // Use this for initialization
        void Start()
        {
            if (transRecorderBase == null)
            {
                _CheckSetting("transRecorderBase not set", true);
            }
            if (animator == null)
            {
                _CheckSetting("animator not setting", true);
            }
            if (nameRecordedAnimation == "")
            {
                _CheckSetting("nameRecordedAnimation not setting", true);
            }
            if (transStartNode == null)
            {
                _CheckSetting("transStartNode not setting", true);
            }
            if (transEndNode == null)
            {
                _CheckSetting("transEndNode not setting", true);
            }

            StartCoroutine(_Recorder());
        }

        // Update is called once per frame
        void Update()
        {
            if (flagFinish)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (sw != null)
            {
                sw.Flush();
                sw.Close();
            }
        }

        #endregion




        void _CheckSetting(string warning, bool destroy)
        {
            Debug.Log("AttackAreaRecorder.cs - " + warning);
            if (destroy)
            {
                Destroy(this);
            }
        }

        IEnumerator _Recorder()
        {
            Debug.Log("Start recording");
            List<Vector3> listStartNode = new List<Vector3>();
            List<Vector3> listEndNode = new List<Vector3>();

            if (rateRecord <= 0.0f)
            {
                rateRecord = 0.05f;
            }

            animator.Play(nameRecordedAnimation);

            // Wait to play animation
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(nameRecordedAnimation))
            {
                yield return new WaitForSeconds(rateRecord);
            }

            // Recording.
            Vector3 startNode;
            Vector3 endNode;
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(nameRecordedAnimation))
            {
                startNode = transRecorderBase.InverseTransformPoint(transStartNode.position.x, transStartNode.position.y, transStartNode.position.z);
                endNode = transRecorderBase.InverseTransformPoint(transEndNode.position.x, transEndNode.position.y, transEndNode.position.z);

                // Save the position of the node.
                listStartNode.Add(startNode);
                listEndNode.Add(endNode);

                yield return new WaitForSeconds(rateRecord);

                // Show debug line render.
                if (debugLineRender != null)
                {
                    GameObject obj = Instantiate(debugLineRender, transform);
                    LineRenderer lineRender = obj.GetComponent<LineRenderer>();
                    lineRender.positionCount = 2;
                    lineRender.SetPosition(0, transRecorderBase.TransformPoint(startNode.x, startNode.y, startNode.z));
                    lineRender.SetPosition(1, transRecorderBase.TransformPoint(endNode.x, endNode.y, endNode.z));
                }
            }

            // Save position
            string path;
            if (pathRecordedPosition != "")
            {
                path = Application.dataPath + "/" + pathRecordedPosition + "/" + nameRecordedAnimation + ".txt";
            }
            else
            {
                path = Application.dataPath + "/" + nameRecordedAnimation + ".txt";
            }

            // Create the file.
            sw = File.CreateText(path);

            // Write the recording rate.
            sw.WriteLine(rateRecord.ToString());

            string temp = "";
            for (int i = 0; i < listStartNode.Count; i++)
            {
                temp = listStartNode[i].x + " " + listStartNode[i].y + " " + listStartNode[i].z + " ";
                temp += listEndNode[i].x + " " + listEndNode[i].y + " " + listEndNode[i].z;
                sw.WriteLine(temp);
            }

            // Flush buffer
            sw.Flush();
            sw.Close();
            sw = null;

            Debug.Log("Finsish recording. file path: " + path);
            flagFinish = true;
        }
    }
}

