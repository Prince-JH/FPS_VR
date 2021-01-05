using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    public float range;
    public float accuracy;
    public float fireRate;
    public float reloadTime;

    public int reloadBulletCount;
    public int currentBulletCount;
    public int carryBulletCount;

    public float resistForce;
    public float resistAimForce;

    public Animator animator;
    public ParticleSystem muzzleFlash;
    public AudioClip fireSound;
    private Vector3 aimOriginPos;
    private void Start()
    {
        aimOriginPos = this.transform.localPosition;
    }

    public Vector3 getAimOriginPos()
    {
        return aimOriginPos;
    }
}
