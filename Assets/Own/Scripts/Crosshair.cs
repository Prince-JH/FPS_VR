using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private float rifleAccuracy;

    //크로스헤어 활성화, 비활성화
    public GameObject crosshairAR;


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
        animator.SetBool("Jump", flag);
    }
    public void AimAnimation(bool flag)
    {
        animator.SetBool("Aim", flag);
    }
    public void FireAnimation()
    {
        if (animator.GetBool("Walk"))
            animator.SetTrigger("WalkFire");
        else if (animator.GetBool("Run"))
            animator.SetTrigger("RunFire");
        else if (animator.GetBool("Jump"))
            animator.SetTrigger("JumpFire");
        else
            animator.SetTrigger("IdleFire");
    }
}
