using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionYellow : MonoBehaviour
{
    private PotionSound potionSound;
    private void Start()
    {
        potionSound = FindObjectOfType<PotionSound>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            if(WeaponManager.currentWeapon.name == "GrenadeLauncher")
                WeaponManager.currentWeapon.GetComponent<GL>().carryBulletCount++;
            else
                WeaponManager.currentWeapon.GetComponent<Rifle>().carryBulletCount += 60;
            
            potionSound.SoundPlay();
            Destroy(gameObject);
        }
    }
}
