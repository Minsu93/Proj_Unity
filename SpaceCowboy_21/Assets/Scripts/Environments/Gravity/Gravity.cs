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
    public Vector2 nearestPointGravityVector { get; private set; }  //지표면 방향 벡터
    public float nearestPointFloorMagnitude { get; private set; }   //지표면과의 거리
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
        //가장 가까이 있는 행성 체크
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


    //public virtual Planet GetNearestPlanet()       //가장 가까운 Planet 스크립트와 해당 스크립트의 GravityForce를 가져온다.
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

    public Planet GetNearestPlanet()       //가장 가까운 Planet 스크립트와 해당 스크립트의 GravityForce를 가져온다.
    {
        Planet targetPlanet = null;
        float minDist = 1000f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            //각 planet들의 edgecollider.nearestPoint까지의 거리를 구한다. 
            Vector2 distVec = gravityPlanets[i].polyCollider.ClosestPoint(transform.position) - (Vector2)transform.position;
            float dist = distVec.magnitude;

            //가장 짧은 곳을 nearest planet으로 지정한다. 
            if (dist < minDist)
            {
                minDist = dist;
                targetPlanet = gravityPlanets[i];
            }
        }

        //edgecol 과 point 까지 동시에 처리한다. 

        return targetPlanet;
    }



}
