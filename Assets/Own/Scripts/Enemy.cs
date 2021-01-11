using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector]
    public int hp = 3;
    private KillLog killLog;
    private Animator animator;
    private bool logFlag = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        killLog = FindObjectOfType<KillLog>();
    }

    // Update is called once per frame
    void Update()
    {
        Die();
    }
    private void Die()
    {
        if (hp <= 0 && logFlag)
            StartCoroutine(Dead());
    }
    IEnumerator Dead()
    {
        logFlag = false;
        if (hp <= 0) 
        killLog.Show();
        if (hp <= -20)
            animator.SetTrigger("Explode");
        else
            animator.SetTrigger("Dead");
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        logFlag = true;
    }

}
