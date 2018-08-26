
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 7, 2018
	Purpose: 
        此程式為控制劍道人物的動作。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
    public class KatanaController : MonoBehaviour
    {

        #region public variable

        public bool enableOnStart = false;
        public Animator animator;
        public float intervalAction = 3.0f;
        public GameObject objectDetecter;
        public Transform transDetecterBase;
        public List<string> listAction;
        #endregion




        #region private variable

        private bool flagEnable = false;

        #endregion





        #region lifecycle
        // Use this for initialization
        void Start()
        {

            if(transDetecterBase == null){
                _CheckSetting("transDetecterBase not set", false);
            }

            if (animator == null)
            {
                _CheckSetting("animator not setting.", true);
            }

            if (listAction.Count == 0)
            {
                _CheckSetting("list animations name  not setting.", true);
            }

            if (enableOnStart)
            {
                _Enable();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {

        }
        #endregion




        void _CheckSetting(string warning, bool destory)
        {
            Debug.Log("KatanaController.cs - " + warning);
            if (destory)
            {
                Destroy(this);
            }
        }

        void _Enable()
        {
            flagEnable = true;
            StartCoroutine(_Action());
        }

        IEnumerator _Action()
        {

            string action = "";

            AttackAreaDetecter detector;

			if(intervalAction <= 0.0f){
				intervalAction = 0.1f;
			}

            while (flagEnable)
            {
                action = listAction[Random.Range(0, listAction.Count)];

                // Detecter
                if (objectDetecter != null)
                {
                    detector = Instantiate(objectDetecter, transform).GetComponent<AttackAreaDetecter>();
                    detector.ShowDetectAttackArea(action, transDetecterBase, intervalAction);
                }

                yield return new WaitForSeconds(intervalAction);

				// Wait to load animation.
                animator.Play(action);
				while(!animator.GetCurrentAnimatorStateInfo(0).IsName(action)){
					yield return new WaitForSeconds(AttackAreaRecorder.timeWaitLoadAnimation);
				}

                // Wait animation.
                float timeWait = animator.GetCurrentAnimatorStateInfo(0).length;
                //Debug.Log("wait time: " + timeWait);
                yield return new WaitForSeconds(timeWait);

                //Debug.Log("animator end: " + Time.time);
            }
        }
    }
}

