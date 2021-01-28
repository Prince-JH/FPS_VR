using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class PlayerMove : MonoBehaviour
{
    //VR Equip
    public SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    public SteamVR_Input_Sources any = SteamVR_Input_Sources.Any;

    public SteamVR_Action_Boolean trigger;
    public SteamVR_Action_Boolean grip = SteamVR_Input.GetBooleanAction("GrabGrip");
    public SteamVR_Action_Boolean trackPadClick;
    public SteamVR_Action_Vector2 trackPadPosition;
    private Gun currentRifle;
    //플레이어 이동 관련 변수
    
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    private float jumpForce = 6.5f;
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
    /*
    //카메라
    private float lookSensitivity = 1.5f;
    private float cameraRotationLimit = 60;
    private float currentCameraRotationX = 0;
    */
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
    private Camera weaponCam;
    [SerializeField]
    private Image gameOver;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject laser;
    // Start is called before the first frame update
    void Start()
    {
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
            VRMove();
            Run();
            Jump();
            //RotateLR();
            //RotateUD();
            PlayerSight();
            KilledInAction();
        }
    }
    //플레이어 VR이동
    private void VRMove()
    {
        Vector2 pos;
        float moveX = 0;
        float moveZ = 0;
        if (trackPadClick.GetState(leftHand))
        {
            pos = trackPadPosition.GetAxis(leftHand);
            moveX = pos.x;
            moveZ = pos.y;

            
        }
        else if(trackPadClick.GetStateUp(leftHand))
        {
            moveX = 0;
            moveZ = 0;
        }
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
        animator.SetBool("Walk", isWalk);
        currentRifle.animator.SetBool("Walk", isWalk);
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
        if (grip.GetStateDown(leftHand) && isWalk)
        {
            isWalk = false;
            isRun = true;
            speed = runSpeed;
        }
        else if (grip.GetStateUp(leftHand))
        {
            isRun = false;
            speed = walkSpeed;
        }
    }
    /*
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
    */
    //바닥에 있는지 확인
    private void IsGround()
    { 
        isGround = Physics.Raycast(transform.position, Vector3.down,out RaycastHit hitinfo, capsuleCollider.bounds.extents.y - 1f);
        //crosshair.JumpAnimation(!isGround);
    }
    //점프
    private void Jump()
    {
        if (trigger.GetStateDown(leftHand) && isGround)
        {
            isWalk = false;
            rig.velocity = transform.up * jumpForce;
        }
    }
    /*
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            isWalk = false;
            rig.velocity = transform.up * jumpForce;
        }
    }
    */
    //카메라 좌우 회전(캐릭터 회전)
    /*
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
    */
    private void PlayerSight()
    {
        gameObject.transform.localEulerAngles = new Vector3(0, cam.transform.localEulerAngles.y, 0); 
    }
    private void KilledInAction()
    {
        if (healthPoint <= 0 && deadSound.canPlay)
        {
            GameManager.isPlay = false;
            animator.SetTrigger("Dead");
            deadSound.DeadSoundPlay();
            string[] layer = new string[] { "Ignore Raycast" };
            weaponCam.cullingMask = LayerMask.GetMask(layer);
            isDead = true;
            rig.isKinematic = true;
            gameOver.gameObject.SetActive(true);
            laser.SetActive(true);
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
