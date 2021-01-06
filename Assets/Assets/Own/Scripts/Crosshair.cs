using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private float rifleAccuracy;

    //크로스헤어 활성화, 비활성화
    [SerializeField]
    private GameObject crosshairHUD;
    

    public void WalkAnimation(bool flag)
    {
        animator.SetBool("Walk", flag);
    }
    public void RunAnimation(bool flag)
    {
        animator.SetBool("Run", flag);
    }
    public void JumpAnimation(bool flag)
    {
        animator.SetBool("Jump", !flag);
    }
}
