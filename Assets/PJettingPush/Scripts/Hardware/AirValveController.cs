
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 10, 2018
	Purpose: 
		接收JetedDetector.cs Signal，接著利用Serial Port控制air valve。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace edu.nctu.nllab.PJettingPush
{
    public class AirValveController : MonoBehaviour
    {

        #region public variable

        static public AirValveController instance;
        public ParticleSystem particle;
        public Material matHit;
        public Material matUnhit;

        #endregion





        #region private variable

        private List<HitPoint> bufferJetSignal = new List<HitPoint>();
        private HitPoint loadingSignal;

        #endregion





        #region api

        public void AddJetSignal(HitPoint signal)
        {
            bufferJetSignal.Add(signal);
        }

        public void RemoveJetSignal(HitPoint signal)
        {
            if (loadingSignal == signal)
            {
                loadingSignal = null;
            }
            bufferJetSignal.Remove(signal);
            Debug.Log("Remove the signal:" + signal);
        }

        #endregion





        #region lifecycle

        // Use this for initialization
        void Awake()
        {
            if (AirValveController.instance == null)
            {
                AirValveController.instance = this;
            }
            else
            {
                Destroy(this);
            }

            if (particle == null)
            {
                _CheckSetting("the particle not seted", false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Polling a new signal.
            if ((loadingSignal == null) && (bufferJetSignal.Count > 0))
            {
                loadingSignal = _LoadSignal();
                if ((loadingSignal != null) && (loadingSignal.GetSignal != null))
                {
                    JetPoint signal = loadingSignal.GetSignal;
                    _AdjectValve(signal.type, signal.force, signal.interval);
                }
            }

            if ((loadingSignal != null) && loadingSignal.flagHited)
            {
                // Change particle material.
                if ((particle != null) && (matHit != null))
                {
                    particle.GetComponent<ParticleSystemRenderer>().material = matHit;
                }
            }
            else
            {
                // Change the particle matierial.
                if ((particle != null) && (matUnhit != null))
                {
                    particle.GetComponent<ParticleSystemRenderer>().material = matUnhit;
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

        HitPoint _LoadSignal()
        {
            HitPoint signal = null;
            foreach (HitPoint s in bufferJetSignal)
            {
                signal = s;
                bufferJetSignal.Remove(s);
                break;
            }
            return signal;
        }

        void _AdjectValve(JetPoint.Type type, float force, float interval)
        {
            Debug.Log("adject valve type: " + type + ", force: " + force + ", interval: " + interval);
        }
    }
}

