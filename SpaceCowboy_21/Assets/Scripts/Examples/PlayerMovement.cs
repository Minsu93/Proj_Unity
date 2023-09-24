using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //우선 플레이어는 자신과 가장 가까운 행성의 위치를 중심으로 잡아야 한다.
    //행성의 중력 영향을 받는다면, 그곳이 자신의 바닥이 되는 것. 
    //바닥을 찾는다면 캐릭터가 바닥을 향해 회전한다. 

    //캐릭터는 자신의 rotation 의 Right 방향으로 전진한다. 
    //조작은 전진, 후진과 점프이다. 

    public List<Transform> influentialPlanets = new List<Transform>();

    public float moveSpeed;
    public float airMoveSpeed;
    public float maxSpeed = 0.7f;
    public float maxAirSpeed = 3f;
    public float turnSpeedOnLand = 100f;
    public bool OnAir;

    [Header("Jump")]
    public float jumpForce = 10f;


    public ParticleSystem runningParticle;
    //public GameObject moveDirArrow;


    private bool checkGround;
    private bool isWalking = false;
    private Vector2 moveDir;
    private int floorLayerMask;
    private int onRight;
    bool onMove = false;
    
    Vector2 lastMovePos = Vector2.zero;
    Vector2 moveDirBefore = Vector2.right;

    Rigidbody2D rb;
    //Animator animator;
    Transform model;
    GravityFinder gravityFinder;
   

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponentInChildren<Animator>();
        model = GetComponentsInChildren<Transform>()[1];
        gravityFinder = GetComponent<GravityFinder>();
        moveDir = Vector2.up;
        OnAir = true;



        //바닥으로 인식할 레이어
        floorLayerMask = 1 << LayerMask.NameToLayer("Planet")  | 1 << LayerMask.NameToLayer("Obstacle");
        checkGround = true;
    }



    private void Update()
    {

        //좌,우 입력값 받아옴
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.y = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs( Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f)
        {
            onMove = true;

            //moveDirArrow.SetActive(true);
            //moveDirArrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, moveDir);
        }
        else
        {
            onMove = false;
            //moveDirArrow.SetActive(false);
        }


        //입력값에 따라 model 을 flip시킴
        FlipModel();

        //캐릭터 회전
        RotateCharacterToGround();

        //공중에 있는지 체크
        CheckOnAir();


        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && !OnAir)
        {
            StartCoroutine(StopCheckGround());

            //지상에서 점프했을 때. OnAir 가 false인 상태에서 점프함.
            OnAir = true;

            Vector2 upVector = (Vector2)transform.position - gravityFinder.nearestPoint;
            Vector2 jumpDir = (upVector + ((Vector2)transform.right * onRight * 0.2f * Convert.ToInt32(onMove))).normalized;
            Vector2 jumpVector = jumpDir * jumpForce;
            rb.velocity = Vector2.zero;
            rb.AddForce(jumpVector, ForceMode2D.Impulse);

            //점프 사운드 출력
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);

        }

    }



    private void FixedUpdate()
    {

        if (moveDir.x != moveDirBefore.x || moveDir.y != moveDirBefore.y)
        {
            // 방향이 바뀜! 손을 뗄 때, 다른 방향키를 눌렀을 때.
            moveDirBefore = moveDir;
            onRight = Vector2.SignedAngle(transform.up, moveDir) < 0 ? 1 : -1;

        }

        //이동 - 플레이어가 공중에 있지 않고, moveDir 입력값이 있을 때, 좌 우로 움직인다. 
        if (onMove)
        {
            if (!OnAir)                //지상 이동
            {
                if (!isWalking)
                {
                    isWalking = true;
                    runningParticle.Play();
                    //animator.SetBool("IsMove", true);
                }

                Vector2 movePos = transform.right * onRight * moveSpeed * 1000f ;
                rb.AddForce(movePos * Time.deltaTime);

                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
                }
            }

            else                 //공중 이동
            {
                if (isWalking)
                {
                    isWalking = false;
                    runningParticle.Stop();
                    //animator.SetBool("IsMove", false);
                }

                //onRight = Vector2.SignedAngle(transform.up, moveDir) < 0 ? 1 : -1;
                Vector2 addVel = moveDir.normalized * airMoveSpeed * Time.deltaTime;

                if (rb.velocity.magnitude < maxAirSpeed)
                {
                   // rb.velocity += addVel;
                }

            }

        }

        else //움직임 입력이 없을 때
        {
            if (isWalking)
            {
                isWalking = false;
                runningParticle.Stop();
                //animator.SetBool("IsMove", false);
            }
        }


    }


    IEnumerator StopCheckGround()
    {
        //점프할 때 잠시 OnAir체크를 끈다
        checkGround = false;
        yield return new WaitForSeconds(0.1f);
        checkGround = true;
    }


    void RotateCharacterToGround()
    {
        if (gravityFinder.fixGravityOn)
        {
            RotateToVector(gravityFinder.fixGravityDir * -1f, turnSpeedOnLand);
            return;
        }


        if (gravityFinder.nearestGround == null)
            return;

        Vector2 upVec = ((Vector2)transform.position - gravityFinder.nearestPoint).normalized;
        RotateToVector(upVec, turnSpeedOnLand);


    }

    void RotateToVector(Vector2 normal, float turnSpeed)
    {
        Vector3 vectorToTarget = normal;
        //Vector3 rotatedVectorToTarget = vectorToTarget;

        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

    }

    void CheckOnAir()
    {
        if (!checkGround)
            return;

        //캐릭터의 중심에서, 발 밑으로 레이를 쏴서 캐릭터가 공중에 떠 있는지 검사한다.
        //RaycastHit2D footHit = Physics2D.Raycast(transform.position, transform.up * -1, 0.5f, floorLayerMask);
        RaycastHit2D footHit = Physics2D.CircleCast(transform.position, 0.25f, transform.up * -1, 1f, floorLayerMask);
        if (footHit.collider != null)
        {
            if (footHit.distance < 0.2f)
            {
                OnAir = false;

                //걷는 중이 아니고, 공중에 떠 있지 않다면
                //if (!OnAir && !isWalking)
                //{
                    //rb.bodyType = RigidbodyType2D.Kinematic;
                    //rb.velocity = Vector2.zero;
                //}
            }
            else
            {
                //바닥과의 거리가 0.25 이상
                if (!OnAir)
                {
                    //rb.bodyType = RigidbodyType2D.Dynamic;
                    runningParticle.Stop();
                    //animator.SetBool("IsMove", false);
                }
                OnAir = true;

            }
        }
    }
    
    


    void FlipModel()
    {
        Vector2 upVector = transform.up;
        int flipRight = Vector2.SignedAngle(upVector, moveDir) < 0 ? 1 : -1;

        model.localScale = new Vector3(flipRight, 1, 1);

    }


    

}
