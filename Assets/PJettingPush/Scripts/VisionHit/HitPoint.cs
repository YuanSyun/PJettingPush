
/*
	Date: Agu, 13, 2018
	Coder: YuanSyun Ye(yuansyuntw@gmail.com)
	Purpose: 針對最近的Turk顯示Halo。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.UI;


namespace edu.nctu.nllab.PJettingPush
{
    public class HitPoint : MonoBehaviour
    {

        #region public variable


        public GameObject showOrHideObject;

        [Header("Scale a point")]
        public GameObject scaleObject;
        public float angleThreshold = 20f;

        [Header("Check the ray to target.")]
        public LayerMask checkLayer;
        public GameObject showPointObject;
        public Material rayToTargetMat;
        public Material rayHaventObstacleMat;

        [Header("Timing")]
        public Canvas lookatCanvas;
        public Text timeText;

        public JetPoint GetSignal
        {
            get
            {
                return jpSignal;
            }
        }

        [HideInInspector]
        public bool flagHited; // Controll by JetHitCasting.cs

        [Space(3)]
        public bool debugFlag = false;

        #endregion






        #region private variable

        private JetPoint jpSignal;
        private Turk nearestTurk;
        private TurksManager tm;
        private Renderer render;
        private float hitDistanceRate = 0.96f;

        #endregion





        #region api

        public void SetSignal(JetPoint jp)
        {
            //Debug.Log("set signal: " + jp);
            if (jp != null)
            {
                this.jpSignal = jp;

                // Show the hit point.
                if (showOrHideObject != null)
                {
                    showOrHideObject.SetActive(true);
                }
            }
            else
            {
                // Hide the hit point
                if (showOrHideObject != null)
                {
                    showOrHideObject.SetActive(false);
                }
            }
        }

        public void RemoveSignal()
        {
            // Remove the jet points line render.
            if (nearestTurk != null)
            {
                nearestTurk.ReleaseHitPoint(this);
            }
        }

        public JetPoint GetJetPoint()
        {
            return jpSignal;
        }

        #endregion





        #region lifecycle

        // Use this for initialization
        void Start()
        {
            if (scaleObject == null)
            {
                _CheckSetting("not seted the halo obejct.", true);
            }

            tm = TurksManager.instance;
            if (tm == null)
            {
                _CheckSetting("not found the turk manager.", true);
            }

            if (showPointObject != null)
            {
                render = showPointObject.GetComponent<Renderer>();
            }

            if (showOrHideObject != null)
            {
                showOrHideObject.SetActive(false);
            }

            if (debugFlag)
            {
                jpSignal = new JetPoint();
                jpSignal.trans = transform;
                jpSignal.size = 0.1f;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if ((jpSignal != null) && (jpSignal.trans != null))
            {

                //Update the position of the hit point.
                transform.DOMove(jpSignal.trans.position, 0.5f);

                // Update the time text.
                if (timeText != null)
                {
                    float t = jpSignal.enableTime - Time.time;
                    timeText.text = t.ToString("0");
                }

                // Scale the point.
                Turk newNearestTurk = tm.GetNearestTurk(transform.position);
                if (newNearestTurk != null)
                {

                    // Register the halo hiter.
                    if (nearestTurk != newNearestTurk)
                    {
                        // Release old ui hiter.
                        if (nearestTurk != null)
                        {
                            nearestTurk.ReleaseHitPoint(this);
                        }

                        // Register new ui hiter.
                        nearestTurk = newNearestTurk;
                        nearestTurk.RegisterHitPoint(this);
                    }

                    // Update the lookat canvas.
                    if (lookatCanvas != null)
                    {
                        lookatCanvas.transform.DOLookAt(nearestTurk.transform.position, 0.5f);
                    }

                    Vector3 head_forward = nearestTurk.camera.transform.forward;
                    Vector3 head_position = nearestTurk.camera.transform.position;
                    Vector3 target_position = transform.position;
                    Vector3 target_direction = target_position - head_position;

                    // Show the halo
                    float angle = Vector3.Angle(target_direction, head_forward);
                    if (angle > angleThreshold)
                    {
                        float sphere_radius = Vector3.Distance(target_position, head_position);

                        // Adjust the halo size.
                        Vector3 head_forward_on_surface = head_position + head_forward * sphere_radius;
                        Vector3 normal = Vector3.Cross(head_forward, target_direction);
                        GameObject head_on_surface_obj = new GameObject();
                        head_on_surface_obj.transform.position = head_forward_on_surface;
                        head_on_surface_obj.transform.RotateAround(head_position, normal, angleThreshold);

                        // The radius need multiple two.
                        float halo_Sphere_radius = Vector3.Distance(target_position, head_on_surface_obj.transform.position) * 2;

                        // Keep the jet size.
                        float minScale = jpSignal.GetShowScale();
                        if(halo_Sphere_radius< minScale){
                            halo_Sphere_radius = minScale;
                        }

                        // Set HaloSphere
                        scaleObject.transform.DOMove(target_position, 0.5f);
                        scaleObject.transform.DOScale(new Vector3(halo_Sphere_radius, halo_Sphere_radius, halo_Sphere_radius), 0.5f);

                        // Check raying a target or raying a obstacle.
                        //Debug.DrawRay(head_position, target_direction * hitDistanceRate, Color.yellow);
                        RaycastHit hit;
                        if (Physics.Raycast(head_position, target_direction, out hit, sphere_radius * hitDistanceRate, checkLayer))
                        {
                            if (rayHaventObstacleMat != null)
                            {
                                render.material = rayHaventObstacleMat;
                            }
                        }
                        else
                        {
                            if (rayToTargetMat != null)
                            {
                                render.material = rayToTargetMat;
                            }
                        }

                        // Show the obejct.
                        if (showOrHideObject != null)
                        {
                            showOrHideObject.SetActive(true);
                        }

                        Destroy(head_on_surface_obj);
                    }
                }//end scale the hit point.
            }
        }

        #endregion






        void _CheckSetting(string warning, bool destory)
        {
            Debug.Log("HaloHiter.cs - " + warning);
            if (destory)
            {
                Destroy(this);
            }
        }
    }
}
