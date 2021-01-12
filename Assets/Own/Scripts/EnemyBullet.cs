using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed = 250.0f;
    private Rigidbody rig;
    //라이트
    private GameObject light;
    void Start()
    {
        light = GameObject.Find("Sun");
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rig.AddForce(gameObject.transform.localRotation * Vector3.forward * speed + new Vector3(10, 0, 0));
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            
        }
    }
    
}
