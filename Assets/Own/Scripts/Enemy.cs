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
    private float observeRange = 45f;
    [SerializeField]
    private LayerMask layerMask;
    private bool isSearch = false;
    private bool isWalk = false;
    private bool isShoot = true;
    private bool isAttack = false;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isPlay)
        {
            Die();
            EnemyAttack();
            EnemyMove();
            WalkAnimation();
        }
        else
        {
            enemy.velocity = Vector3.zero;
        }
    }
    private void Die()
    {
        if (hp <= 0 && logFlag)
            StartCoroutine(Dead());
    }
    IEnumerator Dead()
    {
        GameManager.kill++;
        enemy.enabled = false;
        logFlag = false;
        if (hp <= 0)
            killLog.Show();
        if (hp <= -20)
            animator.SetTrigger("Explode");
        else if (hp <= -10)
            animator.SetTrigger("HeadShot");
        else
            animator.SetTrigger("Dead");
        DropPotion();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
        GameManager.enemyCount--;
        logFlag = true;
    }
    private void DropPotion()
    {
        int potionRedNum = 3;
        int potionYellowNum = 4;
        int num = Random.Range(0, 5);
        if (num == potionRedNum)
        {
            Instantiate(potionRed, transform.position, transform.rotation);
        }
        else if (num == potionYellowNum)
        {
            Instantiate(potionYellow, transform.position, transform.rotation);
        }

    }
    private void EnemyMove()
    {
        if (enemy.velocity == Vector3.zero && enemy.enabled && target == null)
            MoveToNextWayPoint();
        else if (enemy.velocity == Vector3.zero && enemy.enabled  &&  !isSearch && !isAttack)
            MoveToNextWayPoint();
    }
    private void MoveToNextWayPoint()
    {
        enemy.SetDestination(wayPoints[count++].position);

        if (count >= wayPoints.Length)
            count = 0;

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
                isAttack = true;
                enemyBody.LookAt(target);
                enemy.velocity = Vector3.zero;
                animator.SetTrigger("Shoot");
                StartCoroutine(EnemyShoot());
                /*
                if (Vector3.Distance(enemyBody.position, target.position) >= observeRange)
                {
                    enemy.SetDestination(target.transform.position);
                    
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
            */
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
            Destroy(bulletClone, 2.5f);
            isShoot = false;
            gunSound.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(fireRate);
            isShoot = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            target = other.transform;
            if (PlayerMove.healthPoint == 0)
                target = null;
            TargetVisible();
        }
    }
    private void TargetVisible()
    {
        if (target != null && enemy.enabled)
        {
            Vector3 targetDirection = (target.position - transform.position).normalized;
            if (Physics.Raycast(transform.position + new Vector3(0, 2, 0), targetDirection, out RaycastHit rayHit, observeRange))
            {

                Debug.DrawRay(transform.position + new Vector3(0, 2, 0), targetDirection * 45f, Color.green);
                if (rayHit.transform.tag != "Player")
                {
                    isSearch = false;
                    if(enemy.velocity == Vector3.zero)
                        enemy.SetDestination(target.position);
                }
                else
                {
                    isAttack = false;
                    isSearch = true;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player" && target != null && enemy.enabled)
        {
            isSearch = false;
            enemy.SetDestination(target.position);
        }
    }

}
