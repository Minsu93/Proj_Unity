using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class AttachToPlanet : MonoBehaviour
{
    public bool stickToPlanet = false;
    public float yOffset;
    [SerializeField] Collider2D coll;

    private void Awake()
    {
        GetNearestPlanet();
    }

    public void MyEvent()
    {
        Debug.Log("My Event");
    }

    public void GetNearestPlanet()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 5f, Vector2.right, 0f, LayerMask.GetMask("Planet"));

        if(hits.Length >0)
        {
            Collider2D minColl = null;
            float min = Mathf.Infinity;

            foreach(RaycastHit2D hit in hits)
            {
                Vector2 closestPoint = hit.collider.ClosestPoint(transform.position);
                Vector2 check = closestPoint - (Vector2)transform.position;
                if (check.magnitude < min)
                {
                    minColl = hit.collider;
                    min = check.magnitude;
                }
            }

            coll = minColl;
        }
    }

    public void UpdatePosition()
    {
        if (stickToPlanet)
        {
            stickToPlanet = false;
        }
        else
        {
            stickToPlanet = true;
        }
    }

    void Update()
    {
        /*
        if(Editor8Application.isPlaying)
        {
            this.enabled = false;
        }
        */

        if(coll == null)
        {
            return;
        }

        if (!stickToPlanet)
            return;

        Vector2 point = coll.ClosestPoint(transform.position);

        //�븻�� ���ϰ� 
        Vector2 targetVec = ((Vector2)transform.position - point).normalized;

        //ĳ���͸� point�� �ű�� yOffset��ŭ ���� �̵���Ų��
        transform.position = point + (targetVec * yOffset);
        //ĳ���͸� ȸ����Ű��
        transform.rotation = Quaternion.LookRotation(Vector3.forward, targetVec);

    }
}
