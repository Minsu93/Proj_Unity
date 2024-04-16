using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjMov_Normal : ProjectileMovement
{

    public override void StartMovement(float speed)
    {
        
        //기본 속도 움직임 
        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }

    public override void StopMovement()
    {
        rb.velocity = Vector2.zero;
        return;
    }


}
