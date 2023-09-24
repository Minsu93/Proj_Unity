using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGravity : Gravity
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();//순서대로 작동하는게 아니라, 자손에서 부모의 Update를 실행하는것. 아래쪽과는 관련 X...

        if (!activate)
            return; 

        if (nearestPlanet == null)
            return;

        if (nearestPlanet != preNearestPlanet)       //Planet이 바뀌면 한번만 발동한다.
        {
            preNearestPlanet = nearestPlanet;
            currentGravityForce = nearestPlanet.gravityForce;
        }

        float totalGrav = 0f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            Planet planet = gravityPlanets[i].GetComponent<Planet>();

            float planetRadius = planet.planetRadius;   //행성의 반지름
            float gravRadius = planet.gravityRadius;   //행성의 중력 반지름

            Vector2 targetVec = gravityPlanets[i].transform.position - this.transform.position;   //중력의 방향

            if (targetVec.magnitude < planetRadius)  // 행성 안쪽인 경우 해당 행성의 중력만 통째로 받는다
            {
                totalGrav = 1f;
            }
            else
            {
                float upVec = gravityPlanets[i] == nearestPlanet ? 1f : -1f;
                float percent = Mathf.Clamp01((targetVec.magnitude - planetRadius) / (gravRadius - planetRadius));
                float tempGrav = upVec * (1f - percent);

                totalGrav += tempGrav;
            }

        }

        Vector2 gravDir = (nearestPlanet.transform.position - this.transform.position).normalized;
        rb.AddForce(gravDir * totalGrav * currentGravityForce * gravityMultiplier * Time.deltaTime, ForceMode2D.Force);

        //Vector2 currDir = rb.velocity;
        //float vel = currDir.magnitude;
        //currDir = currDir + (gravDir * totalGrav * currentGravityForce * gravityMultiplier * 0.1f * Time.deltaTime);

        //rb.velocity = currDir.normalized * vel;

    }
}
