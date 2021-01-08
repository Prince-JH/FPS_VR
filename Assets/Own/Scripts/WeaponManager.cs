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

    private float changeWeaponDelayTime = 0.1f;
    private float changeWeaponTime = 0.1f;
    private int weaponNum = 1;
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
        if (!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                weaponNum = 1;
                StartCoroutine(ChangeWeaponCoroutine("AssaultRifle"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                weaponNum = 2;
                StartCoroutine(ChangeWeaponCoroutine("GrenadeLauncher"));
            }
        }
    }

    public IEnumerator ChangeWeaponCoroutine(string name)
    {
        isChangeWeapon = true;
        currentWeaponAnimator.SetTrigger("WeaponIn");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreAction();
        WeaponChange(name);

        yield return new WaitForSeconds(changeWeaponTime);
        isChangeWeapon = false;
    }
    private void CancelPreAction()
    {
        
        switch (weaponNum)
        {
            case 1:
                rifleControl.CancelAim();
                rifleControl.CancelReload();
                break;
            case 2:
                gLControl.CancelReload();
                break;
        }
        
        
    }
    private void WeaponChange(string name)
    {
        rifleControl.GunChange(rifleDic[name]);
    }
}
