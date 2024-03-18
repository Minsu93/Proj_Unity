using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gravity : MonoBehaviour
{
    public List<Planet> gravityPlanets = new List<Planet>();
    public Planet nearestPlanet;
    protected Planet preNearestPlanet;
    protected float currentGravityForce;

    public Vector2 nearestPoint;
    public Vector2 nearestPointGravityVector { get; private set; }  //��ǥ�� ���� ����
    public float nearestPointFloorMagnitude { get; private set; }   //��ǥ����� �Ÿ�
    public PolygonCollider2D nearestCollider;

    public bool activate = true;

    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGravityForce = GameManager.Instance.worldGravity;
    }

    protected virtual void Update()
    {
        //���� ������ �ִ� �༺ üũ
        nearestPlanet = GetNearestPlanet();

        if (nearestPlanet == null) return;

        GetNearestPoint();

    }

    protected virtual void FixedUpdate()
    {
        if (!activate)
            return;

        if (nearestPlanet == null)
            return;

        Vector2 gravVec = nearestPoint - (Vector2)transform.position;

        rb.AddForce(gravVec.normalized * currentGravityForce * nearestPlanet.gravityMultiplier * Time.deltaTime, ForceMode2D.Force);
    }


    //public virtual Planet GetNearestPlanet()       //���� ����� Planet ��ũ��Ʈ�� �ش� ��ũ��Ʈ�� GravityForce�� �����´�.
    //{
    //    Planet targetPlanet = null;
    //    float minDist = 1000f;

    //    for (int i = 0; i < gravityPlanets.Count; i++)
    //    {
    //        if (gravityPlanets[i] == null)
    //            continue;

    //        Vector2 targetVec = gravityPlanets[i].transform.position - this.transform.position;

    //        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetVec.normalized, targetVec.magnitude, LayerMask.GetMask("Planet"));
    //        if (hit.collider != null)
    //        {
    //            if (hit.distance < minDist)
    //            {
    //                minDist = hit.distance;
    //                targetPlanet = gravityPlanets[i];
    //            }
    //        }

    //    }
    //    return targetPlanet;
    //}

    void GetNearestPoint()
    {
        nearestCollider = nearestPlanet.polyCollider;
        nearestPoint = nearestCollider.ClosestPoint(transform.position);
        Vector2 vec = nearestPoint - (Vector2)transform.position;
        nearestPointGravityVector = vec.normalized;
        nearestPointFloorMagnitude = vec.magnitude;

    }

    public Planet GetNearestPlanet()       //���� ����� Planet ��ũ��Ʈ�� �ش� ��ũ��Ʈ�� GravityForce�� �����´�.
    {
        Planet targetPlanet = null;
        float minDist = 1000f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            //�� planet���� edgecollider.nearestPoint������ �Ÿ��� ���Ѵ�. 
            Vector2 distVec = gravityPlanets[i].polyCollider.ClosestPoint(transform.position) - (Vector2)transform.position;
            float dist = distVec.magnitude;

            //���� ª�� ���� nearest planet���� �����Ѵ�. 
            if (dist < minDist)
            {
                minDist = dist;
                targetPlanet = gravityPlanets[i];
            }
        }

        //edgecol �� point ���� ���ÿ� ó���Ѵ�. 

        return targetPlanet;
    }



}
