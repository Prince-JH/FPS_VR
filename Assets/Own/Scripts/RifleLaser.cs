using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class RifleLaser : MonoBehaviour
{
    public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    public SteamVR_Action_Boolean trigger;

    private Vector3 laserDir;
    private LineRenderer layser;
    private RaycastHit hitObj;
    private GameObject currentHitObj;

    public float raycastDistance = 2f;
    // Start is called before the first frame update
    void Start()
    {
        layser = this.gameObject.AddComponent<LineRenderer>();
        Material material = new Material(Shader.Find("Standard"));
        material.color = new Color(0, 195, 255, 0.5f);
        layser.material = material;
        layser.positionCount = 2;
        layser.startWidth = 0.01f;
        layser.endWidth = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        layser.SetPosition(0, transform.position);
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green, 0.5f);
        if(Physics.Raycast(transform.position, transform.forward, out hitObj, raycastDistance))
        {
            layser.SetPosition(1, hitObj.point);

            if(hitObj.collider.gameObject.transform.tag == "Button")
            {
                if (trigger.GetStateDown(rightHand))
                    hitObj.collider.gameObject.GetComponent<Button>().onClick.Invoke();
            }
            else
            {
                currentHitObj = hitObj.collider.gameObject;
            }
        }
        else
        {
            layser.SetPosition(1, transform.position + (transform.forward * raycastDistance));
        }
    }
}
