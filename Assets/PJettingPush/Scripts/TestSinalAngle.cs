using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSinalAngle : MonoBehaviour {

	public Transform original;
	public Transform target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawRay(original.position, original.forward, Color.yellow);
		Debug.DrawRay(original.position, (target.position - original.position), Color.red);
		Debug.DrawRay(original.position, original.up, Color.blue);
		Debug.LogFormat("angle {0}", Vector3.SignedAngle(original.forward, (target.position - original.position), original.up));
	}
}
