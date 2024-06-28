using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjMov_Normal : ProjectileMovement
{
    //이 오브젝트가 넉백을 얼마나 할 것인지 정해주는 변수들
    public float knockBackTime = 0.2f;
    protected float _knockBackTime; //실제 대기 
    public float knockBackForce = 2f;
    protected bool knockBackOn = false;

    float accelerateDuration = 1f;

    public override void StartMovement(float speed)
    {
        
        //기본 속도 움직임 
        //rb.AddForce(transform.right * speed, ForceMode2D.Impulse);

        //가속도 움직임.
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
