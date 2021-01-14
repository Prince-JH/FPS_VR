using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 300.0f;
    private Rigidbody rig;
    private RifleControl rifleControl;
    //피격 이펙트
    [SerializeField]
    private GameObject hitEffect;
    [SerializeField]
    private GameObject mapHitEffect;
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.transform.tag == "Enemy")
        {
            GameObject hitClone = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.identity);
            Destroy(hitClone, 0.5f);
            collision.gameObject.transform.GetComponent<Enemy>().hp--;
            collision.gameObject.transform.GetComponent<Animator>().SetTrigger("Hit");
        }
        else if (collision.gameObject.transform.tag == "EnemyHead")
        {
            GameObject hitClone = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.identity);
            Destroy(hitClone, 0.5f);
            collision.gameObject.GetComponentInParent<Enemy>().hp-=3;
            collision.gameObject.GetComponentInParent<Animator>().SetTrigger("HeadShot");
        }
        else if(collision.gameObject.transform.tag == "Map")
        {
            GameObject mapHitClone = Instantiate(mapHitEffect, collision.transform.position, Quaternion.identity);
            Destroy(mapHitClone, 0.5f);
        }
        Destroy(gameObject);
    }
}
