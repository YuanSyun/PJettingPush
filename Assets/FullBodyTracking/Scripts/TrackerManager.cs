/*
	Date: Jul, 31, 2018
	Coder: YuanSyun Ye(yuansyuntw@gmail.com)
	Purpose: Assign a HTC Vivie tracker to a camera. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.NCTU.NILab.PJettingPush
{

	public class TrackerManager : MonoBehaviour
	{


		#region Public
		public List<GameObject> listTrackers;
		public List<Transform> targetObjects;
		public Valve.VR.ETrackedDeviceClass classTracker = Valve.VR.ETrackedDeviceClass.GenericTracker;
		public Transform GetTrackerTransform{
			get{
				return _trackerTransform;
			}
		}
		#endregion



		#region private

		private Transform _trackerTransform;
		private Dictionary<int, Transform> dicTracker = new Dictionary<int, Transform>();
		private int assignCount = 0;

		#endregion



		// Use this for initialization
		void Start ()
		{
			// Sort tracker objects
			foreach (var item in listTrackers) {
				dicTracker [(int)item.GetComponent<SteamVR_TrackedObject> ().index] = item.transform;
			}

			for(int i = (int)SteamVR_TrackedObject.EIndex.Device1; i < (int)SteamVR_TrackedObject.EIndex.Device15; i++){
				var device = SteamVR_Controller.Input (i);
				if (device.hasTracking && device.connected && device.valid) {
					var deviceClass = Valve.VR.OpenVR.System.GetTrackedDeviceClass ((uint)i);
					if (deviceClass == classTracker) {

						if(assignCount <= targetObjects.Count){
							targetObjects[assignCount].parent = dicTracker [i];
							assignCount++;
							Debug.Log("assign count:" + assignCount);
						}

						_trackerTransform = dicTracker [i];
					}
				}
			}
		}
	}

}