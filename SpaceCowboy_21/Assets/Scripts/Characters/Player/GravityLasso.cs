using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GravityLasso : MonoBehaviour
{
    Vector2 lassoTip;   //�ð��� ��ġ
    bool lassoActivate;  // �ð��̰� �߻�Ǿ�����
    bool lassoBack;  //�ð��̰� ���ƿ�����
    bool inLassoMovement; //�ð��̷� �����̴� ���ΰ���

    [Header("Lasso Property")]

    public float lassoSize = 0.3f;
    public float lassoRange = 3f;   //�ð��� �����Ÿ�
    public float lassoSpeed = 1f;   //�ð��� �ӵ�
    public float moveTime = 2f;     //�ð��� �̵� 
    public float turnTime = 1f;
    float timer;
    Vector2 targetDir;  //��ǥ ����
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
        //�ð��� ������Ʈ ����
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
                //�ð��� �����δ�. 
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
                //�ð��̴� �Ųٷ� ���ƿ´�.
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
            //�ð��� ��ġ ������Ʈ
            UpdateLassoPosition();
        }
        
        //�ð��� ó�� �߻� 
        if(!lassoActivate && !inLassoMovement)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                //Debug.Log("Launch Lasso");

                LassoThrow();
            }
        }




        //Ʈ���ŷ� �̵� ���϶� �ٽ� Ŭ���ϸ� impactAttack Ȱ��ȭ.
        if (inLassoMovement)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                impactAttack = true;
                Debug.Log("impactAttack is true");
            }
        }

        //impactAttack ��Ÿ��
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
        //�ð��� �ε����� �� BigTrigger / MediumTrigger
        if (inLassoMovement)
        {
            SwitchMovement();
            return;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //�༺�� �ε����� �ʱ�ȭ��Ű��.

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
        //�ð��̸� ó�� ���� ��

        lassoTip = transform.position;  //�߻� ���� ��ġ �ʱ�ȭ

        //�ð��̸� ���� �� 
        lassoActivate = true;
        lineObj.SetActive(true); 
        UpdateLassoPosition();
        lineColl.enabled = true;

        Vector3 inputPos = Input.mousePosition;
        inputPos.z = 10;    //z�� ī�޶󿡼������� �Ÿ�
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputPos);
        mousePos.z = 0;
        targetDir = (mousePos - transform.position).normalized;



        timer = 0;
    }



    void UpdateLassoPosition()
    {
        //���� �������� ������Ʈ�Ѵ�.
        lineObj.transform.position = lassoTip;

        //�ð��� �Ӹ��� ȸ����Ų��
        Vector2 tipDir = (lassoTip - (Vector2)transform.position).normalized;
        Vector3 upVec = Quaternion.Euler(0, 0, 90) * tipDir;
        Quaternion rot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
        lineObj.transform.rotation = rot;

        //���� ������ ����
        Vector2 p = lassoTip + (tipDir * -1f * 0.5f);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, p);
    }






    void SwitchMovement()
    {
        switch (bigTrigger)
        {
            case true:
                //�ð��̸� ������, ���� �ӵ��� �����ϸ鼭 ���ӵ��� ������ �޾Ƽ� ���������� ���ϴ� �ð��� ���ϴ� ��ġ�� �����ϴ°�. 
                timer += Time.fixedDeltaTime;
                float percent = timer / bigTMoveTime;

                Vector2 maybePos = startPlayerPos + (startVelocity * percent);
                Vector2 pos = Vector2.Lerp(maybePos, targetPos, moveCurve.Evaluate(percent));
                rb.MovePosition(pos);

                //�ð��� ��ġ ������Ʈ
                UpdateLassoPosition();

                if (timer > bigTMoveTime)
                {
                    //�������� �������� �� 
                    bigTrigger = false;

                    lassoActivate = false;
                    inLassoMovement = false;
                    timer = 0;

                    checker.activate = true;

                    lineObj.SetActive(false);

                    //Ÿ�� Ȱ��ȭ
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

                //�ð��� ��ġ ������Ʈ
                lassoTip = target.position;
                UpdateLassoPosition();

                if (timer > bigTMoveTime)
                {
                    //�������� �������� �� 

                    lassoActivate = false;
                    inLassoMovement = false;
                    timer = 0;

                    checker.activate = true;

                    lineObj.SetActive(false);
                    
                    //Ÿ�� Ȱ��ȭ
                    if (target != null)
                    {
                        EnableTarget();
                        target = null;
                    }

                    //����Ʈ ���� �����϶��� �ߵ�. �̿��� ��� �׳� �����İ�.
                    if (impactAttack)
                    {
                        //���ο��� �ݴ�������� ����Ʈ
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
        //�ð����� �������� �����Ѵ�.
        lassoActivate = false;
        lassoBack = false;

        //�������� �����Ѵ�.
        inLassoMovement = true;

        //���̻� �浹�� ���ϰ� ��Ȱ��ȭ�Ѵ�.
        lineColl.enabled = false;

        targetPos = lassoTip;
        startPlayerPos = transform.position;

        bigTrigger = true;
        timer = 0f;

        startVelocity = rb.velocity;
        bigTMoveTime = (targetPos - startPlayerPos).magnitude / bigTForce;
        bigTMoveTime = Mathf.Clamp(bigTMoveTime, 0.7f, 3f);

        //����� Planet�̸� null, �ƴϸ� Enemy, Trap�� ���´�.
        if(other != null)
        {
            target = other.transform;
            DisableTarget();
        }

    }

    public void TriggerByMedium(Collider2D other)
    {
        //�ð����� �������� �����Ѵ�.
        lassoActivate = false;
        lassoBack = false;

        //�������� �����Ѵ�.
        inLassoMovement = true;

        //���̻� �浹�� ���ϰ� ��Ȱ��ȭ�Ѵ�.
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
        //Ÿ�� ����� �����մϴ�
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
        //Ÿ�� ����� �����մϴ�
        if (target.CompareTag("Enemy"))
        {
            target.GetComponent<Enemy>().activate = true;
        }
        else if (target.CompareTag("EnemyHitableProjectile"))
        {

        }
    }

}
