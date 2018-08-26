using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour {

	public Transform targetTransform;

	// Use this for initialization
	void Start () {

		if (targetTransform == null) {
			Debug.Log ("FollowPosition.cs - dosen't set target transform.");
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (targetTransform != null) {
			transform.position = targetTransform.position;
		}
	}
}
