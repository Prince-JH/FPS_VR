using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private GL gL;
    Rigidbody rigid;
    [SerializeField]
    private GameObject explosion;
    public static bool isExplode = false;

    //피격 이펙트
    [SerializeField]
    private GameObject hitEffect;
    private void Start()
    {
        gL = FindObjectOfType<GL>();
        rigid = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Explosion());
    }
    
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(0.5f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        GameObject clone = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(clone, 0.7f);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, gL.explosionRange, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitObject in rayHits)
        {
            //피격 이펙트 + 체력깎임
            hitObject.transform.GetComponent<Enemy>().hp -= 30;
            GameObject hit = Instantiate(hitEffect, hitObject.point, Quaternion.LookRotation(hitObject.normal));
            Destroy(hit, 1f);
        }
    }
    
}
