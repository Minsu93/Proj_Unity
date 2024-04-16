using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class AttachToPlanet : MonoBehaviour
{
    [Header("Stick To Planet!")]
    public bool activate;
    
    [Header("Properties")]
    public bool isOrbital = false;
    public float range = 10f;


    public float yOffset = 0.5f;
    public float zRotation = 0;

    public PolygonCollider2D coll;
    public Planet currPlanet;



    [ContextMenu("GetNearestPlanet")]
    public void GetNearestPlanet()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, Vector2.right, 0f, LayerMask.GetMask("Planet"));

        if(hits.Length >0)
        {
            Transform nearestPlanet = null;
            float min = Mathf.Infinity;

            foreach(RaycastHit2D hit in hits)
            {
                Vector2 closestPoint = hit.collider.ClosestPoint(transform.position);
                Vector2 check = closestPoint - (Vector2)transform.position;
                if (check.magnitude < min)
                {
                    nearestPlanet = hit.transform;
                    min = check.magnitude;
                }
            }

            coll = nearestPlanet.GetComponent<PolygonCollider2D>();
            currPlanet = nearestPlanet.GetComponent<Planet>();

        }
    }
    private void Awake()
    {
        GetNearestPlanet();
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            activate = false;
        }

        if(!activate) { return; }

        GetNearestPlanet();
  
        if(coll == null)
            return;

        Vector2 point = coll.ClosestPoint(transform.position);
       
        //�븻�� ���ϰ� 
        Vector2 targetVec = ((Vector2)transform.position - point).normalized;


        if (!isOrbital)
        {
            //ĳ���͸� point�� �ű�� yOffset��ŭ ���� �̵���Ų��
            transform.position = point + (targetVec * yOffset);
        }
        //ĳ���͸� ȸ����Ű��
        transform.rotation = Quaternion.LookRotation(Vector3.forward, targetVec) * Quaternion.Euler(0,0,zRotation);

    }


}

