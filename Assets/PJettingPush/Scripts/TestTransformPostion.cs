using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransformPostion : MonoBehaviour {

	public Transform transBase;

	public Transform transTarget;

	public Transform transFollow;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transFollow.position = transBase.TransformPoint(transBase.InverseTransformPoint(transTarget.position));
	}
}
