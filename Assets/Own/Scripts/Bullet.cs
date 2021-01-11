using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 250.0f;
    private Rigidbody rig;
    private RifleControl rifleControl;
    // Start is called before the first frame update
    void Start()
    {
        rifleControl = FindObjectOfType<RifleControl>();
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rig.AddForce(rifleControl.getFiredDirection() * speed);
    }
    
}
