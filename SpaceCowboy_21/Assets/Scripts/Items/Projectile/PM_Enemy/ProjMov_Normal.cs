using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjMov_Normal : ProjectileMovement
{
    //�� ������Ʈ�� �˹��� �󸶳� �� ������ �����ִ� ������
    public float knockBackTime = 0.2f;
    protected float _knockBackTime; //���� ��� 
    public float knockBackForce = 2f;
    protected bool knockBackOn = false;

    float accelerateDuration = 1f;

    public override void StartMovement(float speed)
    {
        
        //�⺻ �ӵ� ������ 
        //rb.AddForce(transform.right * speed, ForceMode2D.Impulse);

        //���ӵ� ������.
        StartCoroutine(AccelerateRoutine(speed));
    }

    IEnumerator AccelerateRoutine(float speed)
    {
        float spd = 0f;
        float accel = speed / accelerateDuration;
        while (spd < speed)
        {
            spd += Time.deltaTime * accel;
            rb.velocity = transform.right * spd;
            yield return null;
        }
    }

    public override void StopMovement()
    {
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        return;
    }




}
