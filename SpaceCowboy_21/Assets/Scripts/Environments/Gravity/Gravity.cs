using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetSpace;


public class Gravity : MonoBehaviour
{
    public List<Planet> gravityPlanets = new List<Planet>();
    public Planet nearestPlanet;
    protected Planet preNearestPlanet;
    protected float currentGravityForce;

    public bool activate = true;

    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGravityForce = GameManager.Instance.worldGravity;
    }

    protected virtual void Update()
    {
        if (!activate)
            return;

        //가장 가까이 있는 행성 체크
        nearestPlanet = GetNearestPlanet();


    }

    protected virtual void FixedUpdate()
    {
        if (!activate)
            return;

    }

    public virtual Planet GetNearestPlanet()       //가장 가까운 Planet 스크립트와 해당 스크립트의 GravityForce를 가져온다.
    {
        Planet targetPlanet = null;
        float minDist = 1000f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            Vector2 targetVec = gravityPlanets[i].transform.position - this.transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetVec.normalized, targetVec.magnitude, LayerMask.GetMask("Planet"));
            if (hit.collider != null)
            {
                if (hit.distance < minDist)
                {
                    minDist = hit.distance;
                    targetPlanet = gravityPlanets[i];
                }
            }

            /*
            else //충돌되는 것이 없으면, 그냥 transform의 중점 방향으로 끌어당긴다.
            {
                if (targetVec.magnitude < minDist)
                {
                    minDist = targetVec.magnitude;
                    targetPlanet = gravityPlanets[i];
                }
            }
            */

        }
        return targetPlanet;
    }





}
