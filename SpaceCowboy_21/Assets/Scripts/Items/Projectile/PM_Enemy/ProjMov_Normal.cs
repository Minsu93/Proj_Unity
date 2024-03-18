using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjMov_Normal : ProjectileMovement
{
    public bool slowProj;   //발사 후 몇 초 뒤부터 중력의 영향을 크게 받는다.
    public float slowTimer;
    float timer;
    public float slowSpeed;
    float gravMul;

    public override void StartMovement(float speed)
    {
        
        timer = slowTimer;
        gravMul = 0;
        //기본 속도 움직임 
        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }

    public override void StopMovement()
    {
        rb.velocity = Vector2.zero;
        return;
    }

    public override void Update()
    {
        
        if (!slowProj)
            return;


        if(timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        
        rb.velocity = rb.velocity * slowSpeed;
        /*
        //2. 시간이 갈수록 중력을 더 받기
        if (gravity != null)
        {
            gravMul += Time.deltaTime * slowSpeed; ;
            gravMul = Mathf.Clamp(gravMul, 0f, 3f);
            gravity.GravityAdded(gravMul);
        }
        */


        //테스트 해볼 것
        //1. 시간이 갈수록 점점 느려지기
        /*
        float vel = rb.velocity.magnitude;
        Vector2 dir = rb.velocity.normalized;
        vel -= Time.deltaTime * slowSpeed;
        
        rb.velocity = dir * vel;
        */



        //3. 시간이 갈수록 이동 저항 더 받기
    }
}
