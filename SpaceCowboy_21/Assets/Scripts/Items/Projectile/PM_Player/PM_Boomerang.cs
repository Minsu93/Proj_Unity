using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_Boomerang : ProjectileMovement
{

    [SerializeField] AnimationCurve animCurve;
    bool moveStart;
    bool moveBack;
    float returnTime;
    float moveDistance;
    float timer;
    Vector2 direction;
    Vector2 startPos;
    Projectile_Boomerang penetrate;

    private void OnEnable()
    {
        if(penetrate == null)
        {
            penetrate = GetComponent<Projectile_Boomerang>();
        }
    }

    public override void StartMovement(float speed)
    {
        startPos = transform.position;
        direction = transform.right;
        moveStart = true;
        moveBack = false;
        timer = 0;
        
    }

    public void StartMovement(float speed, float distance)
    {
        moveDistance = distance;
        returnTime = distance / speed;

        startPos = transform.position;
        direction = transform.right;
        moveStart = true;
        moveBack = false;
        timer = 0;
    }

    public override void StopMovement()
    {
        moveStart = false;
    }

    private void FixedUpdate()
    {
        if (!moveStart) return;

        if (!moveBack)
        {
            timer += Time.fixedDeltaTime / returnTime;
            if(timer >= 1)
            {
                MoveReturn();
            }
        }
        else
        {
            timer -= Time.fixedDeltaTime / returnTime;
            if(timer <= 0)
            {
                MoveFinish();
            }
        }

        Vector2 pos = Vector2.Lerp(startPos, startPos + (direction * moveDistance), animCurve.Evaluate(timer));
        rb.MovePosition(pos);
    }

    void MoveReturn()
    {
        moveBack = true;
        if(penetrate!= null)
        {
            penetrate.ResetHitList();
        }
    }

    void MoveFinish()
    {
        StopMovement();
        if (penetrate != null)
        {
            penetrate.ReturnFinish();
        }
    }
}
