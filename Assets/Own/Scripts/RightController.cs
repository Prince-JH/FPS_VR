using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RightController : MonoBehaviour
{
    private SteamVR_Behaviour_Pose pose;
    private SteamVR_Input_Sources hand;
    private LineRenderer line;

    public float maxDistance = 100.0f;
    private RaycastHit hit;
    private Transform tr;
    private GameObject prevObject;
    private GameObject currObject;
    public GameObject pointer;

    public Color color = Color.blue;
    public Color clickedColor = Color.green;
    void Start()
    {
        tr = GetComponent<Transform>();
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        hand = pose.inputSource;

        CreateLineRenderer();
    }
    void CreateLineRenderer()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.receiveShadows = false;

        line.positionCount = 2;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, new Vector3(0, 0, maxDistance));

        line.startWidth = 0.03f;
        line.endWidth = 0.005f;

        line.material = new Material(Shader.Find("Unit/Color"));
        line.material.color = this.color;
    }
    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(tr.position, tr.forward, out hit, maxDistance))
        {
            line.SetPosition(1, new Vector3(0, 0, hit.distance));

            pointer.transform.position = hit.point + (hit.normal * 0.01f);
            pointer.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            pointer.transform.position = tr.position + (tr.forward * maxDistance);
            pointer.transform.rotation = Quaternion.LookRotation(tr.forward);
        }
    }
}
