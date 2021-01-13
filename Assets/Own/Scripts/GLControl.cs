using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLControl : MonoBehaviour
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

    //원래 포지션 값
    private Vector3 originPos;

    //효과음 재생
    private AudioSource audio;
    //레이저 충돌 정보
    private RaycastHit hitInfo;

    //필요 컴포넌트
    [SerializeField]
    private Camera theCam;


    [SerializeField]
    private GameObject grenade;
    [SerializeField]
    private GameObject gL;
    private Vector3 target;
    private NoBulletSound noBulletSound;
    private void Start()
    {
        originPos = Vector3.zero;
        audio = GetComponent<AudioSource>();
        noBulletSound = FindObjectOfType<NoBulletSound>();

        WeaponManager.currentWeapon = currentRifle;
        WeaponManager.currentWeaponAnimator = currentRifle.animator;
    }
    private void Update()
    {
        TargetChange();
        Fire();
        TryReload();
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
                    StartCoroutine(Reload());
                }
            }
        }
    }
    //타겟 체인지
    private void TargetChange()
    {
        target = gL.transform.rotation * Vector3.forward * 50;
    }
    //발사
    private void Shoot()
    {
        currentRifle.currentBulletCount--;
        GameObject bullet = Instantiate(grenade, gL.transform.position, gL.transform.rotation);
        Rigidbody rig = bullet.GetComponent<Rigidbody>();
        rig.AddForce(target, ForceMode.Impulse);
        Destroy(bullet, 1.1f);
        currentRifle.muzzleFlash.Play();
        audio.Play();
        player.animator.SetTrigger("Shoot");
        //총기 반동 코루틴
        StopAllCoroutines();
        StartCoroutine(RetroactionCoroutine());
    }

    IEnumerator RetroactionCoroutine()
    {
        Vector3 recoilBack = new Vector3(originPos.x, originPos.y, -currentRifle.resistForce);
        Vector3 aimRecoilBack = new Vector3(currentRifle.getAimOriginPos().x, currentRifle.getAimOriginPos().y, -currentRifle.resistAimForce);

        currentRifle.transform.localPosition = originPos;

        //반동
        while (currentRifle.transform.localPosition.z > -currentRifle.resistForce + 0.02f)
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, recoilBack, 0.4f);
            yield return null;
        }
        //원위치
        while (currentRifle.transform.localPosition.z < originPos.z - 0.02f)
        {
            currentRifle.transform.localPosition = Vector3.Lerp(currentRifle.transform.localPosition, originPos, 0.1f);
            yield return null;
        }


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

    public Rifle GetRifle()
    {
        return this.currentRifle;
    }


    public void GunChange(Rifle gun)
    {
        if (WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.transform.parent.gameObject.SetActive(false);
        }
        currentRifle = gun;
        WeaponManager.currentWeapon = currentRifle;
        WeaponManager.currentWeaponAnimator = currentRifle.animator;
        reloadSound.ReloadSoundStop();
        currentRifle.transform.localPosition = Vector3.zero;
        currentRifle.gameObject.transform.parent.gameObject.SetActive(true);
    }
}
