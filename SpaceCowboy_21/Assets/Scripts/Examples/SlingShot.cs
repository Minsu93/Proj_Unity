using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    //총알의 종류
    public GameObject projPrefab;
    //발사 쿨타임
    public float shotCooltime;
    public float projectileSpread;
    public int numberOfProjectiles;
    public float shotPowMultiply;


    //당기는 힘
    float shotPow;
    Vector2 shotDir;
    Vector2 startPos;
    bool activateSlineShot;
    //화살표
    LineRenderer lineRenderer;
    Rigidbody2D rb;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        activateSlineShot = false;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {


        //마우스 버튼으로 플레이어 캐릭터를 누르면 시작됨. 
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            activateSlineShot = true;
            lineRenderer.enabled = true;
            Time.timeScale = 0.5f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }


        //마우스 버튼을 누르고 있는 동안
        //화살표로 발사 세기, 발사 각도, line renderer 생성 등. 
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            DrawLineRenderer(mousePos);
        }


        //마우스 버튼을 떼면 
        //발사

        if (Input.GetMouseButtonUp(0))
        {
            Shoot();
            activateSlineShot = false;
            lineRenderer.enabled = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02F;

        }



    }
    
    void DrawLineRenderer(Vector2 mousePos)
    {
        if (!activateSlineShot)
            return;

        shotDir = (mousePos - rb.position).normalized;
        shotPow = (mousePos - rb.position).magnitude * 5f;
        Vector2 tempPos = (Vector2)transform.position + shotDir * shotPow * 0.5f;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, tempPos);

    }

    void GetProjectile()
    {
        //총알 정보를 읽어오는 함수
    }

    void Shoot()
    {
        //총알을 발사하는 함수

        if (!activateSlineShot)
            return;

        float totalSpread = projectileSpread * (numberOfProjectiles - 1);
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * shotDir;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);


        //멀티샷
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));
            //총알 생성
            //GameObject projectile = GameManager.Instance.poolManager.Get(projPrefab);
            //projectile.transform.position = transform.position;
            //projectile.transform.rotation = tempRot;
            //projectile.GetComponent<Projectile>().init(1f, 5f, shotPow * shotPowMultiply, 0.5f);
            GameObject proj = Instantiate(projPrefab, (Vector2)transform.position + (shotDir * 0.5f), Quaternion.identity);
            proj.GetComponent<Rigidbody2D>().AddForce(shotDir * shotPow, ForceMode2D.Impulse);

        }

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Shoot);

    }



}
