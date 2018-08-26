using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace edu.nctu.nllab.PJettingPush
{
    public class Events : MonoBehaviour
    {

		public UnityEvent eventsOnStart;

        // Use this for initialization
        void Start()
        {
			eventsOnStart.Invoke();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


