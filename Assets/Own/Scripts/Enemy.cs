using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //정찰
    private NavMeshAgent enemy = null;
    [SerializeField]
    private Transform[] wayPoints = null;
    private int count = 0;

    //탐지
    [SerializeField]
    private Transform enemyBody;
    private float observeRange = 30f;
    [SerializeField]
    private LayerMask layerMask;
    private bool isSearch = false;
    private bool isWalk = false;
    private bool isShoot = true;
    private Transform target = null;
    private float fireRate = 0.5f;
    [SerializeField]
    private ParticleSystem muzzleFlash;
    [SerializeField]
    private GameObject bullet;
    private Transform bulletPos;
    [SerializeField]
    private GameObject gunSound;
    [SerializeField]
    private GameObject potionRed;
    [SerializeField]
    private GameObject potionYellow;

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
        enemy = GetComponent<NavMeshAgent>();
        bulletPos = muzzleFlash.transform;

        InvokeRepeating("SearchEnemy", 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        Die();
        EnemyAttack();
        MoveToNextWayPoint();
        WalkAnimation();
    }
    private void Die()
    {
        if (hp <= 0 && logFlag)
            StartCoroutine(Dead());
    }
    IEnumerator Dead()
    {
        enemy.enabled = false;
        logFlag = false;
        if (hp <= 0)
            killLog.Show();
        if (hp <= -20)
            animator.SetTrigger("Explode");
        else
            animator.SetTrigger("Dead");
        DropPotion();
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        logFlag = true;
    }
    private void DropPotion()
    {
        int potionRedNum = 3;
        int potionYellowNum = 4;
        int num = Random.Range(0, 5);
        if(num == potionRedNum)
        {
            Instantiate(potionRed, transform.position, transform.rotation);
        }
        else if(num == potionYellowNum)
        {
            Instantiate(potionYellow, transform.position, transform.rotation);
        }

    }
    private void SearchEnemy()
    {
        if (PlayerMove.healthPoint <= 0)
            return;
        //주변의 collider 검출
        Collider[] cols = Physics.OverlapSphere(transform.position, observeRange, layerMask);

        if (cols.Length > 0)
        {
            Transform targetTemp = cols[0].transform;
            
            Vector3 targetDirection = (targetTemp.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, targetDirection, out RaycastHit targetHit, observeRange))
            {
                if (targetHit.transform.tag == "Player")
                {
                    isSearch = true;
                    target = targetTemp;
                }
            }
        }
    }
    private void MoveToNextWayPoint()
    {
        if (enemy.velocity == Vector3.zero && enemy.enabled && !isSearch)
        {
            enemy.SetDestination(wayPoints[count++].position);

            if (count >= wayPoints.Length)
                count = 0;
        }
    }
    private void WalkAnimation()
    {
        if (enemy.velocity != Vector3.zero && enemy.enabled)
        {
            isWalk = true;
            animator.SetBool("Walk", isWalk);
        }
        else
        {
            isWalk = false;
            animator.SetBool("Walk", false);
        }
    }
    private void EnemyAttack()
    {
        if (target != null)
        {
            if (isSearch && enemy.enabled)
            {
                if (Vector3.Distance(enemyBody.position, target.position) >= observeRange)
                {
                    target = null;
                    isSearch = false;
                }
                else if (Vector3.Distance(enemyBody.position, target.position) <= observeRange)
                {
                    enemyBody.LookAt(target);
                    enemy.velocity = Vector3.zero;
                    animator.SetTrigger("Shoot");
                    StartCoroutine(EnemyShoot());
                }
                else if (isWalk)
                    enemy.SetDestination(target.position);
            }
        }
        if (PlayerMove.healthPoint <= 0)
            target = null;
    }
    IEnumerator EnemyShoot()
    {
        if (isShoot)
        {
            muzzleFlash.Play();
            GameObject bulletClone = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
            Destroy(bulletClone, 2.0f);
            isShoot = false;
            gunSound.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(fireRate);
            isShoot = true;
        }
    }

}
