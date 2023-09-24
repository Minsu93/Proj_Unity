using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //�켱 �÷��̾�� �ڽŰ� ���� ����� �༺�� ��ġ�� �߽����� ��ƾ� �Ѵ�.
    //�༺�� �߷� ������ �޴´ٸ�, �װ��� �ڽ��� �ٴ��� �Ǵ� ��. 
    //�ٴ��� ã�´ٸ� ĳ���Ͱ� �ٴ��� ���� ȸ���Ѵ�. 

    //ĳ���ʹ� �ڽ��� rotation �� Right �������� �����Ѵ�. 
    //������ ����, ������ �����̴�. 

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



        //�ٴ����� �ν��� ���̾�
        floorLayerMask = 1 << LayerMask.NameToLayer("Planet")  | 1 << LayerMask.NameToLayer("Obstacle");
        checkGround = true;
    }



    private void Update()
    {

        //��,�� �Է°� �޾ƿ�
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


        //�Է°��� ���� model �� flip��Ŵ
        FlipModel();

        //ĳ���� ȸ��
        RotateCharacterToGround();

        //���߿� �ִ��� üũ
        CheckOnAir();


        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && !OnAir)
        {
            StartCoroutine(StopCheckGround());

            //���󿡼� �������� ��. OnAir �� false�� ���¿��� ������.
            OnAir = true;

            Vector2 upVector = (Vector2)transform.position - gravityFinder.nearestPoint;
            Vector2 jumpDir = (upVector + ((Vector2)transform.right * onRight * 0.2f * Convert.ToInt32(onMove))).normalized;
            Vector2 jumpVector = jumpDir * jumpForce;
            rb.velocity = Vector2.zero;
            rb.AddForce(jumpVector, ForceMode2D.Impulse);

            //���� ���� ���
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);

        }

    }



    private void FixedUpdate()
    {

        if (moveDir.x != moveDirBefore.x || moveDir.y != moveDirBefore.y)
        {
            // ������ �ٲ�! ���� �� ��, �ٸ� ����Ű�� ������ ��.
            moveDirBefore = moveDir;
            onRight = Vector2.SignedAngle(transform.up, moveDir) < 0 ? 1 : -1;

        }

        //�̵� - �÷��̾ ���߿� ���� �ʰ�, moveDir �Է°��� ���� ��, �� ��� �����δ�. 
        if (onMove)
        {
            if (!OnAir)                //���� �̵�
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

            else                 //���� �̵�
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

        else //������ �Է��� ���� ��
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
        //������ �� ��� OnAirüũ�� ����
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

        //ĳ������ �߽ɿ���, �� ������ ���̸� ���� ĳ���Ͱ� ���߿� �� �ִ��� �˻��Ѵ�.
        //RaycastHit2D footHit = Physics2D.Raycast(transform.position, transform.up * -1, 0.5f, floorLayerMask);
        RaycastHit2D footHit = Physics2D.CircleCast(transform.position, 0.25f, transform.up * -1, 1f, floorLayerMask);
        if (footHit.collider != null)
        {
            if (footHit.distance < 0.2f)
            {
                OnAir = false;

                //�ȴ� ���� �ƴϰ�, ���߿� �� ���� �ʴٸ�
                //if (!OnAir && !isWalking)
                //{
                    //rb.bodyType = RigidbodyType2D.Kinematic;
                    //rb.velocity = Vector2.zero;
                //}
            }
            else
            {
                //�ٴڰ��� �Ÿ��� 0.25 �̻�
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
