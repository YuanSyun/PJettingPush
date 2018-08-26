using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloManager : MonoBehaviour {

    public GameObject Head;
    public GameObject haloTarget;

    float angle_threshold = 20;

    public GameObject HaloSphere;

	// Use this for initialization
	void Start () {
		
        HaloSphere.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        if(haloTarget != null)
        {
            Vector3 head_forward = Head.transform.forward;
            Vector3 head_position = Head.transform.position;
            Vector3 target_position = haloTarget.transform.position;
            Vector3 target_direction = target_position - head_position;

            float angle = Vector3.Angle(target_direction, head_forward);

             //print("Angle "+ angle + " Button dirction " + button_direction + " head forward " + head_forward + " dot " + Vector3.Dot(button_direction,head_forward));
            Debug.DrawRay(head_position, head_forward, Color.blue);
            Debug.DrawRay(head_position, target_direction, Color.green);

            if (angle > angle_threshold)
            {
                GameObject SphereUI = GameObject.Find("SphereUI");
                //float sphere_radius = SphereUI.transform.localScale.x / 2.0f;
                float sphere_radius = Vector3.Distance(target_position, head_position);

                Vector3 head_forward_on_surface = head_position + head_forward * sphere_radius;

                Vector3 normal = Vector3.Cross(head_forward, target_direction);

                GameObject headOnSurfaceObj = new GameObject();
                headOnSurfaceObj.transform.position = head_forward_on_surface;
                headOnSurfaceObj.transform.RotateAround(head_position, normal, angle_threshold);        

                Debug.DrawRay(head_position, headOnSurfaceObj.transform.position - head_position, Color.red); 
                Debug.DrawRay(target_position, headOnSurfaceObj.transform.position, Color.yellow);
                float haloSphere_Radius = Vector3.Distance(target_position, headOnSurfaceObj.transform.position);

                //Set HaloSphere
                HaloSphere.transform.position = target_position;
                HaloSphere.transform.localScale = new Vector3(haloSphere_Radius,haloSphere_Radius,haloSphere_Radius) * 2.0f;
                HaloSphere.transform.parent = SphereUI.transform;
                HaloSphere.SetActive(true);

                Destroy(headOnSurfaceObj);
            }
            else
                HaloSphere.SetActive(false);
        }
        else
        {
            HaloSphere.SetActive(false);
        }
	}
}
