using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleControl : MonoBehaviour
{
    //현재 장착된 총
    [SerializeField]
    private Rifle currentRifle;
    //사격 소리
    [SerializeField]
    private ReloadSound reloadSound;
    //플레이어
    [SerializeField]
    private PlayerMove player;
    
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
    //크로스헤어
    [SerializeField]
    private Crosshair crosshair;
    private void Start()
    {
        originPos = Vector3.zero;
        audio = GetComponent<AudioSource>();

        WeaponManager.currentWeapon = currentRifle;
        WeaponManager.currentWeaponAnimator = currentRifle.animator;
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
                {
                    CancelAim();
                    StartCoroutine(Reload());
                }
            }
        }
    }
    //발사
    private void Shoot()
    {
        currentRifle.currentBulletCount--;
        //연사 속도  재계산
        currentFireRate = currentRifle.fireRate;
        currentRifle.muzzleFlash.Play();
        audio.Play();
        Hit();
        player.animator.SetTrigger("Shoot");
        if (!isAimMode)
            crosshair.FireAnimation();
        //총기 반동 코루틴
        StopAllCoroutines();
        StartCoroutine(RetroactionCoroutine());
    }
    //피격
    private void Hit()
    {
        if(Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo, currentRifle.range))
        {
            GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 1f);
        }
    }
    IEnumerator RetroactionCoroutine()
    {
        Vector3 recoilBack = new Vector3(originPos.x, originPos.y, -currentRifle.resistForce);
        Vector3 aimRecoilBack = new Vector3(currentRifle.getAimOriginPos().x, currentRifle.getAimOriginPos().y, -currentRifle.resistAimForce);
        if(!isAimMode)
        {
            currentRifle.transform.localPosition = originPos;
            
            //반동
            while(currentRifle.transform.localPosition.z > -currentRifle.resistForce + 0.02f)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            //원위치
            while(currentRifle.transform.localPosition.z < originPos.z - 0.02f)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentRifle.transform.localPosition = currentRifle.getAimOriginPos();

            //반동
            while (currentRifle.transform.localPosition.z > -currentRifle.resistAimForce + 0.02f)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, aimRecoilBack, 0.4f);
                yield return null;
            }
            //원위치
            while (currentRifle.transform.localPosition.z < currentRifle.getAimOriginPos().z - 0.02f)
            {
                currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, currentRifle.getAimOriginPos(), 0.1f);
                yield return null;
            }
        }
    }

    //재장전 시도
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentRifle.currentBulletCount < currentRifle.reloadBulletCount)
        {
            CancelAim();
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
    //재장전 취소
    public void CancelReload()
    {
        if(isReload)
        {
            StopAllCoroutines();
            isReload = false;
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
            StopAllCoroutines();
            StartCoroutine(AimCoroutine());
            StartCoroutine(ZoomIn());
            StartCoroutine(NearClipIn());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ButtstockCoroutine());
            StartCoroutine(ZoomOut());
            StartCoroutine(NearClipOut());
        }
    }
    //정조준 활성화
    IEnumerator AimCoroutine()
    {
        while (currentRifle.transform.localPosition != currentRifle.getAimOriginPos())
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, currentRifle.getAimOriginPos(), 0.2f);
            
        }
        yield return null;
    }
    //정조준 비활성화
    IEnumerator ButtstockCoroutine()
    {
        while (currentRifle.transform.localPosition != originPos)
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, originPos, 0.2f);
        }
        yield return null;
        
    }
    IEnumerator ZoomIn()
    {
        
        while(theCam.fieldOfView > 22)
        {
            theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, 22, 0.2f);
            yield return null;
        }
        
    }
    IEnumerator ZoomOut()
    {
        while (theCam.fieldOfView < 60)
        {
            theCam.fieldOfView = Mathf.Lerp(theCam.fieldOfView, 60, 0.2f);
            yield return null;
        }
        
    }
    IEnumerator NearClipIn()
    {
        while (theCam.nearClipPlane > 0.01f)
        {
            theCam.nearClipPlane = Mathf.Lerp(theCam.nearClipPlane, 0.01f, 0.2f);
            yield return null;
        }
        
    }
    IEnumerator NearClipOut()
    {
        while (theCam.nearClipPlane < 0.27f)
        {
            theCam.nearClipPlane = Mathf.Lerp(theCam.nearClipPlane, 0.27f, 0.2f);
            yield return null;
        }
        
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
        if(WeaponManager.currentWeapon != null && !isAimMode)
        {
            WeaponManager.currentWeapon.gameObject.transform.parent.gameObject.SetActive(false);
        }
        currentRifle = gun;
        WeaponManager.currentWeapon = currentRifle;
        WeaponManager.currentWeaponAnimator = currentRifle.animator;

        currentRifle.transform.localPosition = Vector3.zero;
        currentRifle.gameObject.transform.parent.gameObject.SetActive(true);
    }
}
