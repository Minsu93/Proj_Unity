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
    bool middleTrigger;
    Vector2 startEnemyPos;
    Vector2 enemyVelocity;
    Transform target;
    Collider2D targetColl;
    Rigidbody2D targetRb;

    LineRenderer lineRenderer;
    GameObject lineObj;
    CircleCollider2D lineColl;

    Rigidbody2D rb;
    PlayerGravity playerGravity;
    

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

        GravityLassoChecker checker =  lineObj.AddComponent<GravityLassoChecker>();
        checker.lasso = this;

        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = .2f;
        lineRenderer.endWidth = .2f;
        lineRenderer.material = lassoMaterial;

        SpriteRenderer tipSpr = lineObj.AddComponent<SpriteRenderer>();
        tipSpr.sprite = tipSprite;

        lineObj.SetActive(false);


        rb = GetComponent<Rigidbody2D>();
        playerGravity = GetComponent<PlayerGravity>();
    }

    // Update is called once per frame
    void Update()
    {

        /// �߷� �ð��̴� �� ������Ʈ���� ������ �ε����� �ʾҴ��� üũ�Ѵ�. 
        /// üũ�� ����� ������ ��� �����´�
        /// üũ�� ����� ���� ��� ������ �������� Add Impulse
        /// üũ�� ����� ������ ���(Planet, Boss) �� �������� �̵��Ѵ�. 
        /// . 
        /// �ϴ� ���콺 �� Ŭ���� ������ �ð��̸� �߻��ϸ�
        /// �߻��� �ð��̴� �� ���ư��ٰ� ���� �ð�, �Ÿ��� �Ǹ� ���ƿ´�. 
        /// 


        if (lassoActivate)
        {
            if (!lassoBack)
            {
                //�ð��� �����δ�. 
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
                //�ð��̴� �Ųٷ� ���ƿ´�.
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
        if (inLassoMovement)
        {
            SwitchMovement();
            return;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
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

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        targetDir = ((Vector2)mousePos - (Vector2)transform.position).normalized;



        timer = 0;
    }



    void UpdateLassoPosition()
    {
        //���� �������� ������Ʈ�Ѵ�.
        lineObj.transform.position = lassoTip;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, lassoTip);

        Vector2 tipDir = lassoTip - (Vector2)transform.position;
        Vector3 upVec = Quaternion.Euler(0, 0, 90) * tipDir;
        Quaternion rot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
        lineObj.transform.rotation = rot;


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
                    middleTrigger = false;

                    lassoActivate = false;
                    inLassoMovement = false;
                    timer = 0;

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

        middleTrigger = true;
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
        else if (target.CompareTag("EnemyProj"))
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
        else if (target.CompareTag("EnemyProj"))
        {

        }
    }

}
