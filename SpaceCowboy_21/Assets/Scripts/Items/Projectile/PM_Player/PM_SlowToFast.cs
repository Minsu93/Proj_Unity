using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_SlowToFast : ProjectileMovement
{
    bool startMove;
    [SerializeField] float velocityLimit = 6.0f;
    //�Ϲ� �Ѿ��� ���
    public override void StartMovement(float speed)
    {
        rb.velocity = Vector3.zero;
        startMove = true;
        this.speed = speed;
    }

    public override void StopMovement()
    {
        rb.velocity = Vector2.zero;
        startMove = false;
    }

    private void FixedUpdate()
    {
        if (startMove)
        {
            if(rb.velocity.magnitude < velocityLimit)
            {
                rb.AddForce(transform.right * speed, ForceMode2D.Force);
            }
        }
    }
}
