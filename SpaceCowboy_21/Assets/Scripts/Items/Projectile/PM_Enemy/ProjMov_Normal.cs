using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjMov_Normal : ProjectileMovement
{
    public bool slowProj;   //�߻� �� �� �� �ں��� �߷��� ������ ũ�� �޴´�.
    public float slowTimer;
    float timer;
    public float slowSpeed;
    float gravMul;

    public override void StartMovement(float speed)
    {
        
        timer = slowTimer;
        gravMul = 0;
        //�⺻ �ӵ� ������ 
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
        //2. �ð��� ������ �߷��� �� �ޱ�
        if (gravity != null)
        {
            gravMul += Time.deltaTime * slowSpeed; ;
            gravMul = Mathf.Clamp(gravMul, 0f, 3f);
            gravity.GravityAdded(gravMul);
        }
        */


        //�׽�Ʈ �غ� ��
        //1. �ð��� ������ ���� ��������
        /*
        float vel = rb.velocity.magnitude;
        Vector2 dir = rb.velocity.normalized;
        vel -= Time.deltaTime * slowSpeed;
        
        rb.velocity = dir * vel;
        */



        //3. �ð��� ������ �̵� ���� �� �ޱ�
    }
}
