
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 4, 2018
	Purpose: Predice projectile time and position;
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
    public class ProjectilePrediction : MonoBehaviour
    {
        #region public variable

        public BM_ProjectileSystem_C projectSystem;
        public LineRenderer lineRenderer;
        public int numberLineSegment = 20;
        public GameObject objectHiter;
        public GameObject jettingController;
        public LayerMask layerMask;
        public float timeWaitProjectile = 0.0f;

        [Header("Force Setting")]
        public JetPoint.Type forceType = JetPoint.Type.Pluse;
        public float jettingForce = 1.0f;
        public float forceInterval = 1.0f;
        public float jettingSize = 1.0f;

        #endregion





        #region private variable

        private Vector3 posOriginal;
        private Vector3 dirOriginal;
        private Vector3 currentPos;
        private Vector3 velOriginal;
        private Vector3 gravity;
        private JetPoint jetPoint;
        private float timeStart;
        private float timeTotal;
        private List<Vector3> pointsTrajctory = new List<Vector3>();
        private float deltaTracjectory;
        private JetPrePoping jPrepoping;

        #endregion





        #region lifecycle

        void Awake()
        {
            if (projectSystem == null)
            {
                _CheckSetting("not set project system", true);
            }
            projectSystem.gameObject.SetActive(false);

            if (lineRenderer == null)
            {
                _CheckSetting("not set lineRenderer", false);
            }

            JetPointsRenderController jetController = JetPointsRenderController.instance;
            if (jetController == null)
            {
                jetController = Instantiate(jettingController).GetComponent<JetPointsRenderController>();
            }
            if (jettingController != null)
            {
                jPrepoping = jetController.GetPrePop();
            }
            else
            {
                _CheckSetting("not set jet controller", true);
            }
        }

        // Use this for initialization
        void Start()
        {
            // Refresh the total time(the start time + the max hit time).
            timeStart = Time.time;
            timeTotal = _DrawTrajectory();
            StartCoroutine(StartTimeProjectie());
            //Debug.Log("total time: " + timeTotal);
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time > timeTotal)
            {
                _Timeout();
            }

            // Refresh the total time(the start time + the max hit time).
            timeTotal = _DetectHit();
        }
        #endregion





        void _Timeout()
        {
            // Destroy other object.
            Destroy(gameObject);
        }

        void _CheckSetting(string warning, bool destory)
        {
            Debug.Log("ProjectilePrediction.cs - " + warning);
            if (destory)
                Destroy(this);
        }

        float _DrawTrajectory()
        {
            posOriginal = transform.position;
            dirOriginal = transform.forward;
            velOriginal = dirOriginal * projectSystem.Speed;
            gravity = new Vector3(0.0f, -projectSystem.Gravity, 0.0f);
            deltaTracjectory = 1.0f / velOriginal.magnitude;

            Vector3 position = posOriginal;
            Vector3 velocity = velOriginal;
            float interval;

            interval = numberLineSegment * deltaTracjectory + timeWaitProjectile;
            for (int i = 0; i < numberLineSegment; i++)
            {
                pointsTrajctory.Add(position);

                position += velocity * deltaTracjectory + 0.5f * gravity * deltaTracjectory * deltaTracjectory;
                velocity += gravity * deltaTracjectory;
            }
            return interval + timeStart;
        }

        float _DetectHit()
        {
            if (lineRenderer == null)
            {
                return 0.0f;
            }

            List<Vector3> showPoins = new List<Vector3>();
            RaycastHit hitInfo;
            Vector3 position = pointsTrajctory[pointsTrajctory.Count - 1];
            Vector3 lastPos = pointsTrajctory[0];
            float enableTime = timeStart + deltaTracjectory * pointsTrajctory.Count + timeWaitProjectile;
            bool flagHit = false;

            for (int i = 1; i < pointsTrajctory.Count; i++)
            {
                if ((Time.time) < (timeStart + timeWaitProjectile + i * deltaTracjectory))
                {
                    position = pointsTrajctory[i];

                    // Check hit something. 
                    if (Physics.Linecast(lastPos, position, out hitInfo, layerMask))
                    {
                        position = hitInfo.point;
                        showPoins.Add(position);
                        enableTime = timeStart + deltaTracjectory * i + timeWaitProjectile;

                        flagHit = true;
                        break;
                    }

                    showPoins.Add(position);
                }

                // last checked position.
                if (i > 1)
                {
                    lastPos = pointsTrajctory[i];
                }
            }

            // Show the line render.
            lineRenderer.positionCount = showPoins.Count;
            lineRenderer.SetPositions(showPoins.ToArray());

            // Show the hited point.
            _ShowTheHitedPoint(flagHit, position, (position - lastPos).normalized, enableTime);

            //Debug.Log("show point count: " + showPoins.Count);
            return enableTime;
        }

        void _ShowTheHitedPoint(bool _flagHit, Vector3 pos, Vector3 direction, float enableTime)
        {
            // Firstly, set the jet point.
            if (jetPoint == null)
            {
                // Get the quaternion from direction.
                Quaternion quaternion = Quaternion.identity;
                if (direction != Vector3.zero)
                {
                    quaternion = Quaternion.LookRotation(direction);
                }

                // Instance a jet point.
                GameObject objHiter = Instantiate(objectHiter, pos, quaternion, transform);
                jetPoint = new JetPoint(true, enableTime, jettingForce, forceInterval, jettingSize, forceType, objHiter.transform);

                // Setting jet prepoping
                if (jPrepoping != null)
                {
                    jPrepoping.jPoints.listPoint.Add(jetPoint);
                    jPrepoping.PrePoping();
                    //Debug.Log("pre poping");
                }
            }
            else
            {
                // Refresh the jet point.
                jetPoint.Refresh(_flagHit, enableTime, pos);
            }
        }

        IEnumerator StartTimeProjectie()
        {
            yield return new WaitForSeconds(timeWaitProjectile);
            projectSystem.gameObject.SetActive(true);
        }
    }
}

