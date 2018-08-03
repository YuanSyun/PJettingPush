
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: Define the point list of jetting.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
    public class JetPoints : MonoBehaviour
    {
		[SerializeField]
        public List<JetPoint> listPoint;

		[HideInInspector]
		public float popingTime;

		public List<JetPoint> GetSortListPoint{
			get{
				listPoint.Sort(CompareJetPoint);
				return listPoint;
			}
		}

		private int CompareJetPoint(JetPoint x, JetPoint y){
			return x.enableTime.CompareTo(y.enableTime);
		}
    }
}

