
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 3, 2018
	Purpose: Define the point of jetting.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace edu.nctu.nllab.PJettingPush
{
	[System.Serializable]
	public class JetPoint
	{

		public enum Type{
			Pluse,
		};

		public bool flagHit;
		public float enableTime;
		public float force = 1.0f;
		public float interval = 1.0f;
		public float size = 1.0f;
		public Type type = Type.Pluse;
		public Transform trans;

		public JetPoint(){
			
		}

		public JetPoint(bool _flag, float _enableTime, float _force, float _interval, float _size, Type _type, Transform _trans){
			this.flagHit = _flag;
			this.enableTime = _enableTime;
			this.force = _force;
			this.interval = _interval;
			this.size = _size;
			this.type = _type;
			this.trans = _trans;
		}

		public JetPoint(float _enableTime, float _force, float _interval, float _size, Type _type, Transform _trans){
			this.enableTime = _enableTime;
			this.force = _force;
			this.interval = _interval;
			this.size = _size;
			this.type = _type;
			this.trans = _trans;
		}

		public void Refresh(bool _flag){
			this.flagHit = _flag;
		}

		public void Refresh(bool _flag, float _enableTime){
			this.flagHit = _flag;
			this.enableTime = _enableTime;
		}

		public void Refresh(bool _flag, Vector3 _pos){
			this.flagHit = _flag;
			this.trans.position = _pos;
		}

		public void Refresh(bool _flag, float _enableTime, Vector3 _pos){
			this.flagHit = _flag;
			this.enableTime = _enableTime;
			this.trans.position = _pos;
			//Debug.Log("jet point refresh");
		}

		/*
			This is used to return the distance from the jeted point to the projector. 
		*/
		public float GetJetDistance(){
			return size;
		}

		public float GetShowScale(){
			return size;
		}
	}
}
