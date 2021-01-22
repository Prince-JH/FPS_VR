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
    private GrenadeSound grenadeSound;

    private void Start()
    {
        gL = FindObjectOfType<GL>();
        rigid = GetComponent<Rigidbody>();
        grenadeSound = FindObjectOfType<GrenadeSound>();
        StartCoroutine(Explosion());
    }
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(1f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        GameObject clone = Instantiate(explosion, transform.position, transform.rotation);
        gameObject.GetComponent<SphereCollider>().radius = gL.explosionRange;
        grenadeSound.SoundPlay();
        Destroy(clone, 1.5f);

        /*
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, gL.explosionRange, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitObject in rayHits)
        {
            Debug.Log(gL.explosionRange);
            //피격 이펙트 + 체력깎임
            hitObject.transform.GetComponent<Enemy>().hp -= 30;
        }
        */
        
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position + Vector3.up * gL.explosionRange);
        Gizmos.DrawWireSphere(transform.position + Vector3.up, 8);
    }
    */
    private void OnCollisionEnter(Collision collision)
    {
        rigid.velocity *= 0.7f;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            //RaycastHit[] rayHits = Phy
            collision.transform.GetComponent<Enemy>().hp -= 30;
        }
    }
}
