using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEdge : MonoBehaviour
{
    public EdgeCollider2D edgeCollider; // 따라갈 엣지 콜라이더

    private Rigidbody2D rb;
    private int currentEdgePointIndex = 0; // 현재 따라가고 있는 엣지 콜라이더의 점 인덱스

    public float moveSpeed = 5f; // 오브젝트의 이동 속도
    public float rotLerpSpeed = 5f; //오브젝트의 회전 보간 속도

    public Vector2 pointA;
    public Vector2 pointB;
    float moveDist;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetClosestPoint();
    }

    private void FixedUpdate()
    {
        if (edgeCollider == null || edgeCollider.points.Length == 0)
        {
            return;
        }

        //오브젝트의 회전
        pointA = edgeCollider.transform.TransformPoint(edgeCollider.points[currentEdgePointIndex]);
        pointB = edgeCollider.transform.TransformPoint(edgeCollider.points[(currentEdgePointIndex + 1) % (edgeCollider.points.Length - 1)]);
        
        Debug.DrawLine(pointA, pointB, Color.black);

        Vector2 edgeVector = pointB - pointA;
        Vector2 normal = new Vector2(-edgeVector.y, edgeVector.x).normalized;

        Quaternion nor = Quaternion.LookRotation(forward: Vector3.forward, upwards:normal);
        transform.rotation = Quaternion.Lerp(transform.rotation, nor, moveSpeed * Time.fixedDeltaTime);     // point에서 normal방향으로 오브젝트 회전



        //현재 타겟까지의 거리를 구한다. 만약 그것이 다음 이동거리보다 짧다면, 타겟을 수정한 뒤 이 행위를 반복한다. 

        Vector2 targetPoint;
        Vector2 moveVector;

        do
        {
            targetPoint = edgeCollider.transform.TransformPoint(edgeCollider.points[currentEdgePointIndex]);
            moveVector = (targetPoint + (normal * 0.5f)) - rb.position;
            moveDist = moveVector.magnitude;

            if(moveDist < moveSpeed * Time.fixedDeltaTime)
            {
                currentEdgePointIndex = (currentEdgePointIndex + 1) % (edgeCollider.points.Length - 1);
            }
        }
        while (moveDist < moveSpeed * Time.fixedDeltaTime);

        Vector2 moveNor = moveVector.normalized;

        // 오브젝트를 이동 방향으로 이동
        rb.MovePosition(rb.position + moveNor * moveSpeed * Time.fixedDeltaTime);

    }

    //바닥에 닿았을 때 실행
    private void GetClosestPoint()
    {
        int closestPointIndex = 0;
        float closestDistance = float.MaxValue;

        // 엣지 콜라이더의 모든 포인트들을 순회하며 가장 가까운 포인트의 인덱스와 거리를 구함
        for (int i = 0; i < edgeCollider.points.Length; i++)
        {
            Vector2 pointPosition = edgeCollider.transform.TransformPoint(edgeCollider.points[i]);
            float distance = Vector2.Distance(rb.position, pointPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPointIndex = i;
            }
        }

        currentEdgePointIndex = closestPointIndex;
    }
}
