using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionRed : MonoBehaviour
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
            if (PlayerMove.healthPoint <= 70)
                PlayerMove.healthPoint += 30;
            else
                PlayerMove.healthPoint = 100;
            potionSound.SoundPlay();
            Destroy(gameObject);
        }
    }
}
