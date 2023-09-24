using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingPunch : MonoBehaviour
{

    //�߻� ��Ÿ��
    public float punchCooltime;
    public float coolTime;
    public float PowMultiply;
    public float punchDuration;
    public float punchDamage;
    public float punchKnockBack;
    public bool activeBulletTime;
    public float bulletScale;
    public GameObject particle;

    //���� ��
    float shotPow;
    float duration;
    Vector2 shotDir;
    Vector2 startPos;
    bool activateSlineShot;
    bool activatePunch;
    bool activate;

    //ȭ��ǥ
    LineRenderer punchLine;
    Rigidbody2D rb;
    GravityFinder gravityChecker;

    private void Awake()
    {
        punchLine = GetComponent<LineRenderer>();
        //���� ������ ����
        /*
        punchLine = gameObject.AddComponent<LineRenderer>();
        punchLine.startWidth = 0.02f;
        punchLine.endWidth = 0.02f;
        punchLine.sortingLayerName = "Above";

        punchLine.material = new Material(Shader.Find("Sprites/Default"));
        punchLine.startColor = new Color(1, 1, 1, 1);
        punchLine.endColor = new Color(1, 1, 1, 0);
        */
        //punchLine.enabled = false;

        //�ʱ�ȭ
        activateSlineShot = false;
        activatePunch = false;
        duration = punchDuration;
        coolTime = punchCooltime;
        activate = true;

        rb = GetComponent<Rigidbody2D>();
        gravityChecker = GetComponent<GravityFinder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!activate && !activatePunch)
        {
            coolTime += Time.deltaTime;

            if(coolTime >= punchCooltime)
            {
                coolTime = punchCooltime;
                activate = true;
            }

            return;
        }

        if(activatePunch)
        {
            duration -= Time.deltaTime;
            if(duration <= 0)
            {
                EndPunch();
            }
        }

        //���콺 ��ư���� �÷��̾� ĳ���͸� ������ ���۵�. 
        if (Input.GetMouseButtonDown(1))
        {
            activateSlineShot = true;
            punchLine.enabled = true;
        }


        //���콺 ��ư�� ������ �ִ� ����
        //ȭ��ǥ�� �߻� ����, �߻� ����, line renderer ���� ��. 
        if (Input.GetMouseButton(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DrawLineRenderer(mousePos);
        }


        //���콺 ��ư�� ���� 
        //�߻�

        if (Input.GetMouseButtonUp(1))
        {
            ShootPlayer();
            StartPunch();
            activateSlineShot = false;
            punchLine.enabled = false;

        }

    }

    void DrawLineRenderer(Vector2 mousePos)
    {
        if (!activateSlineShot)
            return;

        shotDir = (mousePos - (Vector2)transform.position).normalized;
        shotPow = (mousePos - (Vector2)transform.position).magnitude * 2f;
        Vector2 tempPos = (Vector2)transform.position + shotDir * shotPow * 0.5f;

        punchLine.SetPosition(0, transform.position);
        punchLine.SetPosition(1, tempPos);

    }

    void ShootPlayer()
    {
        //�׽�Ʈ������ ������, ĳ���� ������ ������
        if (!activateSlineShot)
            return;
        rb.velocity = Vector2.zero;
        rb.AddForce(shotDir * PowMultiply, ForceMode2D.Impulse);
    }

    void StartPunch()
    {
        activatePunch = true;
        gravityChecker.activate = false;
        coolTime = 0;
        activate = false;
    }

    void EndPunch()
    {
        activatePunch = false;
        gravityChecker.activate = true;
        duration = punchDuration;
        rb.velocity = rb.velocity * 0.2f;
        
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!activatePunch)
            return;

        Instantiate(particle, collision.GetContact(0).point, Quaternion.identity);


        if (collision.transform.TryGetComponent<Health>(out Health targetHealth))
        {


            targetHealth.AnyDamage(punchDamage);

            if (collision.transform.CompareTag("Enemy"))
            {
                //targetHealth.KnockBack(transform, punchKnockBack);
            }
        }

        Vector2 dir = transform.position - collision.transform.position;
        rb.AddForce(dir.normalized * 3f, ForceMode2D.Impulse);


        EndPunch();
    }

    void ResetPunch()
    {
        coolTime = punchCooltime;
        activate = true;
    }
}
