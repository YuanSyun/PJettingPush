
/*
	Coder: YuanSyun Ye (yuansyuntw@gmail.com)
	Date: August, 9, 2018
	Purpose: 
		偵測在噴嘴上的按鈕，透過serial port與Arduino溝通。
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.IO.Ports;
using UnityEngine.Events;

public class JetButtonDetector : MonoBehaviour
{

    #region public variable

    public string jetArduinoSerialPortName = "CoM4";
    public int baudRate = 9600;

    [HideInInspector]
    public bool flagTrigger;

    public UnityEvent eventsTriggerDown;
    public UnityEvent eventsTriggerUp;

    #endregion





    #region private variable

    private SerialPort sp;
    private float timeDetectDelay = 0.04f; // 40ms
    private string dataReceive = "";
    private int state;

    #endregion






    #region lifecycle

    // Use this for initialization
    void Start()
    {
        if (jetArduinoSerialPortName == "")
        {
            _CheckSetting("The Jet arduino serial port name not seted", true);
        }

        // Connect the serial port.
        sp = new SerialPort(jetArduinoSerialPortName, baudRate);
        try
        {
            // Enable the serial port.
            sp.Open();
            sp.ReadTimeout = 10; // 10ms
        }
        catch (IOException e)
        {
            _CheckSetting(e.ToString(), true);
        }

        // Start the detector
        StartCoroutine(_Detector());
    }

    // Update is called once per frame
    void Update()
    {
        if (dataReceive != "")
        {
            Debug.Log(dataReceive);
            try
            {
                state = int.Parse(dataReceive);

                if ((state == 1) && (!flagTrigger))
                {
                    flagTrigger = true;
                    eventsTriggerDown.Invoke();
                }
                else if ((state == 0) && (flagTrigger))
                {
                    flagTrigger = false;
                    eventsTriggerUp.Invoke();
                }
            }
            catch (FormatException e)
            {
                //Debug.Log(e);
            }
        }
    }

    #endregion






    IEnumerator _Detector()
    {
        //Debug.Log("start detector");

        while (true)
        {
            if (sp.IsOpen)
            {
                try
                {
                    dataReceive = sp.ReadLine();
                }
                catch (TimeoutException e)
                {
                    //Debug.Log(e);
                }
            }

            yield return new WaitForSeconds(timeDetectDelay);
        }
    }

    void _CheckSetting(string warning, bool destory)
    {
        Debug.Log("JetButtonDetector.cs - " + warning);
        if (destory)
        {
            this.enabled = false;
        }
    }
}
