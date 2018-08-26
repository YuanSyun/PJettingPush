
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 10, 2018
	Purpose: 
		場景控制
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
    public class SceneManager : MonoBehaviour
    {

		#region public variable

		public UnityEngine.SceneManagement.LoadSceneMode loadMode = UnityEngine.SceneManagement.LoadSceneMode.Additive;
        public List<string> listSceneName = new List<string>();

		#endregion






		#region lifecycle

        // Use this for initialization
        void Start()
        {
			StartCoroutine(_LoadAsyncSceneList());
        }

        // Update is called once per frame
        void Update()
        {

        }

		#endregion






        IEnumerator _LoadAsyncSceneList()
        {
            if (listSceneName.Count > 0)
            {
                AsyncOperation ayncLoad;
                foreach (string name in listSceneName)
                {
					//Debug.Log("Load scene: " + name);
                    ayncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, loadMode);

                    while (!ayncLoad.isDone)
                    {
                        yield return null;
                    }
                }
				//Debug.Log("Finish loading");
            }
        }
    }
}

