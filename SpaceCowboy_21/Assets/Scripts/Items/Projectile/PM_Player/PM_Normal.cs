using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_Normal : ProjectileMovement
{
    //�Ϲ� �Ѿ��� ���
    public override void StartMovement(float speed)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }
      
    public override void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

}
