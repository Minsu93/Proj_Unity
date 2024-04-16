using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase_Orbit : EnemyChase
{
    Vector2 centerPoint;
    int direction = 1;
    Collider2D coll;

    public float highAltitude = 4f; //적정 고도 
    public float lowAltitude = 3f; //적정 고도 
    float targetAltitude;
    float targetTolerance = 0.1f; //여유분
    float degree;

    //test
    [SerializeField] float altitude;

    public void ChangeDirection(bool isRight)
    {
        direction = isRight ? -1 : 1;
    }

    public void SetCenterPoint(Planet planet)
    {
        curPlanet = planet;
        centerPoint = curPlanet.transform.position;
        coll = curPlanet.transform.GetComponent<Collider2D>();
        targetAltitude = Random.Range(lowAltitude, highAltitude);

        float signedAngle = Vector2.SignedAngle(Vector2.up, (Vector2)transform.position - centerPoint);
        degree = (-signedAngle + 360) % 360;
    }


    public override void OnChaseAction()
    {
        if (curPlanet == null) return;

        Vector2 targetVec = ((Vector2)transform.position - centerPoint);
        float radius = targetVec.magnitude;
        //회전 
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, targetVec.normalized);
        //높이 조절
        //Vector2 moveAltitudeVec= SetAltitudeRoutine();
        //캐릭터 속도 조절
        //이동 
        //transform.RotateAround(centerPoint, Vector3.forward, direction * moveSpeed * Time.deltaTime);
        degree += Time.deltaTime * moveSpeed * direction;
        if (degree < 360)
        {
            var rad = Mathf.Deg2Rad * (degree);
            var x = radius * Mathf.Sin(rad);
            var y = radius * Mathf.Cos(rad);
            transform.position = centerPoint + new Vector2(x, y);
            transform.rotation = Quaternion.Euler(0, 0, degree * -1); //가운데를 바라보게 각도 조절
        }
        else
        {
            degree = 0;
        }

    }

    //높이 맞추기 코루틴
    Vector2 SetAltitudeRoutine()
    {
        Vector2 moveVec = Vector2.zero;
        Vector2 closePoint = coll.ClosestPoint(transform.position);
        Vector2 upVec = (Vector2)transform.position - closePoint;
        float dist = upVec.magnitude;
        Vector2 upDir = upVec.normalized;
        Debug.DrawRay(transform.position, upDir, Color.green);
        altitude = dist;
        float speed = 3f;

        if (dist > targetAltitude + targetTolerance)
        {
            //rb.MovePosition(rb.position + (speed * Time.deltaTime * -upDir));
            moveVec = speed * Time.deltaTime * -upDir;
        }
        else if (dist < targetAltitude - targetTolerance)
        {
            //rb.MovePosition(rb.position + (speed * Time.deltaTime * upDir));
            moveVec = speed * Time.deltaTime * upDir;
        }

        return moveVec;
    }
}
