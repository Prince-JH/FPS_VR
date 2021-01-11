using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillLog : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Show()
    {
        animator.SetTrigger("Kill");
    }
}
