using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed = 250.0f;
    private Rigidbody rig;
    //라이트
    private GameObject light1;
    private GameObject light2;
    private Animator lightAnimator1;
    private Animator lightAnimator2;


    void Start()
    {
        light1 = GameObject.Find("Sun1");
        lightAnimator1 = light1.GetComponent<Animator>();
        light2 = GameObject.Find("Sun2");
        lightAnimator2 = light2.GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rig.AddForce(gameObject.transform.localRotation * Vector3.forward * speed + new Vector3(10, 0, 0));
        if (PlayerMove.healthPoint <= 0)
        {
            lightAnimator1.enabled = false;
            lightAnimator2.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            lightAnimator1.SetTrigger("Hit");
            lightAnimator2.SetTrigger("Hit");
        }
        Destroy(gameObject);
    }
}
