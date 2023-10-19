using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GravityLasso : MonoBehaviour
{
    Vector2 lassoTip;   //올가미 위치
    bool lassoActivate;  // 올가미가 발사되었나요
    bool lassoBack;  //올가미가 돌아오나요
    bool inLassoMovement; //올가미로 움직이는 중인가요

    [Header("Lasso Property")]
    public float lassoSize = 0.3f;
    public float lassoSpeed = 1f;   //올가미 속도
    public float moveTime = 2f;     //올가미 이동 
    public float turnTime = 1f;
    float timer;
    Vector2 targetDir;  //목표 지점
    Vector2 preLassoTip;

    [Header("BigTrigger")]
    Vector2 targetPos;
    Vector2 startPlayerPos;
    bool bigTrigger;
    public float playerMoveTime = 1f;
    public float lassoForce = 30f;

    [Header("MiddleTirrger")]
    bool middleTrigger;
    public float bothMoveTime = 0.3f;
    Transform other;

    LineRenderer lineRenderer;
    GameObject lineObj;
    CircleCollider2D lineColl;

    Rigidbody2D rb;
    PlayerGravity playerGravity;
    

    private void Awake()
    {
        //올가미 오브젝트 생성
        lineObj = new GameObject();
        lineObj.transform.parent = transform;
        lineColl = lineObj.AddComponent<CircleCollider2D>();
        lineColl.radius = lassoSize;
        lineColl.isTrigger = true;
        lineColl.enabled = false;
        Rigidbody2D lineRb = lineObj.AddComponent<Rigidbody2D>();
        lineRb.isKinematic = true;
        GravityLassoChecker checker =  lineObj.AddComponent<GravityLassoChecker>();
        checker.lasso = this;
        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;

        lineObj.SetActive(false);


        rb = GetComponent<Rigidbody2D>();
        playerGravity = GetComponent<PlayerGravity>();
    }

    // Update is called once per frame
    void Update()
    {

        /// 중력 올가미는 매 업데이트마다 뭔가와 부딪히지 않았는지 체크한다. 
        /// 체크한 대상이 소형인 경우 가져온다
        /// 체크한 대상이 적인 경우 서로의 방향으로 Add Impulse
        /// 체크한 대상이 대형인 경우(Planet, Boss) 그 방향으로 이동한다. 
        /// . 
        /// 일단 마우스 우 클릭을 누르면 올가미를 발사하며
        /// 발사한 올가미는 쭉 날아가다가 일정 시간, 거리가 되면 돌아온다. 
        /// 


        if (lassoActivate)
        {
            if (!lassoBack)
            {
                //올가미 움직인다. 
                timer += Time.deltaTime;

                lassoTip += targetDir * lassoSpeed * Time.deltaTime;

                if (timer > moveTime)
                {
                    timer = 0;
                    lassoBack = true;
                    preLassoTip = lassoTip;
                    lineColl.enabled = false;
                }
            }
            else
            {
                //올가미는 거꾸로 돌아온다.
                timer += Time.deltaTime;

                //Vector2 dir = (lassoTip - (Vector2)transform.position);
                Vector2 pos = Vector2.Lerp(preLassoTip, (Vector2)transform.position, timer/turnTime);
                lassoTip = pos;

                if(timer > turnTime)
                {
                    lassoActivate = false;
                    lassoBack = false;
                    //lineColl.enabled = false;
                    lineObj.SetActive(false);
                }
            }

            UpdateLassoPosition();
        }
        
        if(!lassoActivate && !inLassoMovement)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                //Debug.Log("Launch Lasso");

                LassoThrow();
            }
        }
    }


    private void FixedUpdate()
    {
        if (inLassoMovement)
        {
            SwitchMovement();
            return;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!inLassoMovement)
            return;

        if (collision.collider.CompareTag("Planet"))
        {   

            bigTrigger = false;

            lassoActivate = false;
            inLassoMovement = false;

            lineObj.SetActive(false);

            rb.velocity = Vector2.zero;

        }
    }


    void LassoThrow()
    {
        lassoTip = transform.position;  //발사 시작 위치 초기화

        //올가미를 던질 때 
        lassoActivate = true;
        lineObj.SetActive(true); 
        UpdateLassoPosition();
        lineColl.enabled = true;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        targetDir = ((Vector2)mousePos - (Vector2)transform.position).normalized;

        timer = 0;
    }

    void UpdateLassoPosition()
    {
        //라인 렌더러를 업데이트한다.
        lineObj.transform.position = lassoTip;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, lassoTip);
  
    }





    void SwitchMovement()
    {
        if (bigTrigger)
        {
            //transform.position = Vector2.Lerp(startPlayerPos, targetPos, timer / playerMoveTime);
            //Vector2 pos = Vector2.Lerp(rb.position, targetPos, timer / playerMoveTime);
            /*
            Vector2 force = targetPos - rb.position;
            Vector2 forceDir = force.normalized;
            float forceDist = force.magnitude;
            Vector2 finalForce = forceDir * Mathf.Clamp(forceDist,3f, 12f);
            rb.AddForce(finalForce * 10f);
            */

            Vector2 force = targetPos - rb.position;
            Vector2 forceDir = force.normalized;
            float forceDist = force.magnitude;

            rb.AddForce(forceDir * lassoForce * forceDist * 0.5f);

            if (forceDist < 1f)
            {
                bigTrigger = false;

                lassoActivate = false;
                inLassoMovement = false;

                //playerGravity.activate = true;


                lineObj.SetActive(false);

            }

        }
        else if (middleTrigger)
        {
            timer += Time.deltaTime;

            lassoTip = other.position;
            UpdateLassoPosition();

            if(timer > bothMoveTime)
            {
                middleTrigger = false;

                lassoActivate = false;
                inLassoMovement = false;

                lineObj.SetActive(false);

            }
        }

    }




    public void TriggerByBig()
    {
        //올가미의 움직임을 정지한다.
        lassoActivate = false;
        lassoBack = false;

        //플레이어 중력을 정지시킨다
        //playerGravity.activate = false;

        //움직임을 시작한다.
        inLassoMovement = true;

        //더이상 충돌을 못하게 비활성화한다.
        lineColl.enabled = false;

        targetPos = lassoTip;
        startPlayerPos = transform.position;
        bigTrigger = true;
        timer = 0f;
    }

    public void TriggerByMedium(Collider2D other)
    {
        //올가미의 움직임을 정지한다.
        lassoActivate = false;

        //움직임을 시작한다.
        inLassoMovement = true;

        //더이상 충돌을 못하게 비활성화한다.
        lineColl.enabled = false;
        middleTrigger = true;
        timer = 0f;

        this.other = other.transform;


        //플레이어를 점프시킨다.(AddImpulse) 원래는 PlayerBehavior 에 해야함
        Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 _dir = (other.transform.position - transform.position).normalized;
            playerRb.AddForce(_dir * 10f, ForceMode2D.Impulse);
        }

        //적에게 Impulse를 준다. 
        Rigidbody2D targetRb = other.GetComponent<Rigidbody2D>();
        if (targetRb != null)
        {
            Vector3 dir = (transform.position - other.transform.position).normalized;
            targetRb.AddForce(dir * 10f, ForceMode2D.Impulse);
        }

    }

    public void TriggerBySmall()
    {

    }
}
