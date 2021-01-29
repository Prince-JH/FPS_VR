using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string name;
    public float range;
    public float reloadTime;

    public int reloadBulletCount;
    public int currentBulletCount;
    public int carryBulletCount;

    public float resistForce;
    public float resistAimForce;

    public Animator animator;
    public ParticleSystem muzzleFlash;
    protected Vector3 aimOriginPos;
    protected Quaternion aimOriginRot;
    [SerializeField]
    private Camera cam;
    private void Start()
    {
        aimOriginPos = new Vector3(0.03f, -0.052f, 0.28f);
        aimOriginRot = Quaternion.Euler(new Vector3(0.002f, -0.294f, -0.014f));
        //aimOriginPos = Vector3.zero;
        //aimOriginRot = Quaternion.Euler(Vector3.zero);
    }
    //protected Vector3 aimOriginPos = new Vector3(-0.14f, 0.059f, -0.11f);

    public Vector3 GetAimOriginPos()
    {
        return aimOriginPos;
    }
    public Quaternion GetAimOriginRot()
    {
        return aimOriginRot;
    }
}
