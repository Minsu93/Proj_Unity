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
        //�ð��� ������Ʈ ����
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
        //�ð����� �������� �����Ѵ�.
        lassoActivate = false;
        lassoBack = false;

        //�÷��̾� �߷��� ������Ų��
        //playerGravity.activate = false;

        //�������� �����Ѵ�.
        inLassoMovement = true;

        //���̻� �浹�� ���ϰ� ��Ȱ��ȭ�Ѵ�.
        lineColl.enabled = false;

        targetPos = lassoTip;
        startPlayerPos = transform.position;
        bigTrigger = true;
        timer = 0f;
    }

    public void TriggerByMedium(Collider2D other)
    {
        //�ð����� �������� �����Ѵ�.
        lassoActivate = false;

        //�������� �����Ѵ�.
        inLassoMovement = true;

        //���̻� �浹�� ���ϰ� ��Ȱ��ȭ�Ѵ�.
        lineColl.enabled = false;
        middleTrigger = true;
        timer = 0f;

        this.other = other.transform;


        //�÷��̾ ������Ų��.(AddImpulse) ������ PlayerBehavior �� �ؾ���
        Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 _dir = (other.transform.position - transform.position).normalized;
            playerRb.AddForce(_dir * 10f, ForceMode2D.Impulse);
        }

        //������ Impulse�� �ش�. 
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
