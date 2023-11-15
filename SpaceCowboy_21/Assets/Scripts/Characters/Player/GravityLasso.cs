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
    public float lassoRange = 3f;   //올가미 사정거리
    public float lassoSpeed = 1f;   //올가미 속도
    public float moveTime = 2f;     //올가미 이동 
    public float turnTime = 1f;
    float timer;
    Vector2 targetDir;  //목표 지점
    Vector2 preLassoTip;
    public Sprite tipSprite;
    public Material lassoMaterial;

    [Header("BigTrigger")]
    public float bigTForce = 5f;
    float bigTMoveTime;

    Vector2 targetPos;
    Vector2 startPlayerPos;
    bool bigTrigger;

    [SerializeField]
    private AnimationCurve moveCurve;
    Vector2 startVelocity;

    [Header("MiddleTirrger")]
    public float normalForce = 1f;
    public float impactForce = 2f;
    bool impactAttack;
    public float impactTime = 0.3f;
    float impactTimer;
    Vector2 startEnemyPos;
    Vector2 enemyVelocity;
    Transform target;
    Collider2D targetColl;
    Rigidbody2D targetRb;

    LineRenderer lineRenderer;
    GameObject lineObj;
    CircleCollider2D lineColl;
    GravityLassoChecker checker;

    Rigidbody2D rb;
    

    private void Awake()
    {
        //올가미 오브젝트 생성
        lineObj = new GameObject();
        //lineObj.transform.parent = transform;

        lineColl = lineObj.AddComponent<CircleCollider2D>();
        lineColl.radius = lassoSize;
        lineColl.isTrigger = true;
        lineColl.enabled = false;

        Rigidbody2D lineRb = lineObj.AddComponent<Rigidbody2D>();
        lineRb.isKinematic = true;

        checker =  lineObj.AddComponent<GravityLassoChecker>();
        checker.lasso = this;

        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = .2f;
        lineRenderer.endWidth = .2f;
        lineRenderer.material = lassoMaterial;
        lineRenderer.sortingLayerName = "Player";
        lineRenderer.sortingOrder = -10 ;

        SpriteRenderer tipSpr = lineObj.AddComponent<SpriteRenderer>();
        tipSpr.sprite = tipSprite;
        tipSpr.sortingLayerName = "Player";
        tipSpr.sortingOrder = -10;
        lineObj.SetActive(false);


        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lassoActivate)
        {
            if (!lassoBack)
            {
                //올가미 움직인다. 
                timer += Time.deltaTime;

                float percent = timer / moveTime;
                Vector2 pos = Vector2.Lerp((Vector2)transform.position, (Vector2)transform.position + (targetDir * lassoRange), moveCurve.Evaluate(percent));;
                lassoTip = pos;

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
                float percent = timer / moveTime;
                Vector2 pos = Vector2.Lerp(preLassoTip, (Vector2)transform.position, moveCurve.Evaluate(percent));
                lassoTip = pos;

                if(timer > turnTime)
                {
                    lassoActivate = false;
                    lassoBack = false;
                    //lineColl.enabled = false;
                    lineObj.SetActive(false);
                }
            }
            //올가미 위치 업데이트
            UpdateLassoPosition();
        }
        
        //올가미 처음 발사 
        if(!lassoActivate && !inLassoMovement)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                //Debug.Log("Launch Lasso");

                LassoThrow();
            }
        }




        //트리거로 이동 중일때 다시 클릭하면 impactAttack 활성화.
        if (inLassoMovement)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                impactAttack = true;
                Debug.Log("impactAttack is true");
            }
        }

        //impactAttack 쿨타임
        if(impactAttack)
        {
            impactTimer += Time.deltaTime;
            if(impactTimer > impactTime)
            {
                impactTimer = 0;
                impactAttack = false;
                Debug.Log("impactAttack is false");
            }
        }
    }


    private void FixedUpdate()
    {
        //올가미 부딪혔을 때 BigTrigger / MediumTrigger
        if (inLassoMovement)
        {
            SwitchMovement();
            return;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //행성에 부딪히면 초기화시키기.

        //if (!inLassoMovement)
        //    return;

        //if (collision.collider.CompareTag("Planet"))
        //{   

        //    bigTrigger = false;

        //    lassoActivate = false;
        //    inLassoMovement = false;

        //    lineObj.SetActive(false);

        //    rb.velocity = Vector2.zero;

        //}
    }


    void LassoThrow()
    {
        //올가미를 처음 던질 때

        lassoTip = transform.position;  //발사 시작 위치 초기화

        //올가미를 던질 때 
        lassoActivate = true;
        lineObj.SetActive(true); 
        UpdateLassoPosition();
        lineColl.enabled = true;

        Vector3 inputPos = Input.mousePosition;
        inputPos.z = 10;    //z는 카메라에서부터의 거리
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputPos);
        mousePos.z = 0;
        targetDir = (mousePos - transform.position).normalized;



        timer = 0;
    }



    void UpdateLassoPosition()
    {
        //라인 렌더러를 업데이트한다.
        lineObj.transform.position = lassoTip;

        //올가미 머리를 회전시킨다
        Vector2 tipDir = (lassoTip - (Vector2)transform.position).normalized;
        Vector3 upVec = Quaternion.Euler(0, 0, 90) * tipDir;
        Quaternion rot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
        lineObj.transform.rotation = rot;

        //라인 랜더러 조정
        Vector2 p = lassoTip + (tipDir * -1f * 0.5f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, p);
    }






    void SwitchMovement()
    {
        switch (bigTrigger)
        {
            case true:
                //올가미를 걸으면, 현재 속도를 유지하면서 가속도를 서서히 받아서 최종적으로 원하는 시간에 원하는 위치에 도달하는것. 
                timer += Time.fixedDeltaTime;
                float percent = timer / bigTMoveTime;

                Vector2 maybePos = startPlayerPos + (startVelocity * percent);
                Vector2 pos = Vector2.Lerp(maybePos, targetPos, moveCurve.Evaluate(percent));
                rb.MovePosition(pos);

                //올가미 위치 업데이트
                UpdateLassoPosition();

                if (timer > bigTMoveTime)
                {
                    //목적지에 도착했을 때 
                    bigTrigger = false;

                    lassoActivate = false;
                    inLassoMovement = false;
                    timer = 0;

                    checker.activate = true;

                    lineObj.SetActive(false);

                    //타겟 활성화
                    if (target != null)
                    {
                        EnableTarget();
                        target = null;
                    }


                }
                break;

            case false:
                timer += Time.fixedDeltaTime;
                percent = timer / bigTMoveTime;

                maybePos = startPlayerPos + (startVelocity * percent);
                pos = Vector2.Lerp(maybePos, targetPos, moveCurve.Evaluate(percent));
                rb.MovePosition(pos);

                Vector2 p = startEnemyPos + (enemyVelocity * percent);
                Vector2 ePos = Vector2.Lerp(p, targetPos, moveCurve.Evaluate(percent));
                targetRb.MovePosition(ePos);

                //올가미 위치 업데이트
                lassoTip = target.position;
                UpdateLassoPosition();

                if (timer > bigTMoveTime)
                {
                    //목적지에 도착했을 때 

                    lassoActivate = false;
                    inLassoMovement = false;
                    timer = 0;

                    checker.activate = true;

                    lineObj.SetActive(false);
                    
                    //타겟 활성화
                    if (target != null)
                    {
                        EnableTarget();
                        target = null;
                    }

                    //임팩트 어택 상태일때만 발동. 이외의 경우 그냥 지나쳐감.
                    if (impactAttack)
                    {
                        //서로에게 반대방향으로 임팩트
                        Vector2 vel = (targetPos - startPlayerPos).normalized;

                        rb.AddForce(-1f * vel * normalForce, ForceMode2D.Impulse);
                        targetRb.AddForce(vel * normalForce, ForceMode2D.Impulse);
                    }


                }
                break;

        }

    }




    public void TriggerByBig(Collider2D other)
    {
        //올가미의 움직임을 정지한다.
        lassoActivate = false;
        lassoBack = false;

        //움직임을 시작한다.
        inLassoMovement = true;

        //더이상 충돌을 못하게 비활성화한다.
        lineColl.enabled = false;

        targetPos = lassoTip;
        startPlayerPos = transform.position;

        bigTrigger = true;
        timer = 0f;

        startVelocity = rb.velocity;
        bigTMoveTime = (targetPos - startPlayerPos).magnitude / bigTForce;
        bigTMoveTime = Mathf.Clamp(bigTMoveTime, 0.7f, 3f);

        //대상이 Planet이면 null, 아니면 Enemy, Trap이 들어온다.
        if(other != null)
        {
            target = other.transform;
            DisableTarget();
        }

    }

    public void TriggerByMedium(Collider2D other)
    {
        //올가미의 움직임을 정지한다.
        lassoActivate = false;
        lassoBack = false;

        //움직임을 시작한다.
        inLassoMovement = true;

        //더이상 충돌을 못하게 비활성화한다.
        lineColl.enabled = false;

        startPlayerPos = transform.position;
        startEnemyPos = other.transform.position;
        targetPos = (startPlayerPos + startEnemyPos) / 2f;

        timer = 0f;

        startVelocity = rb.velocity;
        bigTMoveTime = (targetPos - startPlayerPos).magnitude / bigTForce;
        bigTMoveTime = Mathf.Clamp(bigTMoveTime, 0.7f, 3f);

        target = other.transform;
        targetRb = target.GetComponent<Rigidbody2D>();
        enemyVelocity = targetRb.velocity;

        if (target != null)
        {
            DisableTarget();
        }

    }

    void DisableTarget()
    {
        //타겟 대상을 정지합니다
        if (target.CompareTag("Enemy"))
        {
            target.GetComponent<Enemy>().activate = false;
        }
        else if (target.CompareTag("EnemyHitableProjectile"))
        {

        }
    }

    void EnableTarget()
    {
        //타겟 대상을 정지합니다
        if (target.CompareTag("Enemy"))
        {
            target.GetComponent<Enemy>().activate = true;
        }
        else if (target.CompareTag("EnemyHitableProjectile"))
        {

        }
    }

}
