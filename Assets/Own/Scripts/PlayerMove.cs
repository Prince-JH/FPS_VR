using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    private Gun currentRifle;
    //플레이어 이동 관련 변수
    
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float jumpForce = 8.5f;
    private float speed;
    private Vector3 velocity;

    //체력
    [HideInInspector]
    public static int healthPoint = 100;

    //상태 변수
    private bool isWalk;
    private bool isRun;
    private bool isGround = true;
    private bool isDead;

    //카메라
    private float lookSensitivity = 1.5f;
    private float cameraRotationLimit = 60;
    private float currentCameraRotationX = 0;

    //컴포넌트 
    [SerializeField]
    private Camera cam;
    private Rigidbody rig;
    private CapsuleCollider capsuleCollider;
    [HideInInspector]
    public Animator animator;
    private Crosshair crosshair;
    [SerializeField]
    private DeadSound deadSound;
    [SerializeField]
    private Camera deadCam;
    [SerializeField]
    private Image gameOver;
    [SerializeField]
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
        Cursor.visible = false;
        rig = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        crosshair = FindObjectOfType<Crosshair>();
        //초기화
        speed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.isPlay && !isDead)
        {
            currentRifle = WeaponManager.currentWeapon;
            IsGround();
            Move();
            Run();
            Jump();
            RotateLR();
            RotateUD();
            KilledInAction();
        }
    }
    //플레이어 이동
    private void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveX;
        Vector3 moveVertical = transform.forward * moveZ;

        velocity = (moveHorizontal + moveVertical).normalized * speed;
        if (velocity.magnitude <= 0.01f)
            isWalk = false;
        else if (!isRun)
            isWalk = true;
        rig.MovePosition(transform.position + velocity * Time.deltaTime);
        animator.SetBool("Run", isRun);
        currentRifle.animator.SetBool("Run", isRun);
        crosshair.RunAnimation(isRun);
        animator.SetBool("Walk", isWalk);
        currentRifle.animator.SetBool("Walk", isWalk);
        crosshair.WalkAnimation(isWalk);
    }
    //달리기
    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isWalk)
        {
            isWalk = false;
            isRun = true;
            speed = runSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRun = false;
            speed = walkSpeed;
        }
    }
    //바닥에 있는지 확인
    private void IsGround()
    { 
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y);
        crosshair.JumpAnimation(!isGround);
    }
    //점프
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            isWalk = false;
            rig.velocity = transform.up * jumpForce;
        }
    }
    //카메라 좌우 회전(캐릭터 회전)
    private void RotateLR()
    {
        if(!isDead && !GameManager.isPause)
        {
            float yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 rotateLR = new Vector3(0f, yRotation, 0f) * lookSensitivity;
            rig.MoveRotation(rig.rotation * Quaternion.Euler(rotateLR));
        }
    }
    //카메라 위 아래 회전(카메라 회전)
    private void RotateUD()
    {
        if(!isDead && !GameManager.isPause)
        {
            float xRotation = Input.GetAxisRaw("Mouse Y");
            float rotateUD = xRotation * lookSensitivity;
            currentCameraRotationX -= rotateUD;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0, 0);
        }
    }
    private void KilledInAction()
    {
        if (healthPoint <= 0 && deadSound.canPlay)
        {
            animator.SetTrigger("Dead");
            deadSound.DeadSoundPlay();
            cam.gameObject.SetActive(false);
            deadCam.gameObject.SetActive(true);
            isDead = true;
            rig.isKinematic = true;
            gameOver.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;//마우스 커서 고정 해제
            Cursor.visible = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.tag == "EnemyBullet")
        {
            OnDamaged();
        }
    }
    private void OnDamaged()
    {
        player.gameObject.layer = 13;
        PlayerMove.healthPoint -= 10;
        Invoke("OffDamaged", 1);
    }
    private void OffDamaged()
    {
        player.gameObject.layer = 12;
    }
}
