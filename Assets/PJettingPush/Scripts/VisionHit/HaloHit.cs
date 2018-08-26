
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 15, 2018
	Purpose: 
		在指定的Camera下顯示噴氣方向，利用Canvas image顯示圓形圖案。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;

namespace edu.nctu.nllab.PJettingPush
{
    public class HaloHit : MonoBehaviour
    {

        public class Haloer
        {
            public HitPoint hitPoint;
            public RectTransform uiTransform;

            public Haloer(HitPoint _hiter, RectTransform _ui)
            {
                this.hitPoint = _hiter;
                this.uiTransform = _ui;
            }
        }





        [Header("Halo hitting")]
        public Canvas showCanvas;
        public GameObject showHaloHitObject;
        public float rotateDuration = 0.5f;
        public float acceptableAngle = 10f;
        public float offsetAngle = 18;

        [Header("Aiming")]
        public Camera projectorCamera;
        public LayerMask aimingLayer;
        public float aimingDistanceOffset = 0.2f; // 20cm
        public Canvas aimingCanvas;
        public Image aimingImage;
        public Color inJetedDistanceColor;
        public Color OutOfJetedDistanceColor;

        [Header("Debuging")]
        public Text debugLog;







        private List<Haloer> listHaloer = new List<Haloer>();
        private HitPoint lastHitPoint;
        private float aimingDistance = 50f;






        public void AddTarget(HitPoint _hiter)
        {
            //Debug.LogFormat("Add target hiter: {0}", _hiter);
            RectTransform ui = null;
            if (showHaloHitObject != null)
            {
                ui = Instantiate(showHaloHitObject, showCanvas.transform).GetComponent<RectTransform>();
                ui.gameObject.SetActive(false);
            }

            listHaloer.Add(new Haloer(_hiter, ui));
        }

        public void RemoveTarget(HitPoint hiter)
        {

            // Cancel the aiming color hiter.
            if ((lastHitPoint == hiter) && (aimingImage != null))
            {
                aimingImage.DOColor(OutOfJetedDistanceColor, 0.1f);
            }

            for (int i = 0; i < listHaloer.Count; i++)
            {
                if (listHaloer[i].hitPoint == hiter)
                {
                    if (listHaloer[i].uiTransform != null)
                    {
                        Destroy(listHaloer[i].uiTransform.gameObject);
                    }
                    listHaloer.RemoveAt(i);
                    return;
                }
            }
        }






        // Use this for initialization
        void Start()
        {
            if (showCanvas == null)
            {
                _CheckSetting("not seted the show canvas", true);
            }

            if (showHaloHitObject == null)
            {
                _CheckSetting("not seted the halo hit object", false);
            }
        }

        // Update is called once per frame
        void Update()
        {

            // Update aiming Canvas position.
            if((aimingCanvas != null) && (projectorCamera != null)){

                Vector3 center = new Vector3(projectorCamera.pixelWidth/2, projectorCamera.pixelHeight/2, 0.0f);
                Ray ray = projectorCamera.ScreenPointToRay(center);
                RaycastHit hit;

                // Moveing the position.
                if(Physics.Raycast(ray, out hit, aimingDistance, aimingLayer)){
                    aimingCanvas.transform.position = hit.point;
                }

                // Looking the projector.
                aimingCanvas.transform.DOLookAt(projectorCamera.transform.position, 0.5f);
            }

            if (listHaloer.Count > 0)
            {
                //Debug.LogFormat("list haloer count: {0}", listHaloer.Count);
                float zRotate = 0.0f;
                HitPoint newHitPoint = null;
                bool showTheAimingImage = false;

                // Refresh the halo hit.
                for (int i = 0; i < listHaloer.Count; i++)
                {
                    zRotate = GetDirection(transform, listHaloer[i].hitPoint.transform, acceptableAngle, offsetAngle);

                    // In aiming.
                    if (zRotate == -1)
                    {
                        newHitPoint = listHaloer[i].hitPoint;
                        showTheAimingImage = true;
                        if (listHaloer[i].uiTransform != null)
                        {
                            listHaloer[i].uiTransform.gameObject.SetActive(false);
                        }

                    }
                    // Out of range.
                    else
                    {
                        if (listHaloer[i].uiTransform != null)
                        {
                            listHaloer[i].uiTransform.DOLocalRotate(new Vector3(0, 0, zRotate), rotateDuration);
                            listHaloer[i].uiTransform.gameObject.SetActive(true);
                        }
                    }

                    //Debug.LogFormat("zRotate: {0}, transform: {1}, listHaloer{2}: {3}", zRotate, transform, i, listHaloer[i].hitPoint);
                }

                // Register the hit flag.
                if (newHitPoint != null)
                {
                    // Release the hit flag.
                    if ((lastHitPoint != null) && (lastHitPoint != newHitPoint))
                    {
                        lastHitPoint.flagHited = false;
                    }
                    lastHitPoint = newHitPoint;

                    // Registe the hit flag.
                    lastHitPoint.flagHited = true;
                }

                // Change the aiming image material
                if (lastHitPoint != null)
                {
                    JetPoint target = lastHitPoint.GetJetPoint();
                    if ((lastHitPoint.GetJetPoint() != null) && (target.trans != null))
                    {

                        float distance = Vector3.Distance(transform.transform.position, target.trans.position);
                        //Debug.Log("distance: " + distance);

                        // The projector is in the jeted distance.
                        if ((distance < target.GetJetDistance() + aimingDistanceOffset) && (distance > target.GetJetDistance() - aimingDistanceOffset))
                        {
                            if (aimingImage != null)
                            {
                                aimingImage.DOColor(inJetedDistanceColor, 0.1f);
                                //Debug.Log("in jeted distance.");
                            }
                        }
                        else
                        {
                            // The projector isnot in the jeted distance.
                            if (aimingImage != null)
                            {
                                aimingImage.DOColor(OutOfJetedDistanceColor, 0.1f);
                                //Debug.Log("not in jeted distance.");
                            }
                        }

                        string log = "now distance: " + distance.ToString("0.00") + "\ntarget distance: " + target.GetJetDistance().ToString("0.00");
                        if (debugLog != null)
                        {
                            debugLog.text = log;
                        }
                    }
                }
                else
                {

                }//end changeing the aiming image material.
            }//end if
        }






        float GetDirection(Transform original, Transform _hiter, float _acceptableAngle, float _offsetAngle)
        {

            Vector3 forward = original.forward;
            Vector3 hiterDirection = (_hiter.position - original.position);

            //Debug.DrawRay(self.position, forward, Color.blue);
            //Debug.DrawRay(original.position, hiterDirection, Color.yellow);
            //Debug.DrawRay(original.position, original.up, Color.red);

            float UDAngle = Vector3.SignedAngle(forward, hiterDirection, original.up);
            float LRAngle = Vector3.SignedAngle(forward, hiterDirection, original.right);

            float angle = -1f;

            // Checking the acceptable angle.
            string log = "";
            if ((UDAngle < -_acceptableAngle) || (UDAngle > _acceptableAngle) ||
                (LRAngle < -_acceptableAngle) || (LRAngle > _acceptableAngle))
            {
                // Converting to the Polar coordinate system
                float x = LRAngle;
                float y = UDAngle;

                angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
                angle += _offsetAngle;

                //log = "UD:" + UDAngle.ToString("0") + ", LR: " + LRAngle.ToString("0") + "\nX:" + x.ToString("0") + ",Y:" + y.ToString("0") + "\nAngle: " + angle;
            }

            if (debugLog != null)
            {
                debugLog.text = log;
            }

            return angle;
        }

        void _CheckSetting(string _warning, bool _destory)
        {
            Debug.Log(_warning + " - HaloHit.cs");
            if (_destory)
            {
                Destroy(this);
            }
        }
    }
}
