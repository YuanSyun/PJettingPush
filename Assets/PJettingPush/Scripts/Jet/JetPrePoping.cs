
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: Pre-poping a JetPoints to JettingController.cs
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace edu.nctu.nllab.PJettingPush
{
    public class JetPrePoping
    {
		#region public variable
        public JetPointsRenderController jController;
		public JetPoints jPoints = new JetPoints();
		#endregion

		#region public api
		public void PrePoping(){
			JetPoints ps = jPoints.GetClone();
			jController.AddJetPoints(ps);
		}
		#endregion
    }

}

