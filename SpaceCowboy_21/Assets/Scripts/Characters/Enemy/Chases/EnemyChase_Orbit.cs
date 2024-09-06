using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase_Orbit : EnemyChase
{
    Vector2 centerPoint;
    int direction = 1;
    Collider2D coll;

    public float ascendSpeed = 2.0f;
    public float highAltitude = 4f; //적정 고도 
    public float lowAltitude = 3f; //적정 고도 
    float targetAltitude;
    float targetTolerance = 0.1f; //여유분
    float degree;



    public void ChangeDirection(bool isRight)
    {
        direction = isRight ? 1 : -1;
    }

    public void SetCenterPoint(Planet planet)
    {
        curPlanet = planet;
        centerPoint = curPlanet.transform.position;
        coll = curPlanet.transform.GetComponent<Collider2D>();
        targetAltitude = Random.Range(lowAltitude, highAltitude);

        SetDegree();

    }

    /// <summary>
    /// 현재 행성의 중심 위치를 기준으로 orbit의 위치(각도)를 재조정.
    /// </summary>
    public void ResetCenterPoint()
    {
        curPlanet = null;
        coll = null;
    }

    //degree 변수를 조절합니다. 
    void SetDegree()
    {
        float signedAngle = Vector2.SignedAngle(Vector2.up, (Vector2)transform.position - centerPoint);
        degree = (-signedAngle + 360) % 360;

        transform.rotation = Quaternion.Euler(0, 0, degree * -1); //가운데를 바라보게 각도 조절
    }

    //float updateTimer = 0f;
    public override void OnChaseAction()
    {
        if (curPlanet == null)
        {
            return;
        }

        Vector2 targetVec = ((Vector2)transform.position - centerPoint);
        float radius = targetVec.magnitude;

        //높이 조절
        Vector2 moveAltitudeVec = SetAltitudeRoutine();

        var rad = Mathf.Deg2Rad * (degree);
        var x = radius * Mathf.Sin(rad);
        var y = radius * Mathf.Cos(rad);

        rb.MovePosition(centerPoint + new Vector2(x, y) + moveAltitudeVec);
        transform.rotation = Quaternion.Euler(0, 0, degree * -1); //가운데를 바라보게 각도 조절

        degree = (degree + (Time.deltaTime * moveSpeed * direction) + 360) % 360;


    }

    //높이 맞추기 코루틴
    Vector2 SetAltitudeRoutine()
    {
        Vector2 moveVec = Vector2.zero;

        Vector2 closePoint = coll.ClosestPoint(transform.position);
        Vector2 upVec = (Vector2)transform.position - closePoint;
        float dist = upVec.magnitude;
        Vector2 upDir = upVec.normalized;

        if (dist > targetAltitude + targetTolerance)
        {
            moveVec = ascendSpeed * Time.deltaTime * -upDir;
        }
        else if (dist < targetAltitude - targetTolerance)
        {
            moveVec = ascendSpeed * Time.deltaTime * upDir;
        }

        return moveVec;
    }
}
