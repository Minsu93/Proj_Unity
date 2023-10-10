using PlanetSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGravity : Gravity
{
    public Vector2 nearestPoint;
    public EdgeCollider2D nearestEdgeCollider;
    [Header("Test")]
    public float maxGravDIst;
    public int oxygenInt = -1;



    protected override void Update()
    {
        if (!activate)
            return;

        base.Update();

        if (nearestPlanet == null)
            return;

        GetNearestPoint();



    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (nearestPlanet == null)
            return;

        if(nearestPlanet != preNearestPlanet)       //Planet이 바뀌면 한번만 발동한다.
        {
            ChangePlanet();
        }


        Vector2 grav = (nearestPoint - (Vector2)transform.position).normalized;

        float gravDist = (nearestPoint - (Vector2)transform.position).magnitude;        //지면과 가까울수록 더 중력이 강하게 적용
        gravDist = Mathf.Clamp(maxGravDIst / gravDist, 1, maxGravDIst * maxGravDIst);

        rb.AddForce(grav * currentGravityForce * gravityMultiplier * gravDist * Time.deltaTime, ForceMode2D.Force);
        return;
    }

    protected virtual void ChangePlanet()
    {

        //SendMessage("ChangePlanet", SendMessageOptions.DontRequireReceiver);    //행성 바꿈 이벤트 실행

        preNearestPlanet = nearestPlanet;
        currentGravityForce = nearestPlanet.gravityForce;   //행성별로 다른 중력 적용
    }

    void GetNearestPoint()
    {
        nearestEdgeCollider = nearestPlanet.edgeColl;
        nearestPoint = nearestEdgeCollider.ClosestPoint(transform.position);
    }

    public override Planet GetNearestPlanet()       //가장 가까운 Planet 스크립트와 해당 스크립트의 GravityForce를 가져온다.
    {
        Planet targetPlanet = null;
        float minDist = 1000f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            //각 planet들의 edgecollider.nearestPoint까지의 거리를 구한다. 
            Vector2 distVec = gravityPlanets[i].edgeColl.ClosestPoint(transform.position) - (Vector2)transform.position;
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
