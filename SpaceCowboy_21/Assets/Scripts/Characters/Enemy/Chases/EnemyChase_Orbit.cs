using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase_Orbit : EnemyChase
{
    Vector2 centerPoint;
    int direction = 1;
    Collider2D coll;

    public float ascendSpeed = 2.0f;
    public float highAltitude = 4f; //���� �� 
    public float lowAltitude = 3f; //���� �� 
    float targetAltitude;
    float targetTolerance = 0.1f; //������
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
    /// ���� �༺�� �߽� ��ġ�� �������� orbit�� ��ġ(����)�� ������.
    /// </summary>
    public void ResetCenterPoint()
    {
        curPlanet = null;
        coll = null;
    }

    //degree ������ �����մϴ�. 
    void SetDegree()
    {
        float signedAngle = Vector2.SignedAngle(Vector2.up, (Vector2)transform.position - centerPoint);
        degree = (-signedAngle + 360) % 360;

        transform.rotation = Quaternion.Euler(0, 0, degree * -1); //����� �ٶ󺸰� ���� ����
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

        //���� ����
        Vector2 moveAltitudeVec = SetAltitudeRoutine();

        var rad = Mathf.Deg2Rad * (degree);
        var x = radius * Mathf.Sin(rad);
        var y = radius * Mathf.Cos(rad);

        rb.MovePosition(centerPoint + new Vector2(x, y) + moveAltitudeVec);
        transform.rotation = Quaternion.Euler(0, 0, degree * -1); //����� �ٶ󺸰� ���� ����

        degree = (degree + (Time.deltaTime * moveSpeed * direction) + 360) % 360;


    }

    //���� ���߱� �ڷ�ƾ
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
