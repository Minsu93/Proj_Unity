using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gravity : MonoBehaviour
{
    public bool activate = true;

    protected float currentGravityForce;    //월드 중력

    //참조
    public Vector2 nearestPoint;
    public Vector2 nearestPointGravityVector { get; private set; }  //지표면 방향 벡터
    public float nearestPointFloorMagnitude { get; private set; }   //지표면과의 거리

    public List<Planet> gravityPlanets = new List<Planet>();

    //스크립트들
    public Planet nearestPlanet;
    protected Planet preNearestPlanet;
    public PolygonCollider2D nearestCollider;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGravityForce = GameManager.Instance.worldGravity;
    }
    //private void OnEnable()
    //{
    //    nearestPlanet = GetNearestPlanet();
    //    Debug.Log("Enable :" + gravityPlanets.Count);
    //}

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

    public void AddToGravityList(Planet planet)
    {
        foreach(var gravity in gravityPlanets)
        {
            if (gravity == planet)
                return;
        }

        gravityPlanets.Add(planet);
        nearestPlanet = GetNearestPlanet();
    }

    public void RemoveFromGravityList(Planet planet)
    {
        gravityPlanets.Remove(planet);
    }

}
