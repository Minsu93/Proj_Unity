using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEdge : MonoBehaviour
{
    public EdgeCollider2D edgeCollider; // ���� ���� �ݶ��̴�

    private Rigidbody2D rb;
    private int currentEdgePointIndex = 0; // ���� ���󰡰� �ִ� ���� �ݶ��̴��� �� �ε���

    public float moveSpeed = 5f; // ������Ʈ�� �̵� �ӵ�
    public float rotLerpSpeed = 5f; //������Ʈ�� ȸ�� ���� �ӵ�

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

        //������Ʈ�� ȸ��
        pointA = edgeCollider.transform.TransformPoint(edgeCollider.points[currentEdgePointIndex]);
        pointB = edgeCollider.transform.TransformPoint(edgeCollider.points[(currentEdgePointIndex + 1) % (edgeCollider.points.Length - 1)]);
        
        Debug.DrawLine(pointA, pointB, Color.black);

        Vector2 edgeVector = pointB - pointA;
        Vector2 normal = new Vector2(-edgeVector.y, edgeVector.x).normalized;

        Quaternion nor = Quaternion.LookRotation(forward: Vector3.forward, upwards:normal);
        transform.rotation = Quaternion.Lerp(transform.rotation, nor, moveSpeed * Time.fixedDeltaTime);     // point���� normal�������� ������Ʈ ȸ��



        //���� Ÿ�ٱ����� �Ÿ��� ���Ѵ�. ���� �װ��� ���� �̵��Ÿ����� ª�ٸ�, Ÿ���� ������ �� �� ������ �ݺ��Ѵ�. 

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

        // ������Ʈ�� �̵� �������� �̵�
        rb.MovePosition(rb.position + moveNor * moveSpeed * Time.fixedDeltaTime);

    }

    //�ٴڿ� ����� �� ����
    private void GetClosestPoint()
    {
        int closestPointIndex = 0;
        float closestDistance = float.MaxValue;

        // ���� �ݶ��̴��� ��� ����Ʈ���� ��ȸ�ϸ� ���� ����� ����Ʈ�� �ε����� �Ÿ��� ����
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
