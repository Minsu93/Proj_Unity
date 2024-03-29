using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjMov_Normal : ProjectileMovement
{
    float gravMul;

    public override void StartMovement(float speed)
    {
        
        gravMul = 0;
        //�⺻ �ӵ� ������ 
        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }

    public override void StopMovement()
    {
        rb.velocity = Vector2.zero;
        return;
    }


}
