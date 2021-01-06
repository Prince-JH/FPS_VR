using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    //필요 컴포넌트
    [SerializeField]
    private RifleControl rifleControl;
    private Rifle currentRifle;

    //HUD 활성화, 비활성화
    [SerializeField]
    private GameObject bulletHUD;

    [SerializeField]
    private Text[] textBullet;
    

    // Update is called once per frame
    void Update()
    {
        CountBullet();
    }
    private void CountBullet()
    {
        currentRifle = rifleControl.GetRifle();
        textBullet[0].text = currentRifle.currentBulletCount.ToString();
        textBullet[1].text = currentRifle.reloadBulletCount.ToString();
        textBullet[2].text = currentRifle.carryBulletCount.ToString();
    }
}
