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
    protected Vector3 aimOriginPos = new Vector3(-0.14f, 0.059f, -0.11f);

    public Vector3 getAimOriginPos()
    {
        return aimOriginPos;
    }
}
