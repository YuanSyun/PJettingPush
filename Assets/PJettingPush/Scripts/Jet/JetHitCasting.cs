
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 10, 2018
	Purpose: 
		偵測JetButtonDetector.cs trigger flag得知目前噴嘴按鈕是否有被按下。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace edu.nctu.nllab.PJettingPush
{
    public class JetHitCasting : MonoBehaviour
    {

        #region public variable

        public JetButtonDetector jbDetector;
        public Camera jetCamera;
        public ParticleSystem particle;
        public LayerMask layerMask;


        #endregion





        #region private variable

        private bool flagPressButton = false;
        private bool flagHitDetector = false;
        private float timeChecked = 0.05f; // 50 ms
        private float hitDistance = 30.0f; // 30 meter
        private Transform transHited;
        private Transform transLastHited;
        private HitPoint lastHitPoint;

        #endregion





        #region lifecycle

        // Use this for initialization
        void Start()
        {
            if (jbDetector == null)
            {
                _CheckSetting("jet button detector not seted", true);
            }
            if (particle == null)
            {
                _CheckSetting("the particle not seted", false);
            }

            StartCoroutine(_RefreshButtonState());
            StartCoroutine(_Casting());
        }

        // Update is called once per frame
        void Update()
        {
            // Register the hit flag, if the casting hit the detector and pressing the jeting button. 
            if (flagHitDetector && flagPressButton && (transLastHited != null))
            {
                lastHitPoint = transLastHited.GetComponent<HitPoint>();
                if (lastHitPoint != null)
                {
                    lastHitPoint.flagHited = true;
                }
            }
        }

        #endregion





        void _CheckSetting(string warning, bool destory)
        {
            Debug.Log("JetParticles.cs - " + warning);
            if (destory)
            {
                Destroy(this);
            }
        }

        IEnumerator _Casting()
        {

            Ray ray;
            RaycastHit hiter;
            Vector3 center = new Vector3(jetCamera.pixelWidth / 2, jetCamera.pixelHeight / 2, 0.0f);

            while (true)
            {
                ray = jetCamera.ScreenPointToRay(center);
                Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
                if (Physics.Raycast(ray, out hiter, hitDistance, layerMask))
                {
                    //Debug.Log("hit: " + hiter.transform.name);
                    transHited = hiter.transform;

                    if (transLastHited != transHited)
                    {
                        transLastHited = transHited;

                        // Check the hit obejct is the detector.
                        lastHitPoint = transLastHited.GetComponent<HitPoint>();
                        if (lastHitPoint != null)
                        {
                            flagHitDetector = true;
                        }
                    }
                }
                else
                {
                    transHited = null;

                    // Unregister the hit flag.
                    if (lastHitPoint != null)
                    {
                        lastHitPoint.flagHited = false;
                        lastHitPoint = null;
                    }
                }

                yield return new WaitForSeconds(timeChecked);
            }
        }

        IEnumerator _RefreshButtonState()
        {
            while (true)
            {
                if (flagPressButton != jbDetector.flagTrigger)
                {
                    flagPressButton = jbDetector.flagTrigger;
                    if (flagPressButton)
                    {
                        particle.Play();
                    }
                    else
                    {
                        particle.Stop();
                    }
                }

                yield return new WaitForSeconds(timeChecked);
            }
        }
    }
}

