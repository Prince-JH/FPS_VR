﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RifleControl : MonoBehaviour
{
    public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    public SteamVR_Action_Boolean trigger;
    public SteamVR_Action_Boolean grip;
    public SteamVR_Action_Boolean trackPadTouch;
    public SteamVR_Action_Boolean trackPadClick;

    //현재 장착된 총
    [SerializeField]
    private Rifle currentRifle;
    //사격 소리
    [SerializeField]
    private ReloadSound reloadSound;
    //플레이어
    [SerializeField]
    private PlayerMove player;
    //총알
    [SerializeField]
    private GameObject bullet;
    private Transform bulletPos;
    //발사 방향
    private Vector3 bulletDir;
    //연사 속도 계산
    private float currentFireRate;
    //상태 변수
    private bool isReload = false;
    [HideInInspector]
    public bool isAimMode = false;
    [HideInInspector]
    public bool zoomOutComplete = false;
    //원래 포지션 값
    private Vector3 originPos;
    private Quaternion originRot;
    private Vector3 aimOriginPos;
    public static bool rifleFire = false;
    //효과음 재생
    private AudioSource audio;
    //레이저 충돌 정보
    private RaycastHit hitInfo;

    //필요 컴포넌트
    [SerializeField]
    private Camera theCam;
    [SerializeField]
    private Camera mainCam;
    private float scroll;

    //피격 이펙트
    [SerializeField]
    private GameObject hitEffect;
    [SerializeField]
    private GameObject mapHitEffect;
    //크로스헤어
    [SerializeField]
    private Crosshair crosshair;
    private NoBulletSound noBulletSound;
    [SerializeField]
    private GameObject originParent;
    private void Start()
    {
        //originPos = new Vector3(0.07f, -0.07f, 0.035f);
        //originRot = Quaternion.Euler(new Vector3(0, 0, 0.9f));
        aimOriginPos = new Vector3(0.07f, -0.07f, 0.035f);
        originPos = Vector3.zero;
        originRot = Quaternion.Euler(new Vector3(0, 0, 0.9f));

        audio = GetComponent<AudioSource>();
        bulletPos = GameObject.Find("RifleMuzzleFlash").transform;
        noBulletSound = FindObjectOfType<NoBulletSound>();

        WeaponManager.currentWeapon = currentRifle;
        WeaponManager.currentWeaponAnimator = currentRifle.animator;
    }
    private void Update()
    {
        if (!GameManager.isPause && GameManager.isPlay)
        {
            RifleFireRateCalc();
            Fire();
            FireDirection();
            TryReload();
            TryAim();
            ZoomOutCheck();
            SniperZoom();
        }
    }
    //발사 방향
    private void FireDirection()
    {
        if (isAimMode)
            bulletDir = mainCam.transform.forward;
        else
        {
            bulletDir = currentRifle.transform.forward +
                new Vector3(Random.Range(-crosshair.getAccuracy() - currentRifle.accuracy, crosshair.getAccuracy() + currentRifle.accuracy)
                , Random.Range(-crosshair.getAccuracy() - currentRifle.accuracy, crosshair.getAccuracy() + currentRifle.accuracy), 0);
        }
    }
    public Vector3 GetFiredDirection()
    {
        return bulletDir;
    }
    //연사속도 재계산
    private void RifleFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; //1초에 1 감소
    }
    //발사 시도
    public void Fire()
    {
        if (trigger.GetStateDown(rightHand) && currentFireRate <= 0 && !isReload)
        {
            rifleFire = true;
            if (currentRifle.currentBulletCount > 0)
                Shoot();
            else
            {
                CancelAim();
                StartCoroutine(Reload());
            }

        }
        if (trigger.GetStateUp(rightHand))
            rifleFire = false;
    }
    /*
    private void Fire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            rifleFire = true;
            if (currentRifle.currentBulletCount > 0)
                Shoot();
            else
            {
                CancelAim();
                StartCoroutine(Reload());
            }

        }
        if (Input.GetButtonUp("Fire1"))
            rifleFire = false;
    }
    */
    //발사
    private void Shoot()
    {
        currentRifle.currentBulletCount--;
        //연사 속도  재계산
        currentFireRate = currentRifle.fireRate;
        currentRifle.muzzleFlash.Play();
        audio.Play();
        StartCoroutine(ShootBullet());
        //Hit();
        player.animator.SetTrigger("Shoot");
        if (!isAimMode)
            crosshair.FireAnimation();
        //총기 반동 코루틴
        StopAllCoroutines();
        StartCoroutine(RetroactionCoroutine());
    }
    IEnumerator ShootBullet()
    {
        GameObject bulletClone = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Destroy(bulletClone, 2.0f);
        //GameObject bulletClone = BulletObjectPool.GetObject();
        yield return null;
        //BulletObjectPool.ReturnObject;
    }

    //피격
    private void Hit()
    {
        if (Physics.Raycast(theCam.transform.position, bulletDir, out hitInfo, currentRifle.range))
        {
            if (hitInfo.transform.tag == "Enemy")
            {
                GameObject clone = Instantiate(hitEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(clone, 0.5f);
                hitInfo.transform.GetComponent<Enemy>().hp--;
                hitInfo.transform.GetComponent<Animator>().SetTrigger("Hit");
            }
            else if (hitInfo.transform.tag == "Map")
            {
                GameObject clone = Instantiate(mapHitEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(clone, 0.5f);
            }
        }
    }
    IEnumerator RetroactionCoroutine()
    {
        Vector3 recoilBack = new Vector3(originPos.x, originPos.y, -currentRifle.resistForce);
        Vector3 aimRecoilBack = new Vector3(currentRifle.GetAimOriginPos().x, currentRifle.GetAimOriginPos().y, -currentRifle.resistAimForce);
        if (!isAimMode)
        {
            gameObject.transform.localPosition = originPos;

            //반동
            while (gameObject.transform.localPosition.z > -currentRifle.resistForce + 0.02f)
            {
                gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            //원위치
            while (gameObject.transform.localPosition.z < originPos.z - 0.02f)
            {
                gameObject.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            gameObject.transform.localPosition = currentRifle.GetAimOriginPos();

            //반동
            while (gameObject.transform.localPosition.z > -currentRifle.resistAimForce + 0.2595f)
            {
                gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, aimRecoilBack, 0.4f);
                yield return null;
            }
            //원위치
            while (gameObject.transform.localPosition.z < currentRifle.GetAimOriginPos().z - 0.00002f)
            {
                gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, currentRifle.GetAimOriginPos(), 0.2f);
                yield return null;
            }
        }
    }

    //재장전 시도
    private void TryReload()
    {
        if (grip.GetStateDown(rightHand) && !isReload && currentRifle.currentBulletCount < currentRifle.reloadBulletCount)
        {
            CancelAim();
            StartCoroutine(Reload());
        }
    }
    /*
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentRifle.currentBulletCount < currentRifle.reloadBulletCount)
        {
            CancelAim();
            StartCoroutine(Reload());
        }
    }
    */
    //재장전
    IEnumerator Reload()
    {
        if (currentRifle.carryBulletCount > 0)
        {
            isReload = true;
            currentRifle.animator.SetTrigger("Reload");
            reloadSound.ReloadSoundPlay();

            yield return new WaitForSeconds(currentRifle.reloadTime);
            currentRifle.carryBulletCount += currentRifle.currentBulletCount;
            currentRifle.currentBulletCount = 0;
            if (currentRifle.carryBulletCount >= currentRifle.reloadBulletCount)
            {
                currentRifle.currentBulletCount = currentRifle.reloadBulletCount;
                currentRifle.carryBulletCount -= currentRifle.reloadBulletCount;
            }
            else
            {
                currentRifle.currentBulletCount = currentRifle.carryBulletCount;
                currentRifle.carryBulletCount = 0;
            }
            isReload = false;

        }
        else
        {
            noBulletSound.SoundPlay();
        }
    }
    //재장전 취소
    public void CancelReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }
    //정조준 시도
    private void TryAim()
    {
        if (trackPadClick.GetStateDown(rightHand) && !isReload && !rifleFire)
        {
            Aim();
        }
    }
    //정조준 취소
    public void CancelAim()
    {
        if (isAimMode)
            Aim();
    }
    //정조준
    private void Aim()
    {
        isAimMode = !isAimMode;
        currentRifle.animator.SetBool("Aim", isAimMode);
        crosshair.AimAnimation(isAimMode);
        if (isAimMode)
        {
            gameObject.transform.SetParent(theCam.gameObject.transform);
            StopAllCoroutines();
            gameObject.transform.localPosition = currentRifle.GetAimOriginPos();
            gameObject.transform.localRotation = currentRifle.GetAimOriginRot();
            //StartCoroutine(AimCoroutine());
            //StartCoroutine(FOVIn());
        }
        else
        {
            gameObject.transform.SetParent(originParent.transform);
            //mainCam.fieldOfView = 60;
            StopAllCoroutines();
            gameObject.transform.localPosition = originPos;
            gameObject.transform.localRotation = originRot;
            //StartCoroutine(ButtstockCoroutine());
            //StartCoroutine(FOVOut());
        }
    }
    //정조준 활성화
    IEnumerator AimCoroutine()
    {
        //Debug.Log(currentRifle.transform.localPosition);
        //Debug.Log(currentRifle.GetAimOriginPos());
        //Debug.Log(currentRifle.transform.localPosition == currentRifle.GetAimOriginPos());
        while (currentRifle.transform.localPosition != currentRifle.GetAimOriginPos())
        {
            Debug.Log(currentRifle.transform.localPosition);
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, currentRifle.GetAimOriginPos(), 0.2f);
            currentRifle.transform.localRotation = currentRifle.GetAimOriginRot();
            yield return null;
        }

    }
    //정조준 비활성화
    IEnumerator ButtstockCoroutine()
    {
        while (currentRifle.transform.localPosition != originPos)
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, originPos, 0.2f);
            currentRifle.transform.localRotation = originRot;
            yield return null;
        }


    }
    IEnumerator FOVIn()
    {

        while (theCam.fieldOfView > 22)
        {
            theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, 22, 0.2f);
            yield return null;
        }

    }
    IEnumerator FOVOut()
    {
        while (theCam.fieldOfView < 60)
        {
            theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, 60, 0.2f);
            yield return null;
        }

    }
    private void ZoomOutCheck()
    {
        if (theCam.fieldOfView >= 59.5f)
            zoomOutComplete = true;
        else
            zoomOutComplete = false;
    }
    public Rifle GetRifle()
    {
        return this.currentRifle;
    }
    public bool GetAimMode()
    {
        return isAimMode;
    }
    public void GunChange(Rifle gun)
    {
        WeaponManager.currentWeapon.gameObject.transform.parent.gameObject.SetActive(false);
        currentRifle = gun;
        WeaponManager.currentWeapon = currentRifle;
        WeaponManager.currentWeaponAnimator = currentRifle.animator;
        reloadSound.ReloadSoundStop();

        currentRifle.transform.localPosition = Vector3.zero;
        currentRifle.gameObject.transform.parent.gameObject.SetActive(true);
    }
    private void SniperZoom()
    {
        if (isAimMode)
        {
            StartCoroutine(MainCamZoom());
        }
    }
    IEnumerator MainCamZoom()
    {
        scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0)
        {
            if (mainCam.fieldOfView < 60)
            {
                mainCam.fieldOfView += 4;
                yield return null;
            }
        }
        else if (scroll > 0)
        {
            if (mainCam.fieldOfView > 12)
            {
                mainCam.fieldOfView -= 4;
                yield return null;
            }
        }
    }
}
