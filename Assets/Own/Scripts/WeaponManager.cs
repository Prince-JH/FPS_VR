using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //무기 교체 중복 실행 방지
    public static bool isChangeWeapon;
    //현재 무기, 애니메이션
    public static Gun currentWeapon;
    public static Animator currentWeaponAnimator;

    //크로스헤어
    [SerializeField]
    private Crosshair crosshairAR;
    [SerializeField]
    private CrosshairGL crosshairGL;

    private float changeWeaponDelayTime = 0.1f;
    private float changeWeaponTime = 0.1f;
    [SerializeField]
    private Rifle[] guns;


    //무기 관리
    private Dictionary<string, Rifle> rifleDic = new Dictionary<string, Rifle>();

    //필요 컴포넌트
    [SerializeField]
    private RifleControl rifleControl;
    [SerializeField]
    private GLControl gLControl;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            rifleDic.Add(guns[i].name, guns[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        WeaponSwap();
    }
    private void WeaponSwap()
    {
        if (!GameManager.isPause && !RifleControl.rifleFire && GameManager.isPlay)
        {
            if (!isChangeWeapon)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) && currentWeapon.name == "GrenadeLauncher")
                {
                    StartCoroutine(ChangeWeaponCoroutine("AssaultRifle"));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && currentWeapon.name == "AssaultRifle")
                {
                    StartCoroutine(ChangeWeaponCoroutine("GrenadeLauncher"));
                }
            }
            if (currentWeapon.name == "AssaultRifle")
            {
                crosshairAR.crosshairAR.SetActive(true);
                crosshairGL.crosshairGL.SetActive(false);
            }
            else if (currentWeapon.name == "GrenadeLauncher")
            {
                crosshairAR.crosshairAR.SetActive(false);
                crosshairGL.crosshairGL.SetActive(true);
            }
        }
    }
    public IEnumerator ChangeWeaponCoroutine(string name)
    {
        isChangeWeapon = true;
        currentWeaponAnimator.SetTrigger("WeaponIn");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreAction();
        StartCoroutine(WeaponChange(name));

        yield return new WaitForSeconds(changeWeaponTime);
        isChangeWeapon = false;
    }
    private void CancelPreAction()
    {

        switch (currentWeapon.name)
        {
            case "AssaultRifle":
                rifleControl.CancelAim();
                rifleControl.CancelReload();
                break;
            case "GrenadeLauncher":
                gLControl.CancelReload();
                break;
        }


    }
    IEnumerator WeaponChange(string name)
    {
        while (!rifleControl.zoomOutComplete)
            yield return null;
        rifleControl.GunChange(rifleDic[name]);
    }
}
