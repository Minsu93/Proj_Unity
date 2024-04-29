using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gravity : MonoBehaviour
{
    public bool activate = true;

    protected float currentGravityForce;    //월드 중력

    //참조
    public Vector2 nearestPoint;

    //Meteor용 고정 방향 중력
    public bool fixedGravity = false;
    Planet fixedPlanet;
    Vector2 fixedPoint;
    Vector2 fixedGravityVector;
    float fixedGravitySpeed;

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


    protected virtual void FixedUpdate()
    {
        if (fixedGravity)
        {
            rb.AddForce(fixedGravitySpeed * currentGravityForce * Time.deltaTime * fixedGravityVector, ForceMode2D.Force);
            return;
        }

        //가장 가까이 있는 행성 체크
        if (GetNearestPlanet(out Planet planet))
        {
            nearestPoint = GetNearestPoint(planet);
        }

        nearestPlanet = planet;

        if (nearestPlanet != preNearestPlanet)       //Planet이 바뀌면 한번만 발동한다.
        {
            ChangePlanet();
        }

        if (nearestPlanet == null) return;
        if (!activate) return;

        GravityFunction();
    }

    protected virtual void ChangePlanet()
    {
        preNearestPlanet = nearestPlanet;

    }

    protected virtual void GravityFunction()
    {
        Vector2 gravVec = nearestPoint - (Vector2)transform.position;

        rb.AddForce(currentGravityForce * nearestPlanet.gravityMultiplier * Time.deltaTime * gravVec.normalized, ForceMode2D.Force);
    }

    public void FixedGravityFunction(Planet planet, float speed)
    {
        Debug.Log("fixedGravityOn");

        fixedGravity = true;
        fixedPlanet = planet;
        //fixedPoint = GetNearestPoint(fixedPlanet);
        fixedPoint = GameManager.Instance.player.position;
        fixedGravitySpeed = speed;
       
        Vector2 gravVec;
        if (fixedPlanet != null)
            gravVec = fixedPlanet.transform.position - transform.position;
        else gravVec = fixedPoint - (Vector2)transform.position;

        fixedGravityVector = gravVec.normalized;

    }
    public void CancelFixedGravity()
    {
        fixedGravity = false;
    }

    Vector2 GetNearestPoint(Planet _planet)
    {
        nearestCollider = _planet.polyCollider;        
        return nearestCollider.ClosestPoint(transform.position);
    }

    public bool GetNearestPlanet(out Planet planet)       //가장 가까운 Planet 스크립트와 해당 스크립트의 GravityForce를 가져온다.
    {
        Planet targetPlanet = null;
        float minDist = 1000f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            Vector2 distVec = gravityPlanets[i].polyCollider.ClosestPoint(transform.position) - (Vector2)transform.position;
            float dist = distVec.magnitude;

            if (dist < minDist)
            {
                minDist = dist;
                targetPlanet = gravityPlanets[i];
            }
        }
        planet = targetPlanet;

        return planet == null ? false : true;
    }

    public void AddToGravityList(Planet planet)
    {
        foreach(var gravity in gravityPlanets)
        {
            if (gravity == planet)
                return;
        }

        gravityPlanets.Add(planet);
    }

    public void RemoveFromGravityList(Planet planet)
    {
        gravityPlanets.Remove(planet);
    }

}
