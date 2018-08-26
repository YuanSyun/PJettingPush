using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLinecasting : MonoBehaviour {

	// Use this for initialization

	public Transform point1;
	public Transform point2;
	public LayerMask colliderLayer;

	private RaycastHit hit;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Debug.DrawLine(point1.position, point2.position, Color.red);
		if(Physics.Linecast(point1.position, point2.position, out hit, colliderLayer)){
			Debug.Log(hit.transform.name);
		}
	}
}
