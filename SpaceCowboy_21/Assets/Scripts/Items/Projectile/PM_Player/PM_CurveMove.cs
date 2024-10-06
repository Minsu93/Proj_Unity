using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_CurveMove : ProjectileMovement
{
    [SerializeField] float spd = 0.5f;
    [SerializeField] float height = 2.0f;
    [SerializeField] bool upsideDown;
    bool moveStart;
    float timer;
    float sinTimer;
    Vector2 startPos;
    Vector2 direction;
    Vector2 upVec;

    public override void StartMovement(float speed)
    {
        this.speed = speed;
        moveStart = true;
        float litteBackfloat = -0.1f;
        timer = 0;
        sinTimer = litteBackfloat;

        startPos = transform.position;
        direction = transform.right;
        upVec = transform.up;
    }


    public override void StopMovement()
    {
        moveStart = false;
    }

    private void FixedUpdate()
    {
        if (!moveStart) return;

        timer += Time.fixedDeltaTime;
        sinTimer += Time.fixedDeltaTime * spd;

        int up = upsideDown ? -1 : 1;
        float y = Mathf.Sin(sinTimer) * height * up;
        Debug.Log(y);
        Vector2 pos = startPos + ( speed * timer * direction) + (upVec * y);
        rb.MovePosition(pos);
    }
}
