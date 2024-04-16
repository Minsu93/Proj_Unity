using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase_Orbit : EnemyChase
{
    Vector2 centerPoint;
    int direction = 1;
    Collider2D coll;

    public float highAltitude = 4f; //���� �� 
    public float lowAltitude = 3f; //���� �� 

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
    }

    public override void OnChaseAction()
    {
        if (curPlanet == null) return;

        Vector2 targetVec = ((Vector2)transform.position - centerPoint).normalized;
        
        //ȸ�� 
        transform.rotation = Quaternion.LookRotation(Vector3.forward, targetVec);
        //���� ����
        SetAltitudeRoutine();
        //ĳ���� �ӵ� ����
        //�̵� 
        //transform.RotateAround(centerPoint, Vector3.forward, direction * moveSpeed * Time.deltaTime);
    }

    //���� ���߱� �ڷ�ƾ
    void SetAltitudeRoutine()
    {
        Vector2 closePoint = coll.ClosestPoint(transform.position);
        Vector2 upVec = (Vector2)transform.position - closePoint;
        float dist = upVec.magnitude;
        Vector2 upDir = upVec.normalized;
        Debug.DrawRay(transform.position, upDir, Color.green);
        altitude = dist;
        float speed = 3f;

        if (dist > highAltitude)
        {
            //rb.MovePosition(rb.position + (speed * Time.deltaTime * -upDir));
            transform.Translate((speed * Time.deltaTime * -upDir));

        }
        else if (dist < lowAltitude)
        {
            //rb.MovePosition(rb.position + (speed * Time.deltaTime * upDir));
            transform.Translate((speed * Time.deltaTime * upDir));
        }
    }
}
