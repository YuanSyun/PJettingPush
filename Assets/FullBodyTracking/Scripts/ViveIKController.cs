/*
	Date: Jul, 30, 2018
	Coder: YuanSyun Ye(yuansyuntw@gmail.com)
	Purpose: This assign vive's tricker to the postions of the model, and ThreePointIK will help us to rotate model's bone angle.
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.Events;

namespace edu.NCTU.NILab.PJettingPush
{
	
	public class ViveIKController : MonoBehaviour
	{

		enum Stage
		{
			Stage0,// 自動調整身高
			Stage1,
			Stage2,
			Stage3
		}

		#region public

		public Text logText;
		[Header("Assign To Tracker")]
		public GameObject bodyOffsetObject;
		public GameObject leftFootOffsetObject;
		public GameObject rightFootOffsetObject;
		public GameObject leftHandOffsetObject;
		public GameObject rightHandOffsetObject;
		public GameObject headOffsetObject;
		public GameObject projectOffsetObject;

		[Header("Adjust The Model Height")]
		public Transform ModelEyeTransform;

		[Header("Tracker Objects")]
		public List<GameObject> listTrackerObject;

		[Header("Finished the adjusted height event.")]
		public UnityEvent finishedAdjustedEvents;

		[Header("Calibrated the event")]
		public UnityEvent finishedCalibrateEvents;

		#endregion

		#region private

		Dictionary<int, Transform> deviceMarkerDict = new Dictionary<int, Transform> ();
		Stage stage = Stage.Stage0;
		Queue<object> logQueue = new Queue<object> ();
		// logs in logQueue can be seen in VR
		int maxLogCount = 8;
		float initHeight = 1.7f;
		IKDemoModelState initModelState;

		Dictionary<TrackerRole, int> trackers = new Dictionary<TrackerRole, int> ();
		List<OffsetTracking> offsetTrackedList = new List<OffsetTracking> ();
		Transform leftHandTarget = null;
		Transform rightHandTarget = null;
		AnimationBlendTest animationBlender;
		List<ThreePointIK> ikList = new List<ThreePointIK> ();
		// to fully control the execution order of the IK solvers

		private Transform transProjectTracker = null;

		#endregion

		//-------------------------------------------------------------------------------------------------------------
		void Start ()
		{
			foreach (var item in listTrackerObject) {
				deviceMarkerDict [(int)item.GetComponent<SteamVR_TrackedObject> ().index] = item.transform;
			}
			//InitWithCustomHeight();

			animationBlender = GetComponent<AnimationBlendTest> ();
			RecordInitModelState ();
		}

		//-------------------------------------------------------------------------------------------------------------
		void Update ()
		{

			UpdateIK ();

			//檢查是否有按下按鈕
			bool gripClicked = false;
			for (int i = (int)SteamVR_TrackedObject.EIndex.Device1; i < (int)SteamVR_TrackedObject.EIndex.Device15; i++) {
				var device = SteamVR_Controller.Input (i);
				gripClicked |= device.GetPressUp (SteamVR_Controller.ButtonMask.Grip);
			}

			//自動調整身高
			if (stage == Stage.Stage0) {
				AutoAdjustHeight ();
			}

			//按下grip
			if (stage == Stage.Stage0 && (Input.GetKeyUp (KeyCode.Alpha1) || Input.GetMouseButtonUp (2) || gripClicked)) {
				gripClicked = false;
				if (AssignTrackers ()) {
					stage = Stage.Stage2;
					finishedAdjustedEvents.Invoke();
					MyLog ("Entering stage2");
				} else {
					MyLogError ("Not enough tracked devices found");
				}
			}

			if (stage == Stage.Stage1) {
				var pos = Camera.main.transform.position;
				pos.y = 0;
				pos.z -= 0.2f;
				transform.position = pos;
			}

			if (stage == Stage.Stage1 && (Input.GetKeyUp (KeyCode.Alpha1) || Input.GetMouseButtonUp (2) || gripClicked)) {
				gripClicked = false;
				stage = Stage.Stage2;
				MyLog ("Entering stage2");
			}

			//設定好定位後按下，再次按下grip( 修正誤差 )
			if (stage == Stage.Stage2 && (Input.GetKeyUp (KeyCode.Alpha2) || gripClicked)) {
				gripClicked = false;

				// Create a tracker offset list.
				StartOffsetTracking ();

				// Enable IK Script
				StartIK ();

				stage = Stage.Stage3;
				finishedCalibrateEvents.Invoke ();
				MyLog ("Entering stage3");
			}

			if (stage == Stage.Stage3) {
				UpdateOffsetTracking ();
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		void RecordInitModelState ()
		{
			//保存模型初始位置的位置
			initModelState = new IKDemoModelState ();
			initModelState.eyePos = ModelEyeTransform.position;
			initModelState.ankleMarkerLeftPos = leftFootOffsetObject.transform.position;
			initModelState.ankleMarkerRightPos = rightFootOffsetObject.transform.position;
			initModelState.markerHeadPos = headOffsetObject.transform.position;
			initModelState.modelScale = transform.localScale;
		}

		//-------------------------------------------------------------------------------------------------------------
		void AutoAdjustHeight ()
		{

			float actualEyeHeight = Camera.main.transform.position.y;

			// 限制高度
			actualEyeHeight = Mathf.Clamp (actualEyeHeight, 0.7f, 2.5f);

			float eyeHeightToBodyHeadRatio = initModelState.eyePos.y / initHeight;
			float estimatedHeight = actualEyeHeight / eyeHeightToBodyHeadRatio;

			//Debug.LogFormat ("actualEyeHeight:{0}, estimatedHeight:{1}", actualEyeHeight, estimatedHeight);

			AdjustToHeight (estimatedHeight);
		}

		//-------------------------------------------------------------------------------------------------------------
		void InitWithCustomHeight ()
		{
			float customHeight;
			if (DumbConfigFile.ReadFloat (out customHeight)) {
				MyLog ("Adjusting to height = " + customHeight);
				AdjustToHeight (customHeight);
			} else {
				Debug.Log ("no height config file found, using default");
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		void AdjustToHeight (float customHeight)
		{
			float ratio = customHeight / initHeight;
			leftFootOffsetObject.transform.position = initModelState.ankleMarkerLeftPos * ratio;
			rightFootOffsetObject.transform.position = initModelState.ankleMarkerRightPos * ratio;
			headOffsetObject.transform.position = initModelState.markerHeadPos * ratio;
			transform.localScale = initModelState.modelScale * ratio;
		}

		//-------------------------------------------------------------------------------------------------------------
		void StartIK ()
		{

			int headIndex = -1;
			var tpIkComps = GetComponents<ThreePointIK> ();

			foreach (var item in tpIkComps) {
				// 開始手動更新IK
				item.manualUpdateIK = true;
				item.enabled = true;

				ikList.Add (item);
			}

			// Find the ThreePointIK is head.
			headIndex = ikList.FindIndex (item => item.bendNormalStrategy == ThreePointIK.BendNormalStrategy.head);
			if (headIndex >= 0)

			// Switch the head to first.
			Swap (ikList, 0, headIndex);
		}

		//-------------------------------------------------------------------------------------------------------------
		void UpdateIK ()
		{
			foreach (var item in ikList) {
				item.UpdateIK ();
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		Pose GetPose (int index)
		{
			var trans = deviceMarkerDict [index];
			Pose pose = new Pose { pos = trans.position, rot = trans.rotation };
			return pose;
		}

		//-------------------------------------------------------------------------------------------------------------
		void StartOffsetTracking ()
		{
			List<TrackerRole> keys = new List<TrackerRole> { TrackerRole.Torso, TrackerRole.FootLeft, TrackerRole.FootRight };
			List<Transform> values = new List<Transform> { bodyOffsetObject.transform, leftFootOffsetObject.transform, rightFootOffsetObject.transform };

			// Create a offset List.
			foreach (var item in trackers) {
				int index = keys.IndexOf (item.Key);
				if (index >= 0) {
					var trackedInfo = new OffsetTracking ();
					trackedInfo.deviceIndex = item.Value;
					trackedInfo.trackerRole = keys [index];
					trackedInfo.targetTrans = values [index];
					trackedInfo.deviceMarkerDict = deviceMarkerDict;
					trackedInfo.StartTracking ();
					offsetTrackedList.Add (trackedInfo);
				}
			}

			headOffsetObject.transform.parent = Camera.main.transform;
		}

		//-------------------------------------------------------------------------------------------------------------
		void UpdateOffsetTracking ()
		{
			foreach (var item in offsetTrackedList)
				item.UpdateOffsetTracking ();

			if (animationBlender != null && trackers.ContainsKey (TrackerRole.HandLeft)) {
				int leftHandIndex = trackers [TrackerRole.HandLeft];
				float triggerValue = SteamVR_Controller.Input (leftHandIndex).GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
				animationBlender.lerpValue = triggerValue;
			}

			if (trackers.ContainsKey (TrackerRole.HandRight)) {
				int leftHandIndex = trackers [TrackerRole.HandRight];
				float triggerValue = SteamVR_Controller.Input (leftHandIndex).GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
				UpdatePistolEffect (triggerValue);
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		void UpdatePistolEffect (float triggerValue)
		{

		}

		//-------------------------------------------------------------------------------------------------------------
		void Swap<T> (List<T> list, int indexA, int indexB)
		{
			T temp = list [indexA];
			list [indexA] = list [indexB];
			list [indexB] = temp;
		}

		//-------------------------------------------------------------------------------------------------------------
		bool AssignTrackers ()
		{
			trackers.Clear ();

			List<KeyValuePair<int, Vector3>> devices = new List<KeyValuePair<int, Vector3>> ();
			for (int i = (int)SteamVR_TrackedObject.EIndex.Device1; i < (int)SteamVR_TrackedObject.EIndex.Device15; i++) {
				var device = SteamVR_Controller.Input (i);
				if (device.hasTracking && device.connected && device.valid) {
					//	判斷此tracker的種類(不包含攝影機)
					var deviceClass = Valve.VR.OpenVR.System.GetTrackedDeviceClass ((uint)i);
					if (deviceClass == Valve.VR.ETrackedDeviceClass.Controller || deviceClass == Valve.VR.ETrackedDeviceClass.GenericTracker) {
						// 保存Tracker的位置、旋轉
						devices.Add (new KeyValuePair<int, Vector3> (i, GetPose (i).pos));
						Debug.Log ((SteamVR_TrackedObject.EIndex)i + ", type = " + deviceClass);
					} else {
						MyLog ("Device" + i + " is a basestation, type = " + deviceClass);
					}
				}
			}

			MyLog ("device count = " + devices.Count);

			// 排序(由低到高)
			devices.Sort ((a, b) => a.Value.y.CompareTo (b.Value.y));

			//依Tracker的數量為主
			if (devices.Count == 6) {
				if (devices [0].Value.x < 0f)
					Swap (devices, 0, 1);
				if (devices [4].Value.x < 0f)
					Swap (devices, 4, 5);

				// foot right
				trackers [TrackerRole.FootRight] = devices [0].Key;
				// foot left
				trackers [TrackerRole.FootLeft] = devices [1].Key;
				// torso
				trackers [TrackerRole.Torso] = devices [3].Key;
				// hand right
				trackers [TrackerRole.HandRight] = devices [4].Key;
				// hand left
				trackers [TrackerRole.HandLeft] = devices [5].Key;

				// Setting hand right and left target.
				rightHandTarget = deviceMarkerDict [devices [4].Key];
				leftHandTarget = deviceMarkerDict [devices [5].Key];
				// projecter
				transProjectTracker = deviceMarkerDict [devices [2].Key];
			} else if (devices.Count == 5) {
				// Foot Right + Torso + Hand right and left 
				/*
			if (devices[0].Value.x < 0f)
				Swap(devices, 0, 1);
			*/
				if (devices [3].Value.x < 0f)
					Swap (devices, 3, 4);

				trackers [TrackerRole.FootRight] = devices [0].Key;
				//trackers[TrackerRole.FootLeft] = devices[1].Key;
				trackers [TrackerRole.Torso] = devices [2].Key;
				trackers [TrackerRole.HandRight] = devices [3].Key;
				trackers [TrackerRole.HandLeft] = devices [4].Key;

				//設定左右手的位置
				rightHandTarget = deviceMarkerDict [devices [3].Key];
				leftHandTarget = deviceMarkerDict [devices [4].Key];
				// projecter
				transProjectTracker = deviceMarkerDict [devices [1].Key];
			} else if (devices.Count == 4) {
				// Foot Right + Torso + Hand Right
				/*
			if (devices[0].Value.x < 0f)
				Swap(devices, 0, 1);
			*/

				trackers [TrackerRole.FootRight] = devices [0].Key;
				//trackers[TrackerRole.FootLeft] = devices[1].Key;
				trackers [TrackerRole.Torso] = devices [2].Key;

				//判斷左右手
				if (devices [3].Value.x < 0f) {
					trackers [TrackerRole.HandLeft] = devices [3].Key;
					leftHandTarget = deviceMarkerDict [devices [3].Key];
				} else {
					trackers [TrackerRole.HandRight] = devices [3].Key;
					rightHandTarget = deviceMarkerDict [devices [3].Key];
				}
				// projecter
				transProjectTracker = deviceMarkerDict [devices [1].Key];
			} else if (devices.Count == 3) {
				// Hand Right + Torse
				//trackers[devices[0].Value.x < 0f? TrackerRole.FootLeft : TrackerRole.FootRight] = devices[0].Key;

				//Torso
				trackers [TrackerRole.Torso] = devices [1].Key;

				// Hand
				if (devices [2].Value.x < 0f) {
					trackers [TrackerRole.HandLeft] = devices [2].Key;
					leftHandTarget = deviceMarkerDict [devices [2].Key];
				} else {
					trackers [TrackerRole.HandRight] = devices [2].Key;
					rightHandTarget = deviceMarkerDict [devices [2].Key];
				}
				// projecter
				transProjectTracker = deviceMarkerDict [devices [0].Key];
			} else if (devices.Count == 2) {
				// Torso
				trackers [TrackerRole.Torso] = devices [1].Key;
				/*
			if (devices [1].Value.x < 0f) {
				trackers [TrackerRole.HandLeft] = devices [1].Key;
				leftHandTarget = deviceMarkerDict[devices[1].Key];
			} else {
				trackers [TrackerRole.HandRight] = devices [1].Key;
				rightHandTarget = deviceMarkerDict[devices[1].Key];
			}
			*/
				// Projecter
				transProjectTracker = deviceMarkerDict [devices [0].Key];
			} else {
				//Tracker不足
				return false;
			}

			// 修正左右手誤差
			if (leftHandTarget != null && leftHandOffsetObject != null) {
				AssignChildAndKeepLocalTrans (ref leftHandTarget, leftHandOffsetObject.transform);
			}
			if (rightHandTarget != null && rightHandOffsetObject != null) {
				AssignChildAndKeepLocalTrans (ref rightHandTarget, rightHandOffsetObject.transform);
			}
			// Modify the offset of the project's tracker transform.
			if (transProjectTracker != null && projectOffsetObject != null) {
				AssignChildAndKeepLocalTrans (ref transProjectTracker, projectOffsetObject.transform);
			}
			
			string strTrackers = "";
			foreach (var item in trackers.Keys)
				strTrackers += item + ",";
			MyLog ("bound body parts: " + strTrackers);

			return true;
		}

		//-------------------------------------------------------------------------------------------------------------
		void AssignChildAndKeepLocalTrans (ref Transform parent, Transform child)
		{
			Vector3 localPos = child.localPosition;
			Vector3 localScale = child.localScale;
			Quaternion localRot = child.localRotation;
			child.parent = parent;
			child.localPosition = localPos;
			child.localScale = localScale;
			child.localRotation = localRot;
			parent = child;
		}

		//-------------------------------------------------------------------------------------------------------------
		void MyLog (object msg)
		{
			Debug.Log (msg);

			if (logText != null) {
				//Assign
				logQueue.Enqueue (msg);
				if (logQueue.Count > maxLogCount)
					logQueue.Dequeue ();
				DisplayLogQueue ();
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		void MyLogError (object msg)
		{
			Debug.LogError (msg);

			if (logText != null) {
				logQueue.Enqueue ("ERROR: " + msg);
				if (logQueue.Count > maxLogCount)
					logQueue.Dequeue ();
				DisplayLogQueue ();
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		void DisplayLogQueue ()
		{
			string str = "";
			foreach (var item in logQueue) {
				str += item + "\n";
			}
			logText.text = str;
		}
	}

}
