
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: Faceing a Transform.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTransform : MonoBehaviour {

	public Transform target;

	// Use this for initialization
	void Start () {
		if(target == null){
			_CheckSetting("not seted target");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(target != null){
			transform.LookAt(target);
		}
	}

	void _CheckSetting(string warning){
		Debug.Log("FaceTransform.cs - " + warning);
	}
}
