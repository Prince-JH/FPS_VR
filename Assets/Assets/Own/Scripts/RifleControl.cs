using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleControl : MonoBehaviour
{
    //현재 장착된 총
    [SerializeField]
    private Rifle currentRifle;
    [SerializeField]
    private ReloadSound reloadSound;

    //연사 속도 계산
    private float currentFireRate;
    //상태 변수
    private bool isReload = false;
    [HideInInspector]
    public bool isAimMode = false;

    //원래 포지션 값
    private Vector3 originPos;

    //효과음 재생
    private AudioSource audio;
    //레이저 충돌 정보
    private RaycastHit hitInfo;

    //필요 컴포넌트
    [SerializeField]
    private Camera theCam;

    //피격 이펙트
    [SerializeField]
    private GameObject hit_effect_prefab;
    private void Start()
    {
        originPos = Vector3.zero;
        audio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        RifleFireRateCalc();
        Fire();
        TryReload();
        TryAim();
    }
    //연사속도 재계산
    private void RifleFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; //1초에 1 감소
    }
    //발사 시도
    private void Fire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            if (!isReload)
            {
                if (currentRifle.currentBulletCount > 0)
                    Shoot();
                else
                    StartCoroutine(Reload());
            }
        }
    }
    //발사
    private void Shoot()
    {
        currentRifle.currentBulletCount--;
        currentFireRate = currentRifle.fireRate; //연사 속도 재계산
        currentRifle.muzzleFlash.Play();
        audio.Play();
    }

    //재장전 시도
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentRifle.currentBulletCount < currentRifle.reloadBulletCount)
        {
            StartCoroutine(Reload());
        }
    }
    //재장전
    IEnumerator Reload()
    {
        if (currentRifle.carryBulletCount > 0)
        {
            isReload = true;
            currentRifle.animator.SetTrigger("Reload");
            reloadSound.isPlay = true;

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
            Debug.Log("총알 없음");
        }
    }
    //정조준 시도
    private void TryAim()
    {
        if (Input.GetButtonDown("Fire2") && !isReload)
        {
            Aim();
        }
    }
    //정조준
    private void Aim()
    {
        isAimMode = !isAimMode;
        currentRifle.animator.SetBool("Aim", isAimMode);
        if (isAimMode)
        {
            StopAllCoroutines();
            StartCoroutine(AimCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ButtstockCoroutine());
        }
    }

    //정조준 활성화
    IEnumerator AimCoroutine()
    {
        while (currentRifle.transform.localPosition != currentRifle.getAimOriginPos())
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, currentRifle.getAimOriginPos(), 0.2f);
            yield return null;
        }
        theCam.fieldOfView = 13.4f;
        theCam.nearClipPlane = 0.01f;
    }
    //정조준 비활성화
    IEnumerator ButtstockCoroutine()
    {
        while (currentRifle.transform.localPosition != originPos)
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
        theCam.fieldOfView = 60;
        theCam.nearClipPlane = 0.3f;
    }


    public Rifle GetGun()
    {
        return this.currentRifle;
    }
    public bool GetAimMode()
    {
        return isAimMode;
    }
}
