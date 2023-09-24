using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingPunch : MonoBehaviour
{

    //발사 쿨타임
    public float punchCooltime;
    public float coolTime;
    public float PowMultiply;
    public float punchDuration;
    public float punchDamage;
    public float punchKnockBack;
    public bool activeBulletTime;
    public float bulletScale;
    public GameObject particle;

    //당기는 힘
    float shotPow;
    float duration;
    Vector2 shotDir;
    Vector2 startPos;
    bool activateSlineShot;
    bool activatePunch;
    bool activate;

    //화살표
    LineRenderer punchLine;
    Rigidbody2D rb;
    GravityFinder gravityChecker;

    private void Awake()
    {
        punchLine = GetComponent<LineRenderer>();
        //라인 렌더러 생성
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

        //초기화
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

        //마우스 버튼으로 플레이어 캐릭터를 누르면 시작됨. 
        if (Input.GetMouseButtonDown(1))
        {
            activateSlineShot = true;
            punchLine.enabled = true;
        }


        //마우스 버튼을 누르고 있는 동안
        //화살표로 발사 세기, 발사 각도, line renderer 생성 등. 
        if (Input.GetMouseButton(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DrawLineRenderer(mousePos);
        }


        //마우스 버튼을 떼면 
        //발사

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
        //테스트용으로 만들어보는, 캐릭터 날리기 슬링샷
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
