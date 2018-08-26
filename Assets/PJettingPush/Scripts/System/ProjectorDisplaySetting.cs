
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 18, 2018
	Purpose: 
		Controlle the display size of the projector.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace edu.nctu.nllab.PJettingPush
{
    public class ProjectorDisplaySetting : MonoBehaviour
    {

        public int displayIndex;

        // Use this for initialization
        void Start()
        {
            if (Display.displays.Length > 1)
            {
                Display.displays[displayIndex].Activate();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

