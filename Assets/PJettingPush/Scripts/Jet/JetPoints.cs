
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: Define the list of the jeted point.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
	[System.Serializable]
    public class JetPoints
    {
		[SerializeField]
        public List<JetPoint> listPoint = new List<JetPoint>();

		public float timeMax; // The last enabled time in list jet point. 

		public JetPoints GetClone(){
			JetPoints clone = new JetPoints();
			clone.listPoint = new List<JetPoint>();
			foreach(JetPoint i in listPoint){
				clone.listPoint.Add(i);
			}
			return clone;
		}

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

