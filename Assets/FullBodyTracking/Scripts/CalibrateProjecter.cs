using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace edu.NCTU.NILab.PJettingPush
{

	public class CalibrateProjecter : MonoBehaviour
	{	
		#region public
		public Camera projector;
		public TrackerManager trackerManager;// Get tracker transform
		public Transform offsetTracker;
		public GraphicRaycaster myRaycaster;
		public EventSystem myEventSystem;
		public Transform transTargetUI;
		public int numberHitUI = 30;
		#endregion

		#region private
		private Transform transTracker;
		private PointerEventData myPointerEventData;
		private List<Vector3> rotateHitList = new List<Vector3>();
		#endregion



		// Use this for initialization
		void Start ()
		{
			if (projector == null) {
				_CheckSetting("doesn't set the componment 'projector'");
			}
			if (offsetTracker == null) {
				_CheckSetting("doesn't set the componment 'offsetProjecter'");
			}
			if (myRaycaster == null) {
				_CheckSetting("doesn't set the componment 'myRaycaster'");
			}
			if(myEventSystem == null){
				_CheckSetting("doesn't set the componment 'myEventSystem'");
			}
			if (transTargetUI == null) {
				_CheckSetting("doesn't set the componment 'transTargetUI'");
			}
			if (trackerManager == null) {
				_CheckSetting("doesn't set the componment 'trackerMan'");
			}
		}
	


		// Update is called once per frame
		void Update ()
		{
			if (transTracker == null) {
				transTracker = trackerManager.GetTrackerTransform;
			} else {
				if (rotateHitList.Count < numberHitUI) {
					myPointerEventData = new PointerEventData (myEventSystem);
					myPointerEventData.position = new Vector2 (projector.pixelWidth / 2, projector.pixelHeight / 2);

					List<RaycastResult> results = new List<RaycastResult> ();
					myRaycaster.Raycast (myPointerEventData, results);

					foreach (RaycastResult result in results) {
						//Debug.Log ("Hit " + result.gameObject.name);

						if (result.gameObject.name == transTargetUI.name) {
							// Save tracker rotation
							rotateHitList.Add (transTracker.localEulerAngles);
							Debug.Log ("Hit count: " + rotateHitList.Count);
						}
					}//end foreach
				} else {
					_Calibrate (transTracker, offsetTracker);
					Debug.Log ("Calibrate finish");
					enabled = false;
				}
			}
		}//end update



		void _CheckSetting(string error){
			Debug.Log ("CalibrateProjecter.cs - " + error);
			Destroy (this);
		}



		void _Calibrate(Transform removed, Transform target){

			Vector3 finalRotate = Vector3.zero;
			foreach (Vector3 i in rotateHitList) {
				finalRotate.x += i.x;
				finalRotate.y += i.y;
				finalRotate.z += i.z;
			}
			finalRotate.x /= rotateHitList.Count;
			finalRotate.y /= rotateHitList.Count;
			finalRotate.z /= rotateHitList.Count;

			finalRotate.x = 360 - finalRotate.x;
			finalRotate.y = 360 - finalRotate.y;
			finalRotate.z = 360 - finalRotate.z;

			Debug.Log ("final roate: " + finalRotate);

			removed.parent = target;
			target.localEulerAngles = finalRotate;
		}
	}
}
