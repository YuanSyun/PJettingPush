
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 4, 2018
	Purpose: Controlling BM_Weapon_System_C.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
    public class AutoFireController : MonoBehaviour
    {
        #region public variable

        public BM_Weapon_System_C weaponSystem;
        public float interval = 0.5f;
        public bool enableOnStart = true;
        public Vector3 posRandom;
        public Transform transMoved;

        #endregion





        #region private variable

        private Vector3 posOriginal;
        private bool flagFire = false;

        #endregion





        #region api

        public void StartFire(){
            if(!flagFire){
                StartCoroutine(_Timer());
            }
        }

        #endregion





        #region lifecycle

        // Use this for initialization
        void Start()
        {
            if (weaponSystem == null)
            {
                _CheckSetting("not seted 'weapon system'");
            }
            posOriginal = transform.position;

            if(enableOnStart){
                StartFire();
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        #endregion





        IEnumerator _Timer(){

            flagFire = true;

            while(flagFire){

                _RandomPosition();
                _Fire();

                yield return new WaitForSeconds(interval);
            }
        }

        void _CheckSetting(string warning)
        {
            Debug.Log("AutoFireController.cs - " + warning);
            Destroy(this);
        }

        void _Fire()
        {
            weaponSystem.StartCoroutine("Fire_Primary");
        }

        void _RandomPosition()
        {
            Vector3 posNew = new Vector3();
            posNew = posOriginal;
            posNew.x += Random.Range(-posRandom.x, posRandom.x);
            posNew.y += Random.Range(-posRandom.y, posRandom.y);
            posNew.z += Random.Range(-posRandom.z, posRandom.z);
            transMoved.position = posNew;
        }
    }
}

