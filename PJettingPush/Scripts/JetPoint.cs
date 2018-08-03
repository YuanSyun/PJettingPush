
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: Define the point of jetting.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
	[System.Serializable]
	public class JetPoint
	{
		public float enableTime;
		public float force;
		public float interval;
		public Transform trans;
	}
}
