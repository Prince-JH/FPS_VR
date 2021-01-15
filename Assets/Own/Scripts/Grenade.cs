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
    }
    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Explosion());
    }
    
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(1f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        GameObject clone = Instantiate(explosion, transform.position, transform.rotation);
        grenadeSound.SoundPlay();
        Destroy(clone, 1.2f);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, gL.explosionRange, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitObject in rayHits)
        {
            //피격 이펙트 + 체력깎임
            hitObject.transform.GetComponent<Enemy>().hp -= 30;
        }
    }
}
