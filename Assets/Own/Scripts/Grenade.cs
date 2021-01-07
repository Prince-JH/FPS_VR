using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    Rigidbody rigid;
    [SerializeField]
    private float speed;
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.velocity = transform.position * speed;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
