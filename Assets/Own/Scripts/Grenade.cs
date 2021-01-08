using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    Rigidbody rigid;
    [SerializeField]
    private GameObject explosion;
    public static bool isExplode = false;
    
    private void Start()
    {
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
    }
    
}
