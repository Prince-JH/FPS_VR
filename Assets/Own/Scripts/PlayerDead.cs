using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDead : MonoBehaviour
{
    private Animator animaotr;
    [SerializeField]
    private DeadSound deadSound;
    void Start()
    {
        animaotr = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(PlayerMove.healthPoint);
        if (PlayerMove.healthPoint <= 0 && deadSound.canPlay)
        {
            animaotr.SetBool("Dead", true);
            deadSound.DeadSoundPlay();
        }
    }
}
